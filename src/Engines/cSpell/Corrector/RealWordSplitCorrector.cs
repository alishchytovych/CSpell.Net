using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Candidates;
using SpellChecker.Engines.cSpell.Detector;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Ranker;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Corrector {
	/// <summary>
	///***************************************************************************
	/// This class is to correct real-word split error.
	/// 
	/// <para><b>History:</b>
	/// <ul>
	/// <li>2018 baseline
	/// </ul>
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ****************************************************************************
	/// </para>
	/// </summary>
	public class RealWordSplitCorrector {
		// private constructor
		/// <summary>
		/// Private constructor so no one can instantiate
		/// </summary>
		private RealWordSplitCorrector() { }
		// public method
		/// <summary>
		/// The core method to correct a word by following steps:
		/// <ul>
		/// <li>Convert inToken to coreTerm
		/// <li>detect if real-word
		/// <li>get split candidates
		/// <li>Rank candidates
		///     <ul>
		///     <li>context
		///     </ul>
		/// <li>Update information
		/// 
		/// </ul>
		/// </summary>
		/// <param name="inTokenObj">    the input tokenObj (single word) </param>
		/// <param name="cSpellApi"> cSpell API object </param>
		/// <param name="debugFlag"> flag for debug print </param>
		/// <param name="tarPos"> position of the target token to be split </param>
		/// <param name="nonSpaceTokenList"> the token list without space tokens
		/// </param>
		/// <returns>    the split words in tokenObj.  </returns>
		// return the original term if no good correctin are found
		public static TokenObj GetCorrectTerm(TokenObj inTokenObj, CSpellApi cSpellApi, bool debugFlag, int tarPos, List<TokenObj> nonSpaceTokenList) {
			// init
			int funcMode = cSpellApi.GetFuncMode();

			// get inWord from inTokenObj and init outTokenObj
			string inWord = inTokenObj.GetTokenStr();
			TokenObj outTokenObj = new TokenObj(inTokenObj);
			// 1. convert a word to coreTerm (no leading/ending space, punc, digit)
			int ctType = CoreTermUtil.CT_TYPE_SPACE_PUNC_DIGIT;
			CoreTermObj coreTermObj = new CoreTermObj(inWord, ctType);
			string coreStr = coreTermObj.GetCoreTerm();
			// 2. non-word detection and correction 
			// check if the coreTerm is real-word
			if ((inTokenObj.GetProcHist().Count == 0) && (RealWordSplitDetector.IsDetect(inWord, coreStr, cSpellApi, debugFlag) == true)) {
				cSpellApi.UpdateDetectNo();
				// TBD, should take care of possessive xxx's here
				// 3. get split candidates set from correction
				int maxSplitNo = cSpellApi.GetCanRwMaxSplitNo();
				HashSet<string> splitSet = RealWordSplitCandidates.GetCandidates(coreStr, cSpellApi, maxSplitNo);
				// get candidates from split
				// 4. Ranking: get top ranked candidates as corrected terms
				// in case of using context
				string topRankStr = RankRealWordSplitByMode.GetTopRankStr(coreStr, splitSet, cSpellApi, debugFlag, tarPos, nonSpaceTokenList);
				// 5 update coreTerm and convert back to tokenObj
				coreTermObj.SetCoreTerm(topRankStr);
				string outWord = coreTermObj.ToString();
				// 6. update info if there is a real-word correction
				if (inWord.Equals(outWord) == false) {
					cSpellApi.UpdateCorrectNo();
					outTokenObj.SetTokenStr(outWord);
					outTokenObj.AddProcToHist(TokenObj.HIST_RW_S); //split
					DebugPrint.PrintCorrect("RW", "RealWordSplitCorrector", inWord, outWord, debugFlag);
				}
			}
			return outTokenObj;
		}
		private static void TestSplit(CSpellApi cSpellApi) {
			// setup test case
			// 10349.txt
			//String inText = "sounding in my ear every time for along time.";
			// 13864.txt
			string inText = "I donate my self to be apart of this study.";
			TextObj textObj = new TextObj(inText);
			List<TokenObj> inTextList = textObj.GetTokenList();
			List<TokenObj> nonSpaceTokenList = TextObj.GetNonSpaceTokenObjList(inTextList);
			//int tarPos = 7;
			int tarPos = 6;
			TokenObj inTokenObj = nonSpaceTokenList[tarPos];
			bool debugFlag = false;
			Console.WriteLine("====== Real-Word One-To-One Correction Test =====");
			Console.WriteLine("-- inTextList: [" + inText + "]");
			Console.WriteLine("-- tarPos: [" + tarPos + "]");
			Console.WriteLine("-- inTokenObj: [" + inTokenObj.ToString() + "]");
			// get the correct term
			TokenObj outTokenObj = GetCorrectTerm(inTokenObj, cSpellApi, debugFlag, tarPos, nonSpaceTokenList);
			// print out
			Console.WriteLine("--------- GetCorrectTermStr( ) -----------");
			Console.WriteLine("-- outTokenObj: [" + outTokenObj.ToString() + "]");
		}
		// test driver
		public static void MainTest(string[] args) {
			string configFile = "../data/Config/cSpell.properties";
			if (args.Length == 1) {
				configFile = args[0];
			} else if (args.Length > 0) {
				Console.WriteLine("Usage: java RealWordCorrector <configFile>");
				Environment.Exit(0);
			}

			// init
			CSpellApi cSpellApi = new CSpellApi(configFile);

			// test
			TestSplit(cSpellApi);
		}
		// data member
	}

}
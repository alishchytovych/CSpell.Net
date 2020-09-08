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
	/// This class is to correct real-word 1To1 error.
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
	public class RealWord1To1Corrector {
		// private constructor
		/// <summary>
		/// Private constructor so no one can instantiate
		/// </summary>
		private RealWord1To1Corrector() { }
		// public method
		/// <summary>
		/// The core method to correct a word by following steps:
		/// <ul>
		/// <li>Convert inToken to coreTerm
		/// <li>detect if real-word
		/// <li>get candidates
		///     <ul>
		///     <li>get candidates from one-to-one.
		///     </ul>
		/// <li>Rank candidates
		///     <ul>
		///     <li>context
		///     </ul>
		/// <li>Update information
		/// 
		/// </ul>
		/// </summary>
		/// <param name="inTokenObj">    the input tokenObj (single word) </param>
		/// <param name="cSpellApi"> CSpell Api object </param>
		/// <param name="debugFlag"> flag for debug print </param>
		/// <param name="tarPos"> the position for target token </param>
		/// <param name="nonSpaceTokenList"> token list without space token(s)
		/// </param>
		/// <returns>    the corrected word in tokenObj if suggested word found. 
		///             Otherwise, the original input token is returned. </returns>
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
			// 2. real-word detection and correction 
			// check if the coreTerm is real-word
			if ((inTokenObj.GetProcHist().Count == 0) && (RealWord1To1Detector.IsDetect(inWord, coreStr, cSpellApi, debugFlag) == true)) {
				cSpellApi.UpdateDetectNo();
				// TBD, should take care of possessive xxx's here
				// 3 get 1-to-1 candidates set from correction
				// TBD. realWordFlag to use metaphone ...
				// this process is very slow, 7 min., need to improved
				HashSet<string> candSet = RealWord1To1Candidates.GetCandidates(coreStr, cSpellApi);
				/// <summary>
				///** development analysis print out to see total RW
				///            totalRwNo_++;
				///            int candSize = candSet.size();
				///            if(candSize != 0)
				///            {
				///                totalCandNo_ += candSize;
				///                maxCandSize_ 
				///                    = ((candSize > maxCandSize_)?candSize:maxCandSize_);
				///                System.out.println("---- totalRwNo|totalCandNo(" + coreStr
				///                    + "): " + totalRwNo_ + "|" + candSize + "|" 
				///                    + totalCandNo_ + "|" + maxCandSize_);
				///                System.out.println(candSet);    
				///            }
				/// ***
				/// </summary>
				// 4. Ranking: get top ranked candidates as corrected terms
				// in case of using context
				string topRankStr = RankRealWord1To1ByCSpell.GetTopRankStr(coreStr, candSet, cSpellApi, tarPos, nonSpaceTokenList, debugFlag);
				// 5 update coreTerm and convert back to tokenObj
				coreTermObj.SetCoreTerm(topRankStr);
				string outWord = coreTermObj.ToString();
				// 6. update info if there is a real-word correction
				if (inWord.Equals(outWord, StringComparison.OrdinalIgnoreCase) == false) {
					cSpellApi.UpdateCorrectNo();
					outTokenObj.SetTokenStr(outWord);
					outTokenObj.AddProcToHist(TokenObj.HIST_RW_1); // 1-to-1
					DebugPrint.PrintCorrect("RW", "RealWord1To1Corrector", inWord, outWord, debugFlag);
				}
			}
			return outTokenObj;
		}
		private static void Test1To1(CSpellApi cSpellApi) {
			// setup test case
			// 51.txt
			//String inText = "You'd thing that this is good.";
			//String inText = "The doctor thing that this is good.";
			string inText = "you would thing that is good.";
			TextObj textObj = new TextObj(inText);
			List<TokenObj> inTextList = textObj.GetTokenList();
			List<TokenObj> nonSpaceTokenList = TextObj.GetNonSpaceTokenObjList(inTextList);
			int tarPos = 2;
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
				Console.WriteLine("Usage: java RealWord1To1Corrector <configFile>");
				Environment.Exit(0);
			}

			// init
			CSpellApi cSpellApi = new CSpellApi(configFile);

			// test
			Test1To1(cSpellApi);
		}
		// data member
	}

}
using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Candidates;
using SpellChecker.Engines.cSpell.Detector;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Extensions;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Ranker;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Corrector {
	/// <summary>
	///***************************************************************************
	/// This class is to correct non-word one-to-one and split errors.
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
	public class NonWordCorrector {
		// private constructor
		/// <summary>
		/// Private constructor so no one can instantiate
		/// </summary>
		private NonWordCorrector() { }
		// public method
		/// <summary>
		/// The core method to correct a word by following steps:
		/// <ul>
		/// <li>Convert inToken to coreTerm
		/// <li>detect if misspell (OOV) - non-word
		/// <li>get candidates
		///     <ul>
		///     <li>get candidates from 1To1.
		///     <li>get candidates from split.
		///     </ul>
		/// <li>Rank candidates
		/// <li>Update information
		/// </ul>
		/// 
		/// This method does not use context scores.
		/// </summary>
		/// <param name="inTokenObj">    the input tokenObj (single word) </param>
		/// <param name="cSpellApi"> CSpell Api object
		/// </param>
		/// <returns>    the corrected word in tokenObj if the coreTerm is OOV 
		///             and suggested word found. Otherwise, the original input token 
		///             is returned. </returns>
		public static TokenObj GetCorrectTerm(TokenObj inTokenObj, CSpellApi cSpellApi) {
			bool debugFlag = false;
			return GetCorrectTerm(inTokenObj, cSpellApi, debugFlag);
		}
		/// <summary>
		/// This method does not use context scores to find the correct term.
		/// </summary>
		/// <param name="inTokenObj">    the input tokenObj (single word) </param>
		/// <param name="cSpellApi"> CSpell Api object </param>
		/// <param name="debugFlag"> flag for debug print
		/// </param>
		/// <returns>    the corrected word in tokenObj if the coreTerm is OOV 
		///             and suggested word found. Otherwise, the original input token 
		///             is returned. </returns>
		public static TokenObj GetCorrectTerm(TokenObj inTokenObj, CSpellApi cSpellApi, bool debugFlag) {
			int tarPos = 0; // set to 0 if not use context
			List<TokenObj> nonSpaceTokenList = null;
			return GetCorrectTerm(inTokenObj, cSpellApi, debugFlag, tarPos, nonSpaceTokenList);
		}
		/// <summary>
		/// This method uses context scores to find the correct term.
		/// </summary>
		/// <param name="inTokenObj">    the input tokenObj (single word) </param>
		/// <param name="cSpellApi"> CSpell Api object </param>
		/// <param name="debugFlag"> flag for debug print </param>
		/// <param name="tarPos"> position for target token </param>
		/// <param name="nonSpaceTokenList"> token list without space token(s)
		/// </param>
		/// <returns>    the corrected word in tokenObj if the coreTerm is OOV 
		///             and suggested word found. Otherwise, the original input token 
		///             is returned. </returns>
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
			// check if the coreTerm is spelling errors - non-word
			//!NonWordDetector.IsValidWord(inWord, coreStr, cSpellApi, debugFlag);
			// TBD .. need to separate 1-to-1 and split
			if (NonWordDetector.IsDetect(inWord, coreStr, cSpellApi, debugFlag) == true) {
				cSpellApi.UpdateDetectNo();
				// TBD, should take care of possessive xxx's here
				// 3.1 get 1-to-1 candidates set from correction, no split
				HashSet<string> candSet = NonWord1To1Candidates.GetCandidates(coreStr, cSpellApi);
				// add split
				// TBD ... 
				if (funcMode != CSpellApi.FUNC_MODE_NW_1) {
					// 3.2 get candidates from split
					int maxSplitNo = cSpellApi.GetCanNwMaxSplitNo();
					HashSet<string> splitSet = NonWordSplitCandidates.GetCandidates(coreStr, cSpellApi, maxSplitNo);
					// 3.4 set split candidates to candidate
					if (funcMode == CSpellApi.FUNC_MODE_NW_S) {
						candSet = new HashSet<string>(splitSet);
					} else { // 3.4 add split candidates
						candSet.addAll(splitSet);
					}
				}
				// 4. Ranking: get top ranked candidates as corrected terms
				// 4.1 from orthoGraphic
				/*
				// not used context
				String topRankStr = RankByMode.GetTopRankStr(coreStr, candSet, 
				    cSpellApi, debugFlag);
				*/
				// in case of using context
				string topRankStr = RankNonWordByMode.GetTopRankStr(coreStr, candSet, cSpellApi, debugFlag, tarPos, nonSpaceTokenList);
				// 5 update coreTerm and convert back to tokenObj
				coreTermObj.SetCoreTerm(topRankStr);
				string outWord = coreTermObj.ToString();
				// 6. update info if there is a process
				if (inWord.Equals(outWord) == false) {
					outTokenObj.SetTokenStr(outWord);
					if (TermUtil.IsMultiword(outWord) == true) {
						cSpellApi.UpdateCorrectNo();
						outTokenObj.AddProcToHist(TokenObj.HIST_NW_S); //split
						DebugPrint.PrintCorrect("NW", "NonWordCorrector-Split", inWord, outWord, debugFlag);
					} else { // 1To1 correct
						cSpellApi.UpdateCorrectNo();
						outTokenObj.AddProcToHist(TokenObj.HIST_NW_1);
						DebugPrint.PrintCorrect("NW", "NonWordCorrector-1To1", inWord, outWord, debugFlag);
					}
				}
			}
			return outTokenObj;
		}
		private static string GetCorrectTermStr(string inWord, CSpellApi cSpellApi) {
			TokenObj inTokenObj = new TokenObj(inWord);
			TokenObj outTokenObj = GetCorrectTerm(inTokenObj, cSpellApi);
			string outWord = outTokenObj.GetTokenStr();
			return outWord;
		}
		private static void TestGetCorrectTermStr(CSpellApi cSpellApi) {
			string inText = "playsure"; // wordwise: pleasure
			string outText = GetCorrectTermStr(inText, cSpellApi);
			// print out
			Console.WriteLine("--------- GetCorrectTermStr( ) -----------");
			Console.WriteLine("In: [" + inText + "]");
			Console.WriteLine("Out: [" + outText + "]");
		}
		private static void TestGetCorrectTerm(CSpellApi cSpellApi) {
			// init
			// all lowerCase
			string inText = "hotflashes";
			// test process: 
			TokenObj inToken = new TokenObj(inText);
			TokenObj outToken = NonWordCorrector.GetCorrectTerm(inToken, cSpellApi);
			// result    
			string outText = outToken.GetTokenStr();
			// print out
			Console.WriteLine("--------- GetCorrectTerm( ) -----------");
			Console.WriteLine("In: [" + inText + "]");
			Console.WriteLine("Out: [" + outText + "]");
		}
		// test driver
		public static void MainTest(string[] args) {
			string configFile = "../data/Config/cSpell.properties";
			if (args.Length > 0) {
				Console.WriteLine("Usage: java NonWordCorrector <configFile>");
				Environment.Exit(0);
			}

			// init
			CSpellApi cSpellApi = new CSpellApi(configFile);

			// test
			TestGetCorrectTermStr(cSpellApi);
			TestGetCorrectTerm(cSpellApi);
		}
		// data member
	}

}
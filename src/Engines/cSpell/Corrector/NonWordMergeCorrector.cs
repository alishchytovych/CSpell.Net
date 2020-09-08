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
	/// This class is to correct non-word merge error.
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
	public class NonWordMergeCorrector {
		// private constructor
		/// <summary>
		/// Private constructor so no one can instantiate
		/// </summary>
		private NonWordMergeCorrector() { }
		// public method
		/// <summary>
		/// The core method to correct a word by following steps:
		/// <ul>
		/// <li>Convert inToken to removeEndPuncStr
		/// <li>detect if misspell (OOV) - non-word, exclude Aa
		/// <li>get candidates
		///     <ul>
		///     <li>get candidates from merge.
		///     </ul>
		/// <li>Rank candidates
		///     <ul>
		///     <li>orthographic
		///     <li>frequency
		///     <li>context
		///     </ul>
		/// <li>Update information
		/// 
		/// </ul>
		/// </summary>
		/// <param name="tarPos">    postion of target token </param>
		/// <param name="nonSpaceTokenList"> token list without space token(s) </param>
		/// <param name="cSpellApi"> CSpell Api object </param>
		/// <param name="debugFlag"> flag for debug print
		/// </param>
		/// <returns>    the corrected merged word in MergeObj if the token is OOV 
		///             and suggested merged word found. 
		///             Otherwise, a null of MergeObj is returned. </returns>
		// return the original term if no good correctin are found
		public static MergeObj GetCorrectTerm(int tarPos, List<TokenObj> nonSpaceTokenList, CSpellApi cSpellApi, bool debugFlag) {
			// get tarWord from tarTokenObj and init outTokenObj
			TokenObj tarTokenObj = nonSpaceTokenList[tarPos];
			string tarWord = tarTokenObj.GetTokenStr();
			MergeObj outMergeObj = null; // no merge if it is null
			// 1. only remove ending punctuation for coreTerm
			string coreStr = TermUtil.StripEndPuncSpace(tarWord).ToLower();
			// 2. non-word correction 
			// check if tarWord and removeEndPuncStr is OOV
			if (NonWordMergeDetector.IsDetect(tarWord, coreStr, cSpellApi, debugFlag) == true) {
				cSpellApi.UpdateDetectNo();
				// 3. get candidates from merge
				HashSet<MergeObj> mergeSet = NonWordMergeCandidates.GetCandidates(tarPos, nonSpaceTokenList, cSpellApi);
				// 4. Ranking: get top ranked candidates as corrected terms
				// 4.1 just use frenquency or context, no orthoGraphic
				// in case of using context
				outMergeObj = RankNonWordMergeByMode.GetTopRankMergeObj(mergeSet, cSpellApi, tarPos, nonSpaceTokenList, debugFlag);
			}
			return outMergeObj;
		}
		private static void TestGetCorrectTerm(CSpellApi cSpellApi) {
			// init
			// all lowerCase
			string inText = "Dur ing my absent.";
			bool debugFlag = false;
			List<TokenObj> inTokenList = TextObj.TextToTokenList(inText);
			// 1. convert to the non-empty token list
			List<TokenObj> nonSpaceTokenList = TextObj.GetNonSpaceTokenObjList(inTokenList);
			// result    
			int tarPos = 0;
			MergeObj mergeObj = NonWordMergeCorrector.GetCorrectTerm(tarPos, nonSpaceTokenList, cSpellApi, debugFlag);
			// print out
			Console.WriteLine("--------- GetCorrectTerm( ) -----------");
			Console.WriteLine("In: [" + inText + "]");
			Console.WriteLine("In nonSpaceTokenList: [" + nonSpaceTokenList.Count + "]");
			Console.WriteLine("Out MergeObj: [" + mergeObj.ToString() + "]");
		}
		// test driver
		public static void MainTest(string[] args) {
			string configFile = "../data/Config/cSpell.properties";
			if (args.Length > 0) {
				Console.WriteLine("Usage: java NonWordMergeCorrector <configFile>");
				Environment.Exit(0);
			}

			// init
			CSpellApi cSpellApi = new CSpellApi(configFile);

			// test
			TestGetCorrectTerm(cSpellApi);
		}
		// data member
	}

}
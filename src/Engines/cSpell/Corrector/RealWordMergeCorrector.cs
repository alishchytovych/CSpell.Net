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
	/// This class is to correct real-word merge error.
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
	public class RealWordMergeCorrector {
		// private constructor
		/// <summary>
		/// Private constructor so no one can instantiate
		/// </summary>
		private RealWordMergeCorrector() { }
		// public method
		/// <summary>
		/// The core method to correct a word by following steps:
		/// <ul>
		/// <li>detect if real-word for merge
		/// <li>get candidates
		///     <ul>
		///     <li>get candidates from merge.
		///     </ul>
		/// <li>Rank candidates
		///     <ul>
		///     <li>context
		///     <li>frequency (TBD)
		///     </ul>
		/// <li>Update information
		/// 
		/// </ul>
		/// </summary>
		/// <param name="tarPos">    the position of target tokenObj </param>
		/// <param name="nonSpaceTokenList"> token list without space tokens </param>
		/// <param name="cSpellApi"> for all dictioanry and Word2Vec data </param>
		/// <param name="debugFlag"> boolean flag for debug print
		/// </param>
		/// <returns>    the corrected merged word in MergeObj if the target token 
		///             matches real-word merged rules. 
		///             Otherwise, a null of MergeObj is returned. </returns>
		// return the original term if no good correctin are found
		public static MergeObj GetCorrectTerm(int tarPos, List<TokenObj> nonSpaceTokenList, CSpellApi cSpellApi, bool debugFlag) {
			// get tarWord from tarTokenObj and init outTokenObj
			TokenObj tarTokenObj = nonSpaceTokenList[tarPos];
			string tarWord = tarTokenObj.GetTokenStr();
			// 1. only remove ending punctuation for coreTerm
			// No coreStr is used for real-word merge for less aggressive
			//String coreStr = TermUtil.StripEndPuncSpace(tarWord).toLowerCase();
			// 2. real-word merge correction 
			// check if tarWord and removeEndPuncStr is OOV
			MergeObj outMergeObj = null; // no merge if it is null
			if ((tarTokenObj.GetProcHist().Count == 0) && (RealWordMergeDetector.IsDetect(tarWord, cSpellApi, debugFlag) == true)) {
				cSpellApi.UpdateDetectNo();
				// TBD, should take care of possessive xxx's here
				// 3. get candidates from merge
				// set mergeWithHypehn to false for real-word merge
				HashSet<MergeObj> mergeSet = RealWordMergeCandidates.GetCandidates(tarPos, nonSpaceTokenList, cSpellApi);
				// 4. Ranking: get top ranked candidates as corrected terms
				// 4.1 just use frenquency or context, no orthoGraphic
				// in case of using context
				// need the context & frequency score for the orgMergeTerm
				outMergeObj = RankRealWordMergeByMode.GetTopRankMergeObj(mergeSet, cSpellApi, debugFlag, tarPos, nonSpaceTokenList);
			}
			return outMergeObj;
		}
		private static void TestGetCorrectTerm(CSpellApi cSpellApi) {
			// init
			// all lowerCase
			//String inText = "Dur ing my absent.";
			string inText = "He was diagnosed early on set dementia 3 year ago.";
			bool debugFlag = false;
			List<TokenObj> inTokenList = TextObj.TextToTokenList(inText);
			// 1. convert to the non-empty token list
			List<TokenObj> nonSpaceTokenList = TextObj.GetNonSpaceTokenObjList(inTokenList);
			// result: tar = on    
			int tarPos = 4;
			MergeObj mergeObj = GetCorrectTerm(tarPos, nonSpaceTokenList, cSpellApi, debugFlag);
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
				Console.WriteLine("Usage: java RealWordMergeCorrector <configFile>");
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
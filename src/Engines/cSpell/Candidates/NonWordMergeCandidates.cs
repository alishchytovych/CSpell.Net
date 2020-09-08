using System;
using System.Collections.Generic;
using System.Linq;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Extensions;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Candidates {

	/// <summary>
	///***************************************************************************
	/// This class generates non-word merge candidates. 
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
	public class NonWordMergeCandidates {
		/// <summary>
		/// Private constructor 
		/// </summary>
		private NonWordMergeCandidates() { }
		// mergeNo = 1, only merge to the right or to the left
		// mergerNo = 2, merge to the right x2, to the left x2, 
		// or to the rightx1 and to the left x1
		// inTextList: No empty space token
		public static HashSet<string> GetCandidateStrs(int tarPos, List<TokenObj> inTextList, CSpellApi cSpellApi) {
			HashSet<MergeObj> mergeSet = GetCandidates(tarPos, inTextList, cSpellApi);
			// convert from MergeObj to Str    
			HashSet<string> mergeStrSet = new HashSet<string>(mergeSet.Select(mObj => mObj.GetMergeWord()).ToList());

			return mergeStrSet;
		}
		public static HashSet<MergeObj> GetCandidates(int tarPos, List<TokenObj> nonSpaceTextList, CSpellApi cSpellApi) {
			// 0. get vars from cSpellApi
			RootDictionary suggestDic = cSpellApi.GetSuggestDic();
			RootDictionary aADic = cSpellApi.GetAaDic();
			RootDictionary mwDic = cSpellApi.GetMwDic();
			// get all merge candidates, recursively
			HashSet<MergeObj> mergeSet = new HashSet<MergeObj>();
			int maxMergeNo = cSpellApi.GetCanNwMaxMergeNo();
			// default: non-word includes merge with hyphen
			bool mergeWithHyphen = cSpellApi.GetCanNwMergeWithHyphen();
			// allow short word merge: a m => am
			bool shortWordMerge = true;
			for (int mergeNo = 1; mergeNo <= maxMergeNo; mergeNo++) {
				HashSet<MergeObj> curMergeSet = CandidatesUtilMerge.GetMergeSetByMergeNo(tarPos, nonSpaceTextList, mergeNo, mergeWithHyphen, shortWordMerge, suggestDic, aADic, mwDic);
				mergeSet.addAll(curMergeSet);
			}
			return mergeSet;
		}
		// private methods
		private static void Test() {
			// init cSpellApi
			string configFile = "../data/Config/cSpell.properties";
			CSpellApi cSpellApi = new CSpellApi(configFile);
			Console.WriteLine("===== Unit Test of MergeCandidates =====");
			//String inText = "He was dia gnosed  early onset deminita 3 year ago.";
			// example from 73.txt
			//String inText = "I have seven live births with no problems dur ing my pregnancies. That is a dis appoint ment";
			string inText = "That is a disa ppoint ment.";
			List<TokenObj> inTextList = TextObj.TextToTokenList(inText);
			string inStr = String.Join("|", inTextList.Select(obj => obj.GetTokenStr()));
			Console.WriteLine(" - inTextList (" + inTextList.Count + "): [" + inStr + "]");
			Console.WriteLine("-------------------------");
			foreach (TokenObj tokenObj in inTextList) {
				Console.WriteLine(tokenObj.ToString());
			}
			int tarPos = 4;
			Console.WriteLine("-------------------------");
			Console.WriteLine("- tarPos: " + tarPos);
			Console.WriteLine("- maxMergeNo: " + cSpellApi.GetCanNwMaxMergeNo());
			Console.WriteLine("------ merge set -------");
			// pre-Process: convert to the non-empty token list
			List<TokenObj> nonSpaceTextList = TextObj.GetNonSpaceTokenObjList(inTextList);
			// get the candidate for a specified target position
			HashSet<MergeObj> mergeSet = GetCandidates(tarPos, nonSpaceTextList, cSpellApi);

			// print out
			foreach (MergeObj mergeObj in mergeSet) {
				Console.WriteLine(mergeObj.ToString());
			}
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java NonWordMergeCandidates");
				Environment.Exit(0);
			}

			// test case and print out 
			Test();
		}
		// data member
	}

}
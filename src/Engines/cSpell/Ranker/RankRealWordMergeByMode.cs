using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Candidates;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Extensions;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///************************************************************************** 
	/// This class ranks and finds the best candidates for real-word merge by 
	/// specifying different ranking methods.
	/// 
	/// <para><b>History:</b>
	/// <ul>
	/// <li>2018 baseline
	/// </ul>
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ***************************************************************************
	/// </para>
	/// </summary>
	public class RankRealWordMergeByMode {
		// private constructor
		private RankRealWordMergeByMode() { }
		// public method
		public static MergeObj GetTopRankMergeObj(HashSet<MergeObj> candidates, CSpellApi cSpellApi, bool debugFlag, int tarPos, List<TokenObj> nonSpaceTokenList) {
			// only have time to test Word2Vec score, yet to test other scores
			MergeObj mergeObj = GetTopRankMergeObjByContext(candidates, cSpellApi, debugFlag, tarPos, nonSpaceTokenList);
			/*
			// use frequency score for merge
			MergeObj mergeObj = GetTopRankMergeObjByFrequency(candidates, 
			    cSpellApi, debugFlag, tarPos, nonSpaceTokenList);
			// use context score for merge
			MergeObj mergeObj = GetTopRankMergeObjByContext(candidates, 
			    cSpellApi, debugFlag, tarPos, nonSpaceTokenList);
			// use combination
			MergeObj mergeObj = GetTopRankMergeObjByCSpell(candidates, 
			    cSpellApi, debugFlag, tarPos, nonSpaceTokenList);
			*/
			return mergeObj;
		}
		// cSpell
		private static MergeObj GetTopRankMergeObjByCSpell(HashSet<MergeObj> candidates, CSpellApi cSpellApi, bool debugFlag, int tarPos, List<TokenObj> nonSpaceTokenList) {
			// use context first for higher accuracy
			MergeObj topRankMergeObj = GetTopRankMergeObjByContext(candidates, cSpellApi, debugFlag, tarPos, nonSpaceTokenList);

			// then use frequency for more recall
			if (topRankMergeObj == null) {
				topRankMergeObj = GetTopRankMergeObjByFrequency(candidates, cSpellApi, debugFlag, tarPos, nonSpaceTokenList);
			}
			return topRankMergeObj;
		}
		// use context score
		private static MergeObj GetTopRankMergeObjByContext(HashSet<MergeObj> candidates, CSpellApi cSpellApi, bool debugFlag, int tarPos, List<TokenObj> nonSpaceTokenList) {
			// init
			Word2Vec word2VecIm = cSpellApi.GetWord2VecIm();
			Word2Vec word2VecOm = cSpellApi.GetWord2VecOm();
			int contextRadius = cSpellApi.GetRwMergeContextRadius();
			bool word2VecSkipWord = cSpellApi.GetWord2VecSkipWord();
			int maxCandNo = cSpellApi.GetCanMaxCandNo();
			double rwMergeCFactor = cSpellApi.GetRankRwMergeCFac();
			MergeObj topRankMergeObj = RankRealWordMergeByContext.GetTopRankMergeObj(candidates, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, rwMergeCFactor, debugFlag);
			return topRankMergeObj;
		}
		// return the best ranked str from candidates using orthographic score
		// tarPos: start from 0, not include empty space token
		private static MergeObj GetTopRankMergeObjByFrequency(HashSet<MergeObj> candidates, CSpellApi cSpellApi, bool debugFlag, int tarPos, List<TokenObj> nonSpaceTokenList) {
			// init
			WordWcMap wordWcMap = cSpellApi.GetWordWcMap();
			int maxCandNo = cSpellApi.GetCanMaxCandNo();
			MergeObj topRankMergeObj = null;
			// get the top rank mergeObj by frequency
			if (candidates.Count > 0) {
				// 1. convert mergeObj set to string set
				// key: coreMergeWord, MergeObj
				Dictionary<string, MergeObj> candStrMergeObjMap = new Dictionary<string, MergeObj>();
				foreach (MergeObj mergeObj in candidates) {
					string mergeWord = mergeObj.GetCoreMergeWord();
					candStrMergeObjMap[mergeWord] = mergeObj;
				}
				HashSet<string> candStrSet = new HashSet<string>(candStrMergeObjMap.Keys);
				// 2. find the top rank by Str
				string topRankStr = RankByFrequency.GetTopRankStr(candStrSet, wordWcMap);
				// 3. convert back from top rank str to MergeObj    
				// topRankStr should never be null because candidates is > 0
				if (!string.ReferenceEquals(topRankStr, null)) {
					topRankMergeObj = candStrMergeObjMap.GetValueOrNull(topRankStr);
				}
				// 4. print out frequency score detail
				ScoreDetailByMode.PrintFrequencyScore(candStrSet, wordWcMap, maxCandNo, debugFlag);
			}
			return topRankMergeObj;
		}

		// private method
		private static void Test(string srcStr, string tarStr) {
			/*
			OrthographicScore os = new OrthographicScore(srcStr, tarStr);
			System.out.println(os.ToString());        
			*/
		}
		private static void Tests() {
			// for merge
			/*
			Test("dicti onary", "dict unary");
			Test("dicti onary", "dictionary");
			Test("diction ary", "diction arry");
			Test("diction ary", "dictionary");
			*/
		}
		// test Driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java RankMergeByMode");
				Environment.Exit(0);
			}
			// test
			Tests();
		}
		// data member
	}

}
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
	/// This class ranks and finds the best candidate for non-word merge by 
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
	public class RankNonWordMergeByMode {
		// private constructor
		private RankNonWordMergeByMode() { }
		// public method
		public static MergeObj GetTopRankMergeObj(HashSet<MergeObj> candidates, CSpellApi cSpellApi, int tarPos, List<TokenObj> nonSpaceTokenList, bool debugFlag) {
			/*
			// use frequency score for merge
			MergeObj mergeObj = GetTopRankMergeObjByFrequency(candidates, 
			    cSpellApi, debugFlag, tarPos, nonSpaceTokenList);
			// use context score for merge
			MergeObj mergeObj = GetTopRankMergeObjByContext(candidates, 
			    cSpellApi, debugFlag, tarPos, nonSpaceTokenList);
			*/
			// use combination
			MergeObj mergeObj = GetTopRankMergeObjByCSpell(candidates, cSpellApi, tarPos, nonSpaceTokenList, debugFlag);
			return mergeObj;
		}
		// cSpell
		private static MergeObj GetTopRankMergeObjByCSpell(HashSet<MergeObj> candidates, CSpellApi cSpellApi, int tarPos, List<TokenObj> nonSpaceTokenList, bool debugFlag) {
			// use context first for higher accuracy
			MergeObj topRankMergeObj = GetTopRankMergeObjByContext(candidates, cSpellApi, tarPos, nonSpaceTokenList, debugFlag);

			// then use frequency for more recall
			if (topRankMergeObj == null) {
				topRankMergeObj = GetTopRankMergeObjByFrequency(candidates, cSpellApi, tarPos, nonSpaceTokenList, debugFlag);
			}
			return topRankMergeObj;
		}
		// use context score
		private static MergeObj GetTopRankMergeObjByContext(HashSet<MergeObj> candidates, CSpellApi cSpellApi, int tarPos, List<TokenObj> nonSpaceTokenList, bool debugFlag) {
			// init
			Word2Vec word2VecIm = cSpellApi.GetWord2VecIm();
			Word2Vec word2VecOm = cSpellApi.GetWord2VecOm();
			int contextRadius = cSpellApi.GetNwMergeContextRadius();
			bool word2VecSkipWord = cSpellApi.GetWord2VecSkipWord();
			int maxCandNo = cSpellApi.GetCanMaxCandNo();
			MergeObj topRankMergeObj = RankNonWordMergeByContext.GetTopRankMergeObj(candidates, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, debugFlag);
			return topRankMergeObj;
		}
		// return the best ranked str from candidates using orthographic score
		// tarPos: start from 0, not include empty space token
		private static MergeObj GetTopRankMergeObjByFrequency(HashSet<MergeObj> candidates, CSpellApi cSpellApi, int tarPos, List<TokenObj> nonSpaceTokenList, bool debugFlag) {
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

		// return candidate str list sorted by score, higher first
		/// <summary>
		/// public static ArrayList<String> GetCandidateStrList(String inStr,
		///        HashSet<String> candidates, int rankMode)
		/// {
		///    ArrayList<String> candStrList = new ArrayList<String>();
		///    switch(rankMode)
		///    {
		///        case CSpellApi.RANK_MODE_ORTHOGRAPHIC:
		///            candStrList = RankByOrthographic.GetCandidateStrList(inStr,
		///                candidates);
		///            break;
		///        case CSpellApi.RANK_MODE_FREQUENCY:
		///            break;
		///        case CSpellApi.RANK_MODE_CONTEXT:
		///            break;
		///        case CSpellApi.RANK_MODE_NOISY_CHANNEL:
		///            break;
		///        case CSpellApi.RANK_MODE_ENSEMBLE:
		///            break;
		///        case CSpellApi.RANK_MODE_CSPELL:
		///            break;
		///    }
		///    return candStrList;    
		/// }
		/// 
		/// </summary>
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
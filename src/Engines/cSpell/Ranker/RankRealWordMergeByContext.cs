using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Candidates;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;
using SpellChecker.Engines.cSpell.Extensions;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///***************************************************************************
	/// This class ranks and finds the best ranked candidates for real-word merge
	/// by ContextSocre.
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
	public class RankRealWordMergeByContext {
		// private constructor
		private RankRealWordMergeByContext() { }
		// return the best ranked str from candidates using word2Vec score
		// inTokenList, includes space token, is not coreTerm.Lc
		// return null if no candidate is found to correct
		public static MergeObj GetTopRankMergeObj(HashSet<MergeObj> candidates, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, double rwMergeFactor, bool debugFlag) {
			// init the topRankMergeObj
			MergeObj topRankMergeObj = null;
			if (candidates.Count > 0) {
				// 1. find sorted score list for each candidates ...
				List<ContextScore> candScoreList = GetCandidateScoreList(candidates, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, debugFlag);
				// 2. find the top ranked str
				// the 0 element has the highest score because it is sorted        
				// only 1 candidate, use it for nonWord
				ContextScore topContextScore = null;
				if (candScoreList.Count > 0) {
					topContextScore = candScoreList[0];
				}
				// 3. find the mergeObj from the topRankStr (if exist)
				if (topContextScore != null) {
					// 3.1. convert mergeObj set to string set
					// key: coreMergeWord, MergeObj
					Dictionary<string, MergeObj> candStrMergeObjMap = new Dictionary<string, MergeObj>();
					foreach (MergeObj mergeObj in candidates) {
						string mergeWord = mergeObj.GetCoreMergeWord();
						candStrMergeObjMap[mergeWord] = mergeObj;
					}
					HashSet<string> andStrSet = new HashSet<string>(candStrMergeObjMap.Keys);
					// 3.2 convert back from top rank str to MergeObj
					// topRankStr should never be null because candidates is > 0
					string topRankStr = topContextScore.GetTerm();
					topRankMergeObj = candStrMergeObjMap.GetValueOrNull(topRankStr);
					// 4. compare the top rank merge to the original string b4 merge
					// 1. get the word2Vec score for the orgMergeTerm b4 merge
					// 1.1 wordVec for context
					int tarPos = topRankMergeObj.GetStartPos();
					// tarSize is the total token No of the orgMergeWords
					int tarSize = topRankMergeObj.GetEndPos() - topRankMergeObj.GetStartPos() + 1;
					DoubleVec contextVec = Word2VecContext.GetContextVec(tarPos, tarSize, nonSpaceTokenList, word2VecIm, contextRadius, word2VecSkipWord, debugFlag);
					// 1.2 wordVec for the original words before merge     
					string orgMergeWord = topRankMergeObj.GetOrgMergeWord();
					ContextScore orgContextScore = new ContextScore(orgMergeWord, contextVec, word2VecOm);
					// validate top merge candidate, set to null if false    
					if (IsTopCandValid(orgContextScore, topContextScore, rwMergeFactor, debugFlag) == false) {
						// set to null if score is not good enough for corection
						topRankMergeObj = null;
					}
				}
			}
			return topRankMergeObj;
		}
		// check score rule for real-word merge correctionrrayList<TokenObj>
		// nonSpaceTokenList,
		private static bool IsTopCandValid(ContextScore orgContextScore, ContextScore topContextScore, double rwMergeFactor, bool debugFlag) {
			// Score rules for merge
			double orgScore = orgContextScore.GetScore();
			double topScore = topContextScore.GetScore();
			bool flag = false;
			// 2.1 no merge correction if orgScore is 0.0d, no word2Vec information
			if (orgScore < 0.0d) {
				// 2.2a merge if the org score is negative and top score is positive
				if (topScore > 0.0d) {
					flag = true;
				}
				// 2.2b merge if the org score is negative and top score is better
				// this is needed for higher recall and F1
				else if ((topScore < 0.0d) && (topScore > orgScore * rwMergeFactor)) {
					flag = true;
				}
			} else if (orgScore > 0.0d) {
				// 2.3a merge if the org score is positive and better 0.01*topScore
				if (topScore * rwMergeFactor > orgScore) {
					flag = true;
				}
			}
			return flag;
		}
		// return candidate scoreObj list sorted by score, higher first
		public static List<ContextScore> GetCandidateScoreList(HashSet<MergeObj> candidates, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, bool debugFlag) {
			// find score object set for each candidates ...
			HashSet<ContextScore> candScoreSet = GetCandidateScoreSet(candidates, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, debugFlag);
			// sorted by the score, higher go first
			List<ContextScore> candScoreList = new List<ContextScore>(candScoreSet);
			ContextScoreComparator<ContextScore> csc = new ContextScoreComparator<ContextScore>();
			candScoreList.Sort(csc);
			// print detail
			foreach (ContextScore contextScore in candScoreList) {
				DebugPrint.PrintCScore(contextScore.ToString(), debugFlag);
			}
			return candScoreList;
		}
		// return candidate set with context score
		// word2Vec is the word|wordVec map to get the wordVec 
		// Not sorted, because it is a set
		// tarPos: starting position of target token
		// tarSize: token size of target token (single word = 1)
		public static HashSet<ContextScore> GetCandidateScoreSet(HashSet<MergeObj> candidates, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, bool debugFlag) {
			HashSet<ContextScore> candScoreSet = new HashSet<ContextScore>();
			// get context score for all candidates
			// go through all merge candidates, all have differetn context  
			foreach (MergeObj mergeObj in candidates) {
				// 1. get the context and contextVec, using input matrix
				int tarPos = mergeObj.GetStartPos();
				int tarSize = mergeObj.GetEndPos() - mergeObj.GetStartPos() + 1;
				DoubleVec contextVec = Word2VecContext.GetContextVec(tarPos, tarSize, nonSpaceTokenList, word2VecIm, contextRadius, word2VecSkipWord, debugFlag);
				// 2. get ContextSocre for each merge, use output matrix
				string mergeWord = mergeObj.GetCoreMergeWord();
				ContextScore cs = new ContextScore(mergeWord, contextVec, word2VecOm);
				candScoreSet.Add(cs);
			}
			return candScoreSet;
		}
		// return the best ranked str from candidates using context score
		// this method is replaced by GetTopRankStr, which sorted by comparator
		public static MergeObj GetTopRankMergeObjByScore(HashSet<MergeObj> candidates, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, bool debugFlag) {
			MergeObj topRankMergeObj = null;
			double maxScore = 0.0d;
			foreach (MergeObj mergeObj in candidates) {
				// 1. get the context and contextVec
				int tarPos = mergeObj.GetStartPos();
				int tarSize = mergeObj.GetEndPos() - mergeObj.GetStartPos() + 1;
				DoubleVec contextVec = Word2VecContext.GetContextVec(tarPos, tarSize, nonSpaceTokenList, word2VecIm, contextRadius, word2VecSkipWord, debugFlag);
				// 2. get ContextSocre for each merge, use output matrix
				string mergeWord = mergeObj.GetCoreMergeWord();
				ContextScore cs = new ContextScore(mergeWord, contextVec, word2VecOm);
				double score = cs.GetScore();
				// update only if the score is > 0.0d
				if (score > maxScore) {
					topRankMergeObj = mergeObj;
					maxScore = score;
				}
			}
			return topRankMergeObj;
		}
		// private methods
		// test Driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java RankRealWordMergeByContext");
				Environment.Exit(0);
			}
			// test
		}
		// data member
	}

}
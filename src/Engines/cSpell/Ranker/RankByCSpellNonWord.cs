using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Candidates;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///***************************************************************************
	/// This class ranks and finds the best ranked candidates for non-word correction
	/// by CSpell score system (playoffs).
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
	public class RankByCSpellNonWord {
		// private constructor
		private RankByCSpellNonWord() { }
		// return candidate str list sorted by wordNo score, higher first
		public static List<string> GetCandidateStrList(string wordStr, HashSet<string> candidates, WordWcMap wordWcMap, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, int compareMode, double wf1, double wf2, double wf3, bool debugFlag) {
			List<CSpellScore> candScoreList = GetCandidateScoreList(wordStr, candidates, wordWcMap, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, compareMode, wf1, wf2, wf3, debugFlag);
			List<string> candStrList = new List<string>();
			foreach (CSpellScore cs in candScoreList) {
				candStrList.Add(cs.GetCandStr());
			}
			return candStrList;
		}
		// return candidate scoreObj list sorted by score, higher first
		public static List<CSpellScore> GetCandidateScoreList(string wordStr, HashSet<string> candidates, WordWcMap wordWcMap, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, int compareMode, double wf1, double wf2, double wf3, bool debugFlag) {
			HashSet<CSpellScore> candScoreSet = GetCandidateScoreSet(wordStr, candidates, wordWcMap, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, wf1, wf2, wf3, debugFlag);
			List<CSpellScore> candScoreList = new List<CSpellScore>(candScoreSet);
			// sort the set to list by context
			CSpellScoreNw1To1Comparator<CSpellScore> csc = new CSpellScoreNw1To1Comparator<CSpellScore>();
			csc.SetCompareMode(compareMode);
			candScoreList.Sort(csc);
			return candScoreList;
		}
		// return candidate set with cSpell score
		// wordStr is the srcTxt used to calculate the score between it and cand
		public static HashSet<CSpellScore> GetCandidateScoreSet(string wordStr, HashSet<string> candidates, WordWcMap wordWcMap, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, double wf1, double wf2, double wf3, bool debugFlag) {
			HashSet<CSpellScore> candScoreSet = new HashSet<CSpellScore>();
			foreach (string cand in candidates) {
				// find context for each candidates
				DoubleVec contextVec = Word2VecContext.GetContextVec(tarPos, tarSize, nonSpaceTokenList, word2VecIm, contextRadius, word2VecSkipWord, debugFlag);

				CSpellScore cs = new CSpellScore(wordStr, cand, wordWcMap, contextVec, word2VecOm, wf1, wf2, wf3);
				candScoreSet.Add(cs);
			}
			return candScoreSet;
		}
		// return the best ranked str from candidates using cSpell score
		public static string GetTopRankStr(string inStr, HashSet<string> candidates, WordWcMap wordWcMap, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, double rangeFactor, double nwS1MinOScore, double wf1, double wf2, double wf3) {
			bool debugFlag = false;
			return GetTopRankStr(inStr, candidates, wordWcMap, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, rangeFactor, nwS1MinOScore, wf1, wf2, wf3, debugFlag);
		}
		// 2-stage ranking
		public static string GetTopRankStr(string inStr, HashSet<string> candidates, WordWcMap wordWcMap, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, double rangeFactor, double nwS1MinOScore, double wf1, double wf2, double wf3, bool debugFlag) {
			string topRankStr = inStr;
			// 1. find the top orthograpics score from sorted list by orthographics
			// need the sorted list to find the top orthographics score
			int compareMode = CSpellScoreNw1To1Comparator<int>.COMPARE_BY_ORTHOGRAPHICS;
			// previous way
			//int compareMode = CSpellScoreNw1To1Comparator.COMPARE_BY_COMBO;
			List<CSpellScore> candScoreList = GetCandidateScoreList(inStr, candidates, wordWcMap, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, compareMode, wf1, wf2, wf3, debugFlag);
			// 2. Set a range for the candidates to find all possible top rank
			// use the highest context and frequecny score to final rank.
			double maxFScore = 0.0d;
			double maxCScore = 0.0d; // not -1.0d
			double maxNScore = 0.0d;
			string topRankStrByC = null;
			string topRankStrByF = null;
			string topRankStrByN = null;
			if (candScoreList.Count >= 1) {
				double topOScore = candScoreList[0].GetOScore().GetScore();
				double range = topOScore * rangeFactor;
				// 2.1 find all cands within range
				foreach (CSpellScore candScore in candScoreList) {
					// within the range, find the highest frequency
					if ((topOScore - candScore.GetOScore().GetScore()) <= range) {
						double candCScore = candScore.GetCScore().GetScore();
						if ((candCScore != 0.0d) && (candCScore > maxCScore)) {
							// find the topRank by context
							topRankStrByC = candScore.GetCandStr();
							maxCScore = candCScore;
						}
						double candFScore = candScore.GetFScore().GetScore();
						if (candFScore > maxFScore) {
							// find the topRank by frequency
							topRankStrByF = candScore.GetCandStr();
							maxFScore = candFScore;
						}
						double candNScore = candScore.GetNScore().GetScore();
						if (candNScore > maxNScore) {
							// find the topRank by Noisy Channel
							topRankStrByN = candScore.GetCandStr();
							maxNScore = candNScore;
						}
					}
				}
				// 3. set topRankStr to context
				if (!string.ReferenceEquals(topRankStrByC, null)) {
					topRankStr = topRankStrByC;
				}
				// 4. set topRankStr to Noisy Channel
				else if (!string.ReferenceEquals(topRankStrByN, null)) {
					topRankStr = topRankStrByN;
				}
				// 5. 1 candidate, use orthographic
				else if (candScoreList.Count == 1) {
					// "Lactoccocus lactis" to "Lactococcus lactis"
					if (topOScore >= nwS1MinOScore) { // empirical value
						topRankStr = candScoreList[0].GetOScore().GetTarStr();
					}
				}
				/// <summary>
				///* no need, used for dev and testing
				/// // 6. set topRankStr to frequency
				/// else if(topRankStrByF != null)
				/// {
				///    topRankStr = topRankStrByF;
				/// }
				/// // 7. set topRankStr to Orthographics
				/// else
				/// {
				///    topRankStr = candScoreList.get(0).GetCandStr();
				/// }
				/// </summary>
			}
			return topRankStr;
		}
		// private methods
		// test Driver
		public static void MainTest(string[] args) { }
		// data member
	}

}
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
	/// This class ranks and finds the best ranked candidates for real-word 1To1
	/// correction by CSpell scoring system.
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
	public class RankByCSpellRealWord1To1 {
		// private constructor
		private RankByCSpellRealWord1To1() { }
		// return candidate str list sorted by wordNo score, higher first
		public static List<string> GetCandidateStrList(string wordStr, HashSet<string> candidates, WordWcMap wordWcMap, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, double wf1, double wf2, double wf3, bool debugFlag) {
			List<CSpellScore> candScoreList = GetCandidateScoreList(wordStr, candidates, wordWcMap, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, wf1, wf2, wf3, debugFlag);
			List<string> candStrList = new List<string>();
			foreach (CSpellScore cs in candScoreList) {
				candStrList.Add(cs.GetCandStr());
			}
			return candStrList;
		}
		// return candidate scoreObj list sorted by score, higher first
		public static List<CSpellScore> GetCandidateScoreList(string wordStr, HashSet<string> candidates, WordWcMap wordWcMap, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, double wf1, double wf2, double wf3, bool debugFlag) {
			HashSet<CSpellScore> candScoreSet = GetCandidateScoreSet(wordStr, candidates, wordWcMap, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, wf1, wf2, wf3, debugFlag);
			List<CSpellScore> candScoreList = new List<CSpellScore>(candScoreSet);
			// sort the set to list
			CSpellScoreRw1To1Comparator<CSpellScore> csc = new CSpellScoreRw1To1Comparator<CSpellScore>();
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
		public static string GetTopRankStr(string inStr, HashSet<string> candidates, WordWcMap wordWcMap, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, double wf1, double wf2, double wf3) {
			bool debugFlag = false;
			return GetTopRankStr(inStr, candidates, wordWcMap, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, wf1, wf2, wf3, debugFlag);
		}
		public static string GetTopRankStr(string inStr, HashSet<string> candidates, WordWcMap wordWcMap, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, double wf1, double wf2, double wf3, bool debugFlag) {
			string topRankStr = inStr;
			// get the sorted list
			List<CSpellScore> candScoreList = GetCandidateScoreList(inStr, candidates, wordWcMap, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, wf1, wf2, wf3, debugFlag);
			if (candScoreList.Count > 0) {
				topRankStr = candScoreList[0].GetCandStr();
			}
			return topRankStr;
		}
		// private methods
		// test Driver
		public static void MainTest(string[] args) { }
		// data member
	}

}
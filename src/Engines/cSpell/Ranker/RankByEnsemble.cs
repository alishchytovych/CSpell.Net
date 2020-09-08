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
	/// This class ranks and finds the best ranked candidates by ensemble method.
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
	public class RankByEnsemble {
		// private constructor
		private RankByEnsemble() { }
		// return the best ranked str from candidates using cSpell score
		public static string GetTopRankStr(string inStr, HashSet<string> candidates, WordWcMap wordWcMap, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, double rangeFactor, double wf1, double wf2, double wf3) {
			bool debugFlag = false;
			return GetTopRankStr(inStr, candidates, wordWcMap, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, rangeFactor, wf1, wf2, wf3, debugFlag);
		}
		public static string GetTopRankStr(string inStr, HashSet<string> candidates, WordWcMap wordWcMap, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, double rangeFactor, double wf1, double wf2, double wf3, bool debugFlag) {
			string topRankStr = inStr;
			// get the sorted list
			int compareMode = CSpellScoreNw1To1Comparator<int>.COMPARE_BY_ENSEMBLE;
			List<CSpellScore> candScoreList = RankByCSpellNonWord.GetCandidateScoreList(inStr, candidates, wordWcMap, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, compareMode, wf1, wf2, wf3, debugFlag);
			// Set a range for the candidates to find all possible top rank
			// use the highest context and frequecny score to final rank.
			if (candScoreList.Count >= 0) {
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
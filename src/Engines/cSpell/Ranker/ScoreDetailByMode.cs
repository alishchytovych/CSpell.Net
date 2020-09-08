using System.Collections.Generic;
using System.Linq;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Ranker {
	/// <summary>
	///************************************************************************** 
	/// This class provides detail of scores for different ranking methods.
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
	public class ScoreDetailByMode {
		// private constructor
		private ScoreDetailByMode() { }
		// TBD, this file should be deleted by moving each method to 
		// the assocaited ranking class
		// public method
		public static void PrintContextScore(HashSet<string> candSet, int tarPos, int tarSize, List<TokenObj> inTextList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, int maxCandNo, bool debugFlag) {
			if (debugFlag == true) {
				ContextScoreComparator<ContextScore> csc = new ContextScoreComparator<ContextScore>();
				HashSet<ContextScore> cScoreSet = RankByContext.GetCandidateScoreSet(candSet, tarPos, tarSize, inTextList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, debugFlag);
				var list = cScoreSet.OrderBy(x => x, csc).Take(maxCandNo).Select(obj => obj.ToString()).ToList();
				foreach (var item in list)
					DebugPrint.PrintCScore(item, debugFlag);
			}
		}
		public static void PrintFrequencyScore(HashSet<string> candSet, WordWcMap wordWcMap, int maxCandNo, bool debugFlag) {
			if (debugFlag == true) {
				FrequencyScoreComparator<FrequencyScore> fsc = new FrequencyScoreComparator<FrequencyScore>();
				HashSet<FrequencyScore> fScoreSet = RankByFrequency.GetCandidateScoreSet(candSet, wordWcMap);
				var list = fScoreSet.OrderBy(x => x, fsc).Take(maxCandNo).Select(obj => obj.ToString()).ToList();
				foreach (var item in list)
					DebugPrint.PrintFScore(item, debugFlag);
			}
		}
		public static void PrintOrthographicScore(string inStr, HashSet<string> candSet, int maxCandNo, double wf1, double wf2, double wf3, bool debugFlag) {
			if (debugFlag == true) {
				OrthographicScoreComparator<OrthographicScore> osc = new OrthographicScoreComparator<OrthographicScore>();
				HashSet<OrthographicScore> oScoreSet = RankByOrthographic.GetCandidateScoreSet(inStr, candSet, wf1, wf2, wf3);
				var list = oScoreSet.OrderBy(x => x, osc).Take(maxCandNo).Select(obj => obj.ToString()).ToList();
				foreach (var item in list)
					DebugPrint.PrintOScore(item, debugFlag);
			}
		}
		public static void PrintNoisyChannelScore(string inStr, HashSet<string> candSet, WordWcMap wordWcMap, int maxCandNo, double wf1, double wf2, double wf3, bool debugFlag) {
			if (debugFlag == true) {
				NoisyChannelScoreComparator<NoisyChannelScore> ncsc = new NoisyChannelScoreComparator<NoisyChannelScore>();
				HashSet<NoisyChannelScore> ncScoreSet = RankByNoisyChannel.GetCandidateScoreSet(inStr, candSet, wordWcMap, wf1, wf2, wf3);
				var list = ncScoreSet.OrderBy(x => x, ncsc).Take(maxCandNo).Select(obj => obj.ToString()).ToList();
				foreach (var item in list)
					DebugPrint.PrintNScore(item, debugFlag);
			}
		}
		// this detail does not print how cSpell really fidn the top rank
		// it is sorted by CSpell score
		// CSpell use the cSpell score + context and frequency to find the top
		public static void PrintCSpellScore(string inStr, HashSet<string> candSet, WordWcMap wordWcMap, int maxCandNo, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, double wf1, double wf2, double wf3, bool debugFlag) {
			if (debugFlag == true) {
				// NW 1To1
				CSpellScoreNw1To1Comparator<CSpellScore> csc = new CSpellScoreNw1To1Comparator<CSpellScore>();
				HashSet<CSpellScore> cScoreSet = RankByCSpellNonWord.GetCandidateScoreSet(inStr, candSet, wordWcMap, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, wf1, wf2, wf3, debugFlag);
				var list = cScoreSet.OrderBy(x => x, csc).Take(maxCandNo).Select(obj => obj.ToString()).ToList();
				foreach (var item in list)
					DebugPrint.PrintScore(item, debugFlag);
			}
		}
		// private method
		// data member
	}

}
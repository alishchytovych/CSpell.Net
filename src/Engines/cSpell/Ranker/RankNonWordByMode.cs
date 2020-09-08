using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///************************************************************************** 
	/// This class ranks and finds the best candidates for non-word split and 1To1
	/// correction by specifying different ranking methods.
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
	public class RankNonWordByMode {
		// private constructor
		private RankNonWordByMode() { }
		// public method
		// return the best ranked str from candidates using orthographic score
		public static string GetTopRankStr(string inStr, HashSet<string> candidates, CSpellApi cSpellApi) {
			bool debugFlag = false;
			return GetTopRankStr(inStr, candidates, cSpellApi, debugFlag);
		}
		public static string GetTopRankStr(string inStr, HashSet<string> candidates, CSpellApi cSpellApi, bool debugFlag) {
			int pos = -1;
			List<TokenObj> inTokenList = null;
			return GetTopRankStr(inStr, candidates, cSpellApi, debugFlag, pos, inTokenList);
		}
		// tarPos: start from 0, not include empty space token
		public static string GetTopRankStr(string inStr, HashSet<string> candidates, CSpellApi cSpellApi, bool debugFlag, int tarPos, List<TokenObj> nonSpaceTokenList) {
			// init
			int rankMode = cSpellApi.GetRankMode();
			double wf1 = cSpellApi.GetOrthoScoreEdDistFac();
			double wf2 = cSpellApi.GetOrthoScorePhoneticFac();
			double wf3 = cSpellApi.GetOrthoScoreOverlapFac();
			WordWcMap wordWcMap = cSpellApi.GetWordWcMap();
			string topRankStr = inStr;
			int maxCandNo = cSpellApi.GetCanMaxCandNo();
			Word2Vec word2VecIm = cSpellApi.GetWord2VecIm();
			Word2Vec word2VecOm = cSpellApi.GetWord2VecOm();
			int contextRadius = cSpellApi.GetNw1To1ContextRadius();
			bool word2VecSkipWord = cSpellApi.GetWord2VecSkipWord();
			double rangeFactor = cSpellApi.GetRankNwS1RankRangeFac();
			double nwS1MinOScore = cSpellApi.GetRankNwS1MinOScore();
			int tarSize = 1; // only for one-to-one or split, no merge here
			// get the top ranked candidate 
			if (candidates.Count > 0) {
				// get the top rank str by scores
				switch (rankMode) {
					case CSpellApi.RANK_MODE_ORTHOGRAPHIC:
						topRankStr = RankByOrthographic.GetTopRankStr(inStr, candidates, wf1, wf2, wf3);
						ScoreDetailByMode.PrintOrthographicScore(inStr, candidates, maxCandNo, wf1, wf2, wf3, debugFlag);
						break;
					case CSpellApi.RANK_MODE_FREQUENCY:
						topRankStr = RankByFrequency.GetTopRankStr(candidates, wordWcMap);
						ScoreDetailByMode.PrintFrequencyScore(candidates, wordWcMap, maxCandNo, debugFlag);
						break;
					case CSpellApi.RANK_MODE_CONTEXT:
						topRankStr = RankByContext.GetTopRankStr(inStr, candidates, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius);
						ScoreDetailByMode.PrintContextScore(candidates, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, maxCandNo, debugFlag);
						break;
					case CSpellApi.RANK_MODE_NOISY_CHANNEL:
						topRankStr = RankByNoisyChannel.GetTopRankStr(inStr, candidates, wordWcMap, wf1, wf2, wf3);
						ScoreDetailByMode.PrintNoisyChannelScore(inStr, candidates, wordWcMap, maxCandNo, wf1, wf2, wf3, debugFlag);
						break;
					case CSpellApi.RANK_MODE_ENSEMBLE:
						topRankStr = RankByEnsemble.GetTopRankStr(inStr, candidates, wordWcMap, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, rangeFactor, wf1, wf2, wf3);
						// ensemble use same basic socre as CSpell    
						ScoreDetailByMode.PrintCSpellScore(inStr, candidates, wordWcMap, maxCandNo, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, wf1, wf2, wf3, debugFlag);
						break;
					case CSpellApi.RANK_MODE_CSPELL:
						topRankStr = RankByCSpellNonWord.GetTopRankStr(inStr, candidates, wordWcMap, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, rangeFactor, nwS1MinOScore, wf1, wf2, wf3);
						ScoreDetailByMode.PrintCSpellScore(inStr, candidates, wordWcMap, maxCandNo, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, wf1, wf2, wf3, debugFlag);
						break;
				}
			}
			return topRankStr;
		}

		// to be used, not used yet
		// return candidate str list sorted by score, higher first
		public static List<string> GetCandidateStrList(string inStr, HashSet<string> candidates, int rankMode, double wf1, double wf2, double wf3) {
			List<string> candStrList = new List<string>();
			switch (rankMode) {
				case CSpellApi.RANK_MODE_ORTHOGRAPHIC:
					candStrList = RankByOrthographic.GetCandidateStrList(inStr, candidates, wf1, wf2, wf3);
					break;
				case CSpellApi.RANK_MODE_FREQUENCY:
					break;
				case CSpellApi.RANK_MODE_CONTEXT:
					break;
				case CSpellApi.RANK_MODE_NOISY_CHANNEL:
					break;
				case CSpellApi.RANK_MODE_ENSEMBLE:
					break;
				case CSpellApi.RANK_MODE_CSPELL:
					break;
			}
			return candStrList;
		}
		// private method
		private static void Test(string srcStr, string tarStr) {
			double wf1 = 1.00; // weighting factor
			double wf2 = 0.70;
			double wf3 = 0.80;
			OrthographicScore os = new OrthographicScore(srcStr, tarStr, wf1, wf2, wf3);
			Console.WriteLine(os.ToString());
		}
		private static void Tests() {
			Test("spel", "spell");
			Test("spel", "speil");
			Test("spelld", "spell");
			Test("spelld", "spelled");
			// for merge
			Test("dicti onary", "dict unary");
			Test("dicti onary", "dictionary");
			Test("diction ary", "diction arry");
			Test("diction ary", "dictionary");
		}
		// test Driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java OrthographicScore");
				Environment.Exit(0);
			}
			// test
			Tests();
		}
		// data member
		private static int count1_ = 0;
		private static int count2_ = 0;
		private static int count3_ = 0;
	}

}
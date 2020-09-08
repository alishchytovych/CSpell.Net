using System;
using SpellChecker.Engines.cSpell.Lib;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///***************************************************************************
	/// This class provides a java class to get the similarity score for overlap.
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
	public class OverlapScore {
		// private constructor
		private OverlapScore() { }
		public static double GetScore(string srcStr, string tarStr) {
			bool caseFlag = false;
			return GetScore(srcStr, tarStr, caseFlag);
		}
		public static double GetScore(string srcStr, string tarStr, bool caseFlag) {
			double score = 1.0;
			// check case
			string src = srcStr;
			string tar = tarStr;
			if (caseFlag == false) { // not case sensitive
				src = srcStr.ToLower();
				tar = tarStr.ToLower();
			}
			int srcLen = src.Length;
			int tarLen = tar.Length;
			// not the same String
			if (src.Equals(tar) == false) {
				// get maxLength
				int maxLen = Math.Max(srcLen, tarLen);
				// add split penalty
				maxLen += OrthographicUtil.GetSplitPenalty(src, tar);
				// cal leadOverlap
				int minLen = Math.Min(srcLen, tarLen);
				int leadOverlap = 0;
				int ii = 0;
				while ((ii < minLen) && (src[ii] == tar[ii])) {
					leadOverlap++;
					ii++;
				}
				// cal endOverlap
				int trailOverlap = 0;
				int jj = 0;
				while ((jj < minLen) && (src[srcLen - 1 - jj] == tar[tarLen - 1 - jj])) {
					trailOverlap++;
					jj++;
				}
				// if match all charactrs to minLen
				// "123" and "123123" should be 0.55 not 1.0
				// spel should have higher score with spell than speil
				if (leadOverlap == minLen) {
					score = (1.0 * leadOverlap + 0.1 * trailOverlap) / (1.0 * maxLen);
				}
				// spell should have higher score with sspell than nspell
				else if (trailOverlap == minLen) {
					score = (0.1 * leadOverlap + 1.0 * trailOverlap) / (1.0 * maxLen);
				} else {
					score = (1.0 * leadOverlap + 1.0 * trailOverlap) / (1.0 * maxLen);
				}
			}
			// make sure score is between 0.0 ~ 1.0
			score = ((score > 1.0) ? 1.0 : score);
			return score;
		}
		// private methods
		private static void TestScores() {
			Console.WriteLine("srcStr|tarStr|Overlap Score");
			/// <summary>
			/// TestScore("dianosed", "diagnosed");
			/// TestScore("diagnosed", "diagnosed");
			/// TestScore("abcdef", "123456");
			/// TestScore("abadef", "aba1def");
			/// TestScore("aaadef", "aaa123def");
			/// TestScore("bbbdef", "bbb123456def");
			/// TestScore("123456", "123 456");
			/// TestScore("123321", "12 33 21");
			/// TestScore("123 456", "123456");
			/// TestScore("lead", "leadends");    // leadOverLap - minLen
			/// TestScore("ends", "leadends");    // trailOverLap = minLen
			/// TestScore("123", "123123");
			/// TestScore("spel", "spell");    // should have higher score than below
			/// TestScore("spel", "speil");
			/// TestScore("spell", "sspell");    // should have higher score than below
			/// TestScore("spell", "nspell");
			/// TestScore("aaa", "aaaa");
			/// TestScore("baa", "abaa");
			/// TestScore("baa", "bbaa");
			/// TestScore("aaa", "aaa");
			/// TestScore("diction ary", "dictionary");
			/// TestScore("diction ary", "diction arry");
			/// 
			/// </summary>
			TestScore("diagnost", "diagnosis");
			TestScore("diagnost", "diagnosed");
			TestScore("diagnost", "diagnosed");
			TestScore("truely", "truly");
			TestScore("knoledge", "knowledge");
		}
		private static void TestScore(string srcStr, string tarStr) {
			double score = GetScore(srcStr, tarStr);
			Console.WriteLine(srcStr + "|" + tarStr + "|" + string.Format("{0,1:F4}", score));
		}
		// test Driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java OverlapScore");
				Environment.Exit(0);
			}
			// test
			TestScores();
		}
		// data member
	}

}
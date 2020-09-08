using System;
using System.Collections.Generic;
using System.Linq;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///***************************************************************************
	/// This class provides a java object of CSpell score. It includes scores of
	/// orthographic, frequency, noisy channel and context.
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
	public class CSpellScore {
		// private constructor
		public CSpellScore(string wordStr, string candStr, WordWcMap wordWcMap, DoubleVec contextVec, Word2Vec word2Vec, double wf1, double wf2, double wf3) {
			wordStr_ = wordStr;
			candStr_ = candStr;
			// calculate score
			oScore_ = new OrthographicScore(wordStr_, candStr_, wf1, wf2, wf3);
			fScore_ = new FrequencyScore(candStr_, wordWcMap);
			nScore_ = new NoisyChannelScore(wordStr_, candStr_, wordWcMap, wf1, wf2, wf3);
			cScore_ = new ContextScore(candStr_, contextVec, word2Vec);
		}
		// public method
		public virtual double GetScore() {
			return score_;
		}
		public virtual string GetCandStr() {
			return candStr_;
		}
		public virtual OrthographicScore GetOScore() {
			return oScore_;
		}
		public virtual FrequencyScore GetFScore() {
			return fScore_;
		}
		public virtual NoisyChannelScore GetNScore() {
			return nScore_;
		}
		public virtual ContextScore GetCScore() {
			return cScore_;
		}
		public static string GetScoreHeader() {
			string outStr = "NC_Score|wordStr|candStr|O_core|EditDis|Phonetic|overlap|candStr|F_Score|canStr|C_Score";
			return outStr;
		}
		// noisy channel + context
		// where noisyChannel = orthographic score + frequency
		public virtual string ToString() {
			string outStr = ToString(GlobalVars.FS_STR);
			return outStr;
		}
		public virtual string ToString(string fieldSepStr) {
			string outStr = nScore_.ToString(fieldSepStr) + fieldSepStr + cScore_.ToString(fieldSepStr);
			return outStr;
		}
		public static double GetMaxFScore(List<CSpellScore> cSpellScoreList) {
			double maxFScore = cSpellScoreList.Select(c => c.GetFScore().GetScore()).Max(c => c);
			return maxFScore;
		}
		// Edit distance score
		public static double GetMaxEScore(List<CSpellScore> cSpellScoreList) {
			double maxEScore = cSpellScoreList.Select(c => c.GetOScore().GetTokenScore()).Max(c => c);
			return maxEScore;
		}
		// phonetic
		public static double GetMaxPScore(List<CSpellScore> cSpellScoreList) {
			double maxPScore = cSpellScoreList.Select(c => c.GetOScore().GetPhoneticScore()).Max(c => c);
			return maxPScore;
		}
		// overlap
		public static double GetMaxOScore(List<CSpellScore> cSpellScoreList) {
			double maxOScore = cSpellScoreList.Select(c => c.GetOScore().GetOverlapScore()).Max(c => c);
			return maxOScore;
		}
		// single word score, multiwords is 0.0
		// not completed with contextScore
		private static void Test(string wordStr, string candStr, WordWcMap wordWcMap) {
			//CSpellScore cs = new CSpellScore(wordStr, candStr, wordWcMap);
			//System.out.println(cs.ToString());
		}
		// not completed with contextScore
		private static void Tests(WordWcMap wordWcMap, Word2Vec w2vOm) {
			List<string> testStrList = new List<string>();
			Test("spel", "spell", wordWcMap);
			Test("spel", "speil", wordWcMap);
			Test("spelld", "spell", wordWcMap);
			Test("spelld", "spelled", wordWcMap);
		}
		// test Driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java CSpellScore");
				Environment.Exit(0);
			}
			// test
			string inFile = "../data/Frequency/wcWord.data";
			bool verboseFlag = true;
			WordWcMap wordWcMap = new WordWcMap(inFile, verboseFlag);
			string inOmFile = "../data/Context/syn1n.data";
			Word2Vec w2vOm = new Word2Vec(inOmFile, verboseFlag);
			Tests(wordWcMap, w2vOm);
		}
		// data member
		private string wordStr_ = "";
		private string candStr_ = "";
		private double score_ = 0.0; // not used, reserved for future usage
		private OrthographicScore oScore_ = null;
		private FrequencyScore fScore_ = null;
		private NoisyChannelScore nScore_ = null;
		private ContextScore cScore_ = null;
	}

}
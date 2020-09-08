using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;

namespace SpellChecker.Engines.cSpell.Ranker {
	/// <summary>
	///***************************************************************************
	/// This class provides a java object of noisy channel score.
	/// The language model is the frequency WC score, P(w).
	/// The error model is the orthographics score, P(x/w).
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
	public class NoisyChannelScore {
		// private constructor
		public NoisyChannelScore(string wordStr, string candStr, WordWcMap wordWcMap, double wf1, double wf2, double wf3) {
			wordStr_ = wordStr;
			candStr_ = candStr;
			// calculate score
			oScore_ = new OrthographicScore(wordStr_, candStr_, wf1, wf2, wf3);
			fScore_ = new FrequencyScore(candStr_, wordWcMap);
			score_ = oScore_.GetScore() * fScore_.GetScore();
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
		public virtual string ToString() {
			string outStr = ToString(GlobalVars.FS_STR);
			return outStr;
		}
		public virtual string ToString(string fieldSepStr) {
			string outStr = string.Format("{0,1:F8}", score_) + fieldSepStr + oScore_.ToString(fieldSepStr) + fieldSepStr + fScore_.ToString(fieldSepStr);
			return outStr;
		}
		// single word score, multiwords is 0.0
		private static void Test(string wordStr, string candStr, WordWcMap wordWcMap) {
			double wf1 = 1.00;
			double wf2 = 0.70;
			double wf3 = 0.80;
			NoisyChannelScore ncs = new NoisyChannelScore(wordStr, candStr, wordWcMap, wf1, wf2, wf3);
			Console.WriteLine(ncs.ToString());
		}
		private static void Tests(WordWcMap wordWcMap) {
			List<string> testStrList = new List<string>();
			Test("spel", "spell", wordWcMap);
			Test("spel", "speil", wordWcMap);
			Test("spelld", "spell", wordWcMap);
			Test("spelld", "spelled", wordWcMap);
		}
		// test Driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java NosiyChannelScore");
				Environment.Exit(0);
			}
			// test
			string inFile = "../data/Frequency/wcWord.data";
			bool verboseFlag = true;
			WordWcMap wordWcMap = new WordWcMap(inFile, verboseFlag);
			Tests(wordWcMap);
		}
		// data member
		private string wordStr_ = "";
		private string candStr_ = "";
		private double score_ = 0.0;
		private OrthographicScore oScore_ = null;
		private FrequencyScore fScore_ = null;
	}

}
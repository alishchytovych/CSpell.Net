using System;

namespace SpellChecker.Engines.cSpell.Ranker {
	/// <summary>
	///***************************************************************************
	/// This class calculates the token similarity score in orthographic score.
	/// 
	/// <para><b>History:</b>
	/// <ul>
	/// <li>2018 baseline
	/// <li>SCR-1, chlu, 10-15-18, change EditDistanceScore to TokenScore
	/// </ul>
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ***************************************************************************
	/// </para>
	/// </summary>
	public class TokenScore {
		// private constructor
		private TokenScore() { }
		// TBD: read the values from config file
		// get teh similarity score
		public static double GetScore(string srcStr, string tarStr) {
			/// <summary>
			/// Orignal value form Ensemlble
			/// int deleteCost = 95;    // = 0.095
			/// int insertCost = 95;    // = 0.095
			/// int replaceCost = 100;    // = 0.010 sub
			/// int swapCost = 90;        // TBD: should try 98 between delete and replace
			/// 
			/// </summary>
			// new value for better perfroamcne
			int deleteCost = 96; // = 0.095
			int insertCost = 90; // = 0.090
			int replaceCost = 100; // = 0.100
			int swapCost = 94; // TBD: should try 98 between delete and replace
			int caseChangeCost = 10;
			bool enhancedFlag = false; // enhanced algorithm
			int splitCost = insertCost;
			return GetScore(srcStr, tarStr, deleteCost, insertCost, replaceCost, swapCost, caseChangeCost, enhancedFlag, splitCost);
		}
		public static double GetScore(string srcStr, string tarStr, int deleteCost, int insertCost, int replaceCost, int swapCost, int caseChangeCost, bool enhancedFlag, int splitCost) {
			int cost = EditDistance.GetEditDistance(srcStr, tarStr, deleteCost, insertCost, replaceCost, swapCost, caseChangeCost, enhancedFlag);
			int penalty = OrthographicUtil.GetSplitPenalty(srcStr, tarStr, splitCost);
			double score = OrthographicUtil.GetNormScore(cost + penalty, 1000.0);
			return score;
		}
		// private methods
		private static void TestEdSimScores() {
			TestEdSimScore("Spell", "Spell"); // same
			TestEdSimScore("Spell", "Spel"); // delete
			TestEdSimScore("Spell", "Speell"); // insert
			TestEdSimScore("Spell", "Spall"); // replace
			TestEdSimScore("Spell", "Sepll"); // swap
			TestEdSimScore("Spell", "Sp ell"); // insert, split
			TestEdSimScore("Spell", "spell"); // case
			TestEdSimScore("Spell", "Seplls"); // transpose, insert
			TestEdSimScore("SPELL", "spell"); // case
			TestEdSimScore("SPELL", "spel"); // case + delete
			TestEdSimScore("SPELL", "speell"); // case + insert
			TestEdSimScore("SPELL", "spall"); // case + replace
			TestEdSimScore("SPELL", "SEPll"); // 5 case + trnaspose
			TestEdSimScore("SPELL", "sepll"); // 5 case + trnaspose
			TestEdSimScore("SPELL", "saall"); // 5 case + trnaspose
			TestEdSimScore("SPELL", "sEpll"); // 5 case + trnaspose
			TestEdSimScore("SPELL", "sEall"); // 5 case + trnaspose
			TestEdSimScore("SPELL", "sePll"); // 5 case + trnaspose
			TestEdSimScore("Spell", "sp ell"); // insert
			TestEdSimScore("kitten", "sitting"); // insert
			TestEdSimScore("dianosed", "diagnosed"); // insert
			TestEdSimScore("dianosed", "deionized"); // d + s +...
			TestEdSimScore("diagnost", "diagnosed");
			TestEdSimScore("truely", "truly");
			TestEdSimScore("knoledge", "knowledge");
		}
		private static void TestEdSimScore(string srcStr, string tarStr) {
			double score = GetScore(srcStr, tarStr);
			int cost = EditDistance.GetEditDistance(srcStr, tarStr, 96, 90, 100, 94, 10, false);
			int penalty = OrthographicUtil.GetSplitPenalty(srcStr, tarStr, 90);
			Console.WriteLine(srcStr + "|" + tarStr + "|" + cost + "|" + penalty + "|" + score);
		}
		// test Driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java TokenScore");
				Environment.Exit(0);
			}
			// test
			TestEdSimScores();
		}
		// data member
	}

}
using System;

namespace SpellChecker.Engines.cSpell.Ranker {
	/// <summary>
	///***************************************************************************
	/// This class provides a java class to get the similarity score of phonetic 
	/// used in orthographic score.
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
	public class PhoneticScore {
		// private constructor
		private PhoneticScore() { }
		// TBD: read the values from config file
		public static double GetScore(string srcStr, string tarStr) {
			/// <summary>
			/// init value form ensemble
			/// int deleteCost = 95;
			/// int insertCost = 95;
			/// int replaceCost = 100;
			/// int swapCost = 90;
			/// 
			/// </summary>
			// new value
			int deleteCost = 100;
			int insertCost = 100;
			int replaceCost = 100;
			int swapCost = 100;
			int caseChangeCost = 10;
			bool enhancedFlag = false;
			int splitCost = insertCost;
			int maxCodeLength = 10;
			/// <summary>
			/// Test on different phonetic methods
			/// String srcM2 = Metaphone2.GetCode(srcStr, maxCodeLength);
			/// String tarM2 = Metaphone2.GetCode(tarStr, maxCodeLength);
			/// String srcM2 = RefinedSoundex.GetCode(srcStr);
			/// String tarM2 = RefinedSoundex.GetCode(tarStr);
			/// String srcM2 = Caverphone2.GetCaverphone(srcStr);
			/// String tarM2 = Caverphone2.GetCaverphone(tarStr);
			/// String srcM2 = Metaphone.GetMetaphone(srcStr, maxCodeLength);
			/// String tarM2 = Metaphone.GetMetaphone(tarStr, maxCodeLength);
			/// Metaphone3 m3 = new Metaphone3();
			/// m3.SetKeyLength(maxCodeLength);
			/// String srcM2 = m3.GetMetaphone(srcStr);
			/// String tarM2 = m3.GetMetaphone(tarStr);
			/// 
			/// </summary>
			string srcM2 = Metaphone2.GetCode(srcStr, maxCodeLength);
			string tarM2 = Metaphone2.GetCode(tarStr, maxCodeLength);
			int cost = EditDistance.GetEditDistance(srcM2, tarM2, deleteCost, insertCost, replaceCost, swapCost, caseChangeCost, enhancedFlag);
			int penalty = OrthographicUtil.GetSplitPenalty(srcStr, tarStr, splitCost);
			double score = OrthographicUtil.GetNormScore(cost + penalty, 1000.0);
			return score;
		}
		// private methods
		private static void TestSimScores() {
			TestSimScore("kitten", "sitting"); // insert
			TestSimScore("dianosed", "diagnosed"); // insert
			TestSimScore("dianosed", "dianose"); // d + s +...
			TestSimScore("Spell", "Spel"); // delete
			TestSimScore("Spell", "Speell"); // insert
			TestSimScore("Spell", "Spall"); // replace
			TestSimScore("Spell", "Sepll"); // swap
			TestSimScore("Spell", "Sp ell"); // insert
			TestSimScore("Spell", "spell"); // case
			TestSimScore("SPELL", "spell"); // case
			TestSimScore("SPELL", "spel"); // case + delete
			TestSimScore("SPELL", "speell"); // case + insert
			TestSimScore("SPELL", "spall"); // case + replace
			TestSimScore("SPELL", "SEPll"); // 5 case + trnaspose
			TestSimScore("SPELL", "sepll"); // 5 case + trnaspose
			TestSimScore("SPELL", "saall"); // 5 case + trnaspose
			TestSimScore("SPELL", "sEpll"); // 5 case + trnaspose
			TestSimScore("SPELL", "sEall"); // 5 case + trnaspose
			TestSimScore("SPELL", "sePll"); // 5 case + trnaspose
			TestSimScore("Spell", "sp ell"); // insert
			TestSimScore("diagnost", "diagnosed");
			TestSimScore("truely", "truly");
			TestSimScore("knoledge", "knowledge");
		}
		private static void TestSimScore(string srcStr, string tarStr) {
			double score = GetScore(srcStr, tarStr);
			int cost = EditDistance.GetEditDistance(srcStr, tarStr, 96, 90, 100, 94, 10, false);
			Console.WriteLine(srcStr + "|" + tarStr + "|" + cost + "|" + score);
		}
		// test Driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java PhoneticScore");
				Environment.Exit(0);
			}
			// test
			TestSimScores();
		}
		// data member
	}

}
using System;
using SpellChecker.Engines.cSpell.Lib;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///***************************************************************************
	/// This is a java class for orthographic utility to get 
	/// similarity and penalty for split.
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
	public class OrthographicUtil {
		// private constructor
		private OrthographicUtil() { }
		// the srcStr is the spelling error String
		// the tarStr is the suggestion String
		public static int GetSplitPenalty(string srcStr, string tarStr) {
			int splitCost = 1;
			return GetSplitPenalty(srcStr, tarStr, splitCost);
		}
		public static int GetSplitPenalty(string srcStr, string tarStr, int splitCost) {
			int penalty = 0;
			// add penalty for split
			int srcSpaceNo = GetCharNo(srcStr, GlobalVars.SPACE_CHAR);
			int tarSpaceNo = GetCharNo(tarStr, GlobalVars.SPACE_CHAR);
			int splitNo = tarSpaceNo - srcSpaceNo;
			if (splitNo > 0) {
				penalty = splitNo * splitCost;
			}
			return penalty;
		}
		// norm score from 0.0 to 1.000
		public static double GetNormScore(int cost, double ceiling) {
			double score = 0.0;
			if (cost <= ceiling) {
				score = (ceiling - cost) / ceiling;
			}
			return score;
		}
		public static int GetCharNo(string inStr, char matchChar) {
			int charNo = 0;
			if (!string.ReferenceEquals(inStr, null)) {
				for (int i = 0; i < inStr.Length; i++) {
					char curChar = inStr[i];
					if (curChar == matchChar) {
						charNo++;
					}
				}
			}
			return charNo;
		}
		// private methods
		// test Driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java OrthographicUtil");
				Environment.Exit(0);
			}
			// test
		}
		// data member
	}

}
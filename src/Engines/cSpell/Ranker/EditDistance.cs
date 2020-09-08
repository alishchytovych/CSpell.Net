using System;
using SpellChecker.Engines.cSpell.Extensions;

namespace SpellChecker.Engines.cSpell.Ranker {
	/// <summary>
	///***************************************************************************
	/// This class provides a java class to get the cost of edit distance of 
	/// two strings.
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
	public class EditDistance {
		// private constructor
		private EditDistance() {
		}
		// for real-word 1-to-1
		public static int GetDistanceForRealWord(string srcStr, string tarStr) {
			srcStr = srcStr.ToLower();
			tarStr = tarStr.ToLower();
			int deleteCost = 1;
			int insertCost = 1;
			int replaceCost = 1;
			int swapCost = 1;
			int caseChangeCost = 1;
			return GetEditDistance(srcStr, tarStr, deleteCost, insertCost, replaceCost, swapCost, caseChangeCost);
		}
		// get edit distance between two strings
		public static int GetEditDistance(string srcStr, string tarStr) {
			bool caseFlag = true;
			return GetEditDistance(srcStr, tarStr, caseFlag);
		}
		// get edit distance between two strings with options of case flag
		public static int GetEditDistance(string srcStr, string tarStr, bool caseFlag) {
			if (caseFlag == false) {
				srcStr = srcStr.ToLower();
				tarStr = tarStr.ToLower();
			}
			int deleteCost = 1;
			int insertCost = 1;
			int replaceCost = 1;
			int swapCost = 1;
			int caseChangeCost = 1;
			return GetEditDistance(srcStr, tarStr, deleteCost, insertCost, replaceCost, swapCost, caseChangeCost);
		}
		// get edit distance between two strings with options of delete cost,
		// insert cost, replace cost, swap cost, and case change cost
		public static int GetEditDistance(string srcStr, string tarStr, int deleteCost, int insertCost, int replaceCost, int swapCost, int caseChangeCost) {
			bool enhancedFlag = false;
			return GetEditDistance(srcStr, tarStr, deleteCost, insertCost, replaceCost, swapCost, caseChangeCost, enhancedFlag);
		}
		// public method
		/// <summary>
		/// Return Edit distance with options of specify the costs of delete, insert,
		/// replace (substitute), swap (transpose), and case change. In general,
		/// the cost of delete and insert should be the same. The cost of replacement
		/// is higher and swap is lower, while the cost of case change is mimimum.
		/// The algorithm use x[i] for delete, y[j] for add. it is exchangable.
		/// A smart optino is provide to consider transpose with case changes,
		/// instead of two substitutes to minmize the costs.
		/// </summary>
		/// <param name="srcStr">    soruce string </param>
		/// <param name="tarStr">    target string </param>
		/// <param name="deleteCost">    cost of a deletion </param>
		/// <param name="insertCost">    cost of a insertion </param>
		/// <param name="replaceCost">    cost of a replacement (substitution) </param>
		/// <param name="swapCost">    cost of a swap (transposition) </param>
		/// <param name="caseChangeCost">    cost of case change </param>
		/// <param name="enhancedFlag">    true to use enhanced algorithm to min. swap and case
		/// </param>
		/// <returns> the total cost of edit distance </returns>
		public static int GetEditDistance(string srcStr, string tarStr, int deleteCost, int insertCost, int replaceCost, int swapCost, int caseChangeCost, bool enhancedFlag) {
			// add 1 extra cell
			srcStr = " " + srcStr;
			tarStr = " " + tarStr;
			int srcSize = srcStr.Length;
			int tarSize = tarStr.Length;
			int[][] matrix = RectangularArrays.ReturnRectangularIntArray(srcSize, tarSize);
			matrix[0][0] = 0;
			// initialize the first row: x for delete
			for (int i = 1; i < srcSize; i++) {
				matrix[i][0] = matrix[i - 1][0] + deleteCost;
			}
			//initalize the first column: y for insert
			for (int j = 1; j < tarSize; j++) {
				matrix[0][j] = matrix[0][j - 1] + insertCost;
			}
			// fill up the matrix
			for (int i = 1; i < srcSize; i++) {
				char srcChar = srcStr[i];
				for (int j = 1; j < tarSize; j++) {
					char tarChar = tarStr[j];
					if (srcChar == tarChar) {
						//no change required, so just carry the current cost up
						matrix[i][j] = matrix[i - 1][j - 1];
					}
					else {
						int costOfD = matrix[i - 1][j] + deleteCost;
						int costOfI = matrix[i][j - 1] + insertCost;
						int costOfR = replaceCost + matrix[i - 1][j - 1];
						// for the case of case change
						int costOfC = int.MaxValue;
						char srcCharLc = char.ToLower(srcChar);
						char tarCharLc = char.ToLower(tarChar);
						if (srcCharLc == tarCharLc) {
							costOfC = matrix[i - 1][j - 1] + caseChangeCost;
						}

						// for the case of Transpose, swap
						int costOfS = int.MaxValue;
						char srcChar1 = srcStr[i - 1];
						char tarChar1 = tarStr[j - 1];
						char srcChar1Lc = char.ToLower(srcChar1);
						char tarChar1Lc = char.ToLower(tarChar1);
						// transpose, Swap
						if ((i > 1) && (j > 1) && (srcChar == tarChar1) && (srcChar1 == tarChar)) {
							costOfS = matrix[i - 2][j - 2] + swapCost;
						}
						// transpose + 1 case change
						else if ((enhancedFlag == true) && (i > 1) && (j > 1) && (((srcCharLc == tarChar1Lc) && (srcChar1 == tarChar)) || ((srcChar == tarChar1) && (srcChar1Lc == tarCharLc)))) {
							costOfS = matrix[i - 2][j - 2] + swapCost + caseChangeCost;
						}
						// transpose + 2 case change
						else if ((enhancedFlag == true) && (i > 1) && (j > 1) && (srcCharLc == tarChar1Lc) && (srcChar1Lc == tarCharLc)) {
							costOfS = matrix[i - 2][j - 2] + swapCost + 2 * caseChangeCost;
						}

						matrix[i][j] = Minimum5(costOfD, costOfI, costOfR, costOfS, costOfC);
					}
				}
			}
			int cost = matrix[srcSize-1][tarSize-1];

			return cost;
		}
		// check if two input Strings have edit distance within maxEditDist
		public static bool IsWithinEditDistanace(string inStr1, string inStr2, bool caseFlag, int maxEditDist) {
			int length1 = inStr1.Length;
			int length2 = inStr2.Length;
			int lenDiff = ((length1 > length2)?(length1 - length2):(length2 - length1));
			// 1. quick check on length
			if (lenDiff > maxEditDist) {
				return false;
			}
			// 2. check the edit distance
			if (GetEditDistance(inStr1, inStr2, caseFlag) > maxEditDist) {
				return false;
			}
			return true;
		}
		// test driver
		public static void MainTest(string[] args) {
			string srcStr = "kitten";
			string tarStr = "sitting";
			if (args.Length == 2) {
				srcStr = args[0];
				tarStr = args[1];
			}
			else if (args.Length > 0) {
				Console.Error.WriteLine("*** Usage: java EditDistance <Str1> <Str2>");
				Environment.Exit(1);
			}
			Console.WriteLine("srcStr|tarStr|Ed(1)|Ed(1)-noCase|Ed-Jazzy|Ed-Enhanced");
			TestEd(srcStr, tarStr);
			TestEd("fast", "cats");
			Console.WriteLine("--- Insert cases ---");
			TestEd("insert", "insert1");
			TestEd("insert", "1insert");
			TestEd("insert", "inser1t");
			TestEd("insert", "inse12rt");
			TestEd("insert", "1in2se3rt4");
			Console.WriteLine("--- Delete cases ---");
			TestEd("delete", "delet");
			TestEd("delete", "elete");
			TestEd("delete", "elet");
			TestEd("delete", "dlte");
			TestEd("delete", "dlt");
			Console.WriteLine("--- Replace cases ---");
			TestEd("replace", "replac1");
			TestEd("replace", "1eplace");
			TestEd("replace", "re1lace");
			TestEd("replace", "re1la2e");
			Console.WriteLine("--- Transpose cases ---");
			TestEd("transpose", "tarnspose");
			TestEd("transpose", "tarnspsoe");
			Console.WriteLine("--- Transpose + case change ---");
			TestEd("swapCase", "swaCpase");
			TestEd("swapCase", "swaCPase");
			TestEd("swapCase", "swacPase");
			TestEd("swapCase", "swacpase");
			Console.WriteLine("--- Others paper case ---");
			TestEd("dianosed", "diagnosed");
			TestEd("dianosed", "deionized");
			TestEd("Spell", "Spell");
			TestEd("Spell", "Spel");
			TestEd("Spell", "Speell");
			TestEd("Spell", "Spall");
			TestEd("Spell", "Sepll");
			TestEd("Spell", "spell");
			TestEd("Spell", "Sp ell");
			TestEd("SPELL", "spell");
			TestEd("SPELL", "spel");
			TestEd("SPELL", "speell");
			TestEd("SPELL", "spall");
			TestEd("SPELL", "SEPll");
			TestEd("SPELL", "sepll");
			TestEd("SPELL", "saall");
			TestEd("SPELL", "sEpll");
			TestEd("SPELL", "sEall");
			TestEd("SPELL", "sePll");
			TestEd("Spell", "sp ell");
			TestEd("Spell", "Seplls");
		}
		private static void TestEd(string srcStr, string tarStr) {
			int ed = GetEditDistance(srcStr, tarStr);
			int edCase = GetEditDistance(srcStr, tarStr, false);
			int edJ = GetEditDistance(srcStr, tarStr, 95, 95, 100, 90, 10, false);
			int edJ1 = GetEditDistance(srcStr, tarStr, 95, 95, 100, 90, 10, true);
			Console.WriteLine("- ED: [" + srcStr + "|" + tarStr + "|" + ed + "|" + edCase + "|" + edJ + "|" + edJ1 + "]");
		}
		// private method
		// Get minimum of three values
		private static int Minimum3(int a, int b, int c) {
			int min = a;
			min = ((b < min)?b:min);
			min = ((c < min)?c:min);
			return min;
		}
		private static int Minimum5(int a, int b, int c, int d, int e) {
			int min = Minimum3(a, b, c);
			min = Minimum3(min, d, e);
			return min;
		}
		// data member
	}

}
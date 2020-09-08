using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Util;
using SpellChecker.Engines.cSpell.Extensions;

namespace SpellChecker.Engines.cSpell.NlpUtil {

	/// <summary>
	///***************************************************************************
	/// This NLP utility class checks if a token is a number, such as thirty-four.
	/// 
	/// <para><b>History:</b>
	/// <ul>
	/// <li>2018 baseline
	/// </ul>
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ****************************************************************************
	/// </para>
	/// </summary>
	public class NumberTokenUtil {
		// public constructor
		/// <summary>
		/// Public constructor to initiate the HashMap table.
		/// </summary>
		private NumberTokenUtil() { }
		// public methods
		// some token contain "-", such as two-third, thirty-four
		// inToken can't contain spaces 
		public static bool IsNumber(string inToken, HashSet<string> numberSet) {
			bool flag = true;
			string[] inTokenArray = inToken.Split("-", true);
			foreach (string inTokenItem in inTokenArray) {
				if (IsNumberToken(inTokenItem, numberSet) == false) {
					flag = false;
					break;
				}
			}
			return flag;
		}
		// use lower case
		// inToken can't have punctuation
		public static bool IsNumberToken(string inToken, HashSet<string> numberSet) {
			return numberSet.Contains(inToken.ToLower());
		}
		// get number from LEXICON
		public static HashSet<string> GetNumberSetFromFile(string lexNumberFile) {
			int fieldNo = 1;
			bool lowercaseFlag = true;
			HashSet<string> numberSet = FileInToSet.GetHashSetByField(lexNumberFile, fieldNo, lowercaseFlag);
			return numberSet;
		}
		// private methods
		private static void Test(HashSet<string> lexNumberSet) {
			Console.WriteLine("===== Unit Test of NumberTokenUtil =====");
			List<string> inWordList = new List<string>();
			inWordList.Add("Fifty");
			inWordList.Add("half");
			inWordList.Add("two-thirds");
			inWordList.Add("eighty");
			inWordList.Add("first");
			inWordList.Add("firsts");
			inWordList.Add("fourth");
			inWordList.Add("zeroth");
			inWordList.Add("billion");
			inWordList.Add("halve");
			inWordList.Add("thirty-four");
			inWordList.Add("thirty-and");
			inWordList.Add("thirty-a");
			inWordList.Add("half-and-half");
			inWordList.Add("one on one");
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsNumber(" + inWord + "): " + IsNumber(inWord, lexNumberSet));
			}
			Console.WriteLine("===== End of Unit Test =====");
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java NumberTokenUtil");
				Environment.Exit(0);
			}

			// init
			string lexNumberFile = "../data/Dictionary/cSpell/NRVAR";
			HashSet<string> lexNumberSet = NumberTokenUtil.GetNumberSetFromFile(lexNumberFile);
			// test case and print out 
			Test(lexNumberSet);
		}
		// data member
	}

}
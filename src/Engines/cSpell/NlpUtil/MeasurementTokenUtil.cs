using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Util;
using SpellChecker.Engines.cSpell.Extensions;

namespace SpellChecker.Engines.cSpell.NlpUtil {

	/// <summary>
	///***************************************************************************
	/// This NLP utility class checks if a token is a measurement, unit, or 
	/// simplified format of measurement. A siimplified format of measurement is
	/// a digit plus a unit. Such as 25miles, 10inches, -0.5mm, 3weeks, 5mg, etc..
	/// The space between number (digit) and unit is omited.
	/// 
	/// <ul>
	/// <li>2018 baseline
	/// </ul>
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ****************************************************************************
	/// </summary>
	public class MeasurementTokenUtil {
		// private constructor
		private MeasurementTokenUtil() { }
		// public methods
		// includ eunit, simplified, and combined
		public static bool IsMeasurements(string inWord, RootDictionary unitDic) {
			HashSet<string> unitSet = unitDic.GetDictionarySet();
			return IsMeasurements(inWord, unitSet);
		}
		public static bool IsMeasurements(string inWord, HashSet<string> unitSet) {
			bool mFlag = (IsUnits(inWord, unitSet) || IsCombinedMeasurements(inWord, unitSet) || IsSimplifiedMeasurement(inWord, unitSet));
			return mFlag;
		}
		// check if it is an unit
		public static bool IsUnits(string inWord, HashSet<string> unitSet) {
			bool uFlag = IsUnit(inWord, unitSet) || IsCombinedUnit(inWord, unitSet);
			return uFlag;
		}
		public static bool IsUnit(string inWord, HashSet<string> unitSet) {
			bool uFlag = unitSet.Contains(inWord);
			return uFlag;
		}
		// such as mg/day, mcg/ml
		public static bool IsCombinedUnit(string inWord, HashSet<string> unitSet) {
			bool uFlag = false;
			if (inWord.IndexOf("/", StringComparison.Ordinal) > -1) {
				string[] orWords = inWord.Split("/", true);
				bool orFlag = true;
				foreach (string orWord in orWords) {
					if (IsUnit(orWord, unitSet) == false) {
						orFlag = false;
						break;
					}
				}
				uFlag = orFlag;
			}
			return uFlag;
		}
		// 30mg/50mg
		public static bool IsCombinedMeasurements(string inWord, HashSet<string> unitSet) {
			bool mFlag = false;
			if (inWord.IndexOf("/", StringComparison.Ordinal) > -1) {
				string[] orWords = inWord.Split("/", true);
				bool orFlag = true;
				foreach (string orWord in orWords) {
					if (IsSimplifiedMeasurement(orWord, unitSet) == false) {
						orFlag = false;
						break;
					}
				}
				mFlag = orFlag;
			}

			return mFlag;
		}
		// simplified measure matches the patten of [digit] + [unit] without space
		// and lowerCased
		public static bool IsSimplifiedMeasurement(string inWord, HashSet<string> unitSet) {
			bool flag = false;
			var matcherM = patternM_.Match(inWord);
			// 1. check if match measurement pattern
			//if(TokenUtil.IsMatch(inToken, patternM_) == true)
			if (matcherM.Success == true) {
				// split digit and unit
				string digit = matcherM.Groups[1].Value;
				string unit = matcherM.Groups[2].Value.ToLower();
				// check unit
				if (unitSet.Contains(unit) == true) {
					flag = true;
				}
			}

			return flag;
		}

		// private methods
		private static void Tests(HashSet<string> unitSet) {
			// test case
			Console.WriteLine("===== Unit Test of MeasurementTokenUtil =====");
			Console.WriteLine("inWord|Unit|Simple-M|Comb-M|Measurements");
			List<string> inWordList = new List<string>();
			inWordList.Add("0.5mg");
			inWordList.Add("-0.25mm");
			inWordList.Add("70kg");
			inWordList.Add("500MG");
			inWordList.Add("3days");
			inWordList.Add("45Chris");
			inWordList.Add("30mg/50kg");
			inWordList.Add("cm3");
			inWordList.Add("Gbit");
			inWordList.Add("degreesC");
			inWordList.Add("mg/day");
			inWordList.Add("mg/ml");
			inWordList.Add("mg/l");
			inWordList.Add("mcg/ml");

			foreach (string inWord in inWordList) {
				Test(inWord, unitSet);
			}
			Console.WriteLine("===== End of Unit Test =====");
		}
		private static void Test(string inWord, HashSet<string> unitSet) {
			Console.WriteLine(inWord + "|" + IsUnits(inWord, unitSet) + "|" + IsSimplifiedMeasurement(inWord, unitSet) + "|" + IsCombinedMeasurements(inWord, unitSet) + "|" + IsMeasurements(inWord, unitSet));
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java MeasurementTokenUtil");
				Environment.Exit(0);
			}
			// inFile
			string unitFile = "/nfsvol/lex/Lu/Development/Spelling/cSpell2017/data/Dictionary/cSpell/unit.data";
			bool lowerCaseFlag = false; // lowerCase to all input
			HashSet<string> unitSet = FileInToSet.GetHashSetByLine(unitFile, lowerCaseFlag);
			// test case and print out
			Tests(unitSet);
		}
		// data member
		// [digit] + [unit]
		private const string patternStrM_ = "^([+-]?\\d*\\.?\\d+)([a-zA-Z]+)$";
		private static readonly Regex patternM_ = new JRegex(patternStrM_, RegexOptions.Compiled);
	}

}
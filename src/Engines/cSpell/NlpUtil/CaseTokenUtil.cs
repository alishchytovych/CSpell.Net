using System;
using System.Text.RegularExpressions;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.NlpUtil {
	/// <summary>
	///***************************************************************************
	/// This NLP utility class checks the cases of a token.
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
	public class CaseTokenUtil {
		// Private constructor
		/// <summary>
		/// Private constructor so no one can call
		/// </summary>
		private CaseTokenUtil() { }
		// public methods
		public static bool IsLowerCase(string inToken) {
			// can not be pure digit and Punc
			var matcher = patternLC_.Match(inToken);
			bool flag = !DigitPuncTokenUtil.IsDigitPunc(inToken) && matcher.Success;
			return flag;
		}
		public static bool IsUpperCase(string inToken) {
			var matcher = patternUC_.Match(inToken);
			bool flag = !DigitPuncTokenUtil.IsDigitPunc(inToken) && matcher.Success;
			return flag;
		}
		public static bool IsCapitalizedCase(string inToken) {
			var matcher = patternCP_.Match(inToken);
			bool flag = !DigitPuncTokenUtil.IsDigitPunc(inToken) && matcher.Success;
			return flag;
		}
		public static bool IsMixedCased(string inToken) {
			bool flag = !DigitPuncTokenUtil.IsDigitPunc(inToken) && !IsLowerCase(inToken) && !IsUpperCase(inToken) && !IsCapitalizedCase(inToken);
			return flag;
		}
		public static string ToCapitalizedCase(string inToken) {
			string outStr = inToken.Substring(0, 1).ToUpper() + inToken.Substring(1).ToLower();
			return outStr;
		}
		// use lower case
		// private methods
		private static void Tests() {
			Console.WriteLine("===== Unit Test of CaseTokenUtil =====");
			Test("lowercase");
			Test("UPPERCASE");
			Test("Capitalized");
			Test("MixedCased");
			Test("mixedCased");
			Test("mixeD123Cased");
			Test("123");
			Test("!@#");
			Test("123!@#");
		}
		private static void Test(string inStr) {
			Console.WriteLine(inStr + "|" + IsLowerCase(inStr) + "|" + IsUpperCase(inStr) + "|" + IsCapitalizedCase(inStr) + "|" + IsMixedCased(inStr) + "|" + ToCapitalizedCase(inStr));
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java CaseTokenUtil");
				Environment.Exit(0);
			}

			// init
			// test case and print out 
			Tests();
		}
		// data member
		private const string patternStrUC_ = "^[^a-z]*$"; // upperCased
		private const string patternStrLC_ = "^[^A-Z]*$"; // lowerCase
		private const string patternStrCP_ = "^[A-Z][^A-Z]*$"; // Capitalized
		private static readonly Regex patternUC_ = new JRegex(patternStrUC_, RegexOptions.Compiled);
		private static readonly Regex patternLC_ = new JRegex(patternStrLC_, RegexOptions.Compiled);
		private static readonly Regex patternCP_ = new JRegex(patternStrCP_, RegexOptions.Compiled);
	}

}
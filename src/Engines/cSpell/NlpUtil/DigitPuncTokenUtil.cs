using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.NlpUtil {
	/// <summary>
	///***************************************************************************
	/// This NLP utility class handles all operations of digits and punctuation.
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
	public class DigitPuncTokenUtil {
		// public constructor
		/// <summary>
		/// Public constructor to initiate DigitPuncTokenUtil.
		/// </summary>
		private DigitPuncTokenUtil() { }
		// public methods
		public static bool IsDigitPunc(string inToken) {
			bool checkEmptyTokenFlag = true;
			return TokenUtil.IsMatch(inToken, patternDP_, checkEmptyTokenFlag);
		}
		public static bool IsPunc(string inToken) {
			bool checkEmptyTokenFlag = true;
			return TokenUtil.IsMatch(inToken, patternP_, checkEmptyTokenFlag);
		}
		// check if an Arabic number or digit
		public static bool IsDigit(string inToken) {
			bool checkEmptyTokenFlag = true;
			// can not be pure puntuation, such as "-" or "+"
			bool digitFlag = ((IsPunc(inToken) == false) && (TokenUtil.IsMatch(inToken, patternD_, checkEmptyTokenFlag)));
			return digitFlag;
		}
		// private method
		private static void TestDigitPunc() {
			List<string> inWordList = new List<string>();
			inWordList.Add("~!@#$%^&*()");
			inWordList.Add("123.500");
			inWordList.Add("12-35-00");
			inWordList.Add("12.35.00!");
			inWordList.Add("!@#123$%^");
			inWordList.Add("a.456");
			inWordList.Add("");
			inWordList.Add("  "); // space is not a punctuation
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsDigitPunc(" + inWord + "): " + IsDigitPunc(inWord));
			}
		}
		private static void TestPunc() {
			List<string> inWordList = new List<string>();
			inWordList.Add("~!@#$%^&*()");
			inWordList.Add("_");
			inWordList.Add("+`-={}|[]\\:\"<>?;',./");
			inWordList.Add("~!@#$%^&*()_+`-={}|[]\\:\"<>?;',./");
			inWordList.Add("abc");
			inWordList.Add("123.500");
			inWordList.Add("!@#123$%^");
			inWordList.Add("  "); // space is not a punctuation
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsPunc(" + inWord + "): " + IsPunc(inWord));
			}
		}
		private static void TestDigit() {
			List<string> inWordList = new List<string>();
			inWordList.Add("+123.456");
			inWordList.Add("-123.456");
			inWordList.Add("+123.");
			inWordList.Add("+123");
			inWordList.Add("123.");
			inWordList.Add("123");
			inWordList.Add("3");
			inWordList.Add("3.");
			inWordList.Add("3.0");
			inWordList.Add(".4");
			inWordList.Add(".45");
			inWordList.Add("+.456");
			inWordList.Add(".456");
			inWordList.Add("0.4");
			inWordList.Add("a0.456");
			inWordList.Add("a.456");
			inWordList.Add("0.456b");
			inWordList.Add("");
			inWordList.Add("a");
			inWordList.Add("-"); // shoudl be false
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsDigit(" + inWord + "): " + IsDigit(inWord));
			}
		}
		private static void Test() {
			Console.WriteLine("===== Unit Test of DigitPuncTokenUtil =====");
			//TestDigit();
			Console.WriteLine("---- TestPunc() ---");
			TestPunc();
			Console.WriteLine("---- TestDigit() ---");
			TestDigit();
			Console.WriteLine("---- TestDigitPunc() ---");
			TestDigitPunc();
			Console.WriteLine("===== End of Unit Test =====");
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java DigitPuncTokenUtil");
				Environment.Exit(0);
			}

			// test case and print out 
			Test();
		}
		// data member
		private const string patternStrDP_ = "^([\\W_\\d-[^\\S]]+)$";
		private static readonly Regex patternDP_ = new JRegex(patternStrDP_, RegexOptions.Compiled);
		// non-word char|_|&& not non-space char
		//private static final String patternStrP_ = "^([\\W_&&\\S]+)$";
		private const string patternStrP_ = "^([!\"#$%&'()*+,\\-./:;<=>?@\\[\\\\\\]^_`{|}~]+)$";
		private static readonly Regex patternP_ = new JRegex(patternStrP_, RegexOptions.Compiled);
		//private static final String patternStrD_ = "^([+-]?(\\d*\\.)?\\d*)$";
		private const string patternStrD_ = "^([+-]?(\\d)*(\\.)?\\d*)$";
		private static readonly Regex patternD_ = new JRegex(patternStrD_, RegexOptions.Compiled);
	}

}
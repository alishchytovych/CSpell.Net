using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.NdCorrector {

	/// <summary>
	///***************************************************************************
	/// This class is the collection for all leading punctuations.
	/// 
	/// Leading punctuation is a punctuation leads a word after a space.
	/// It includes:
	/// - Ampersand [&amp;]
	/// - Left Parenthesis [(]
	/// - Left square bracket [[]
	/// - Left curlu Brace [{]
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
	public class LeadingPunc {
		// private constructor
		private LeadingPunc() { }

		public static bool IsException(string inWord, string leadingPunc) {
			Regex pattern = null;
			switch (leadingPunc) {
				case LEADING_A:
					pattern = patternA_;
					break;
				case LEADING_LP:
					pattern = patternLP_;
					break;
				case LEADING_LSB:
					pattern = patternLSB_;
					break;
				case LEADING_LCB:
					pattern = patternLCB_;
					break;
			}

			bool exceptionFlag = false;
			if (pattern != null) {
				var matcher = pattern.Match(inWord);
				exceptionFlag = matcher.Success;
			}
			return exceptionFlag;
		}
		public static string GetSplitStr(string inWord, string leadingPunc) {
			string outStr = inWord;
			// split it if inWord is not an exception
			if (IsException(inWord, leadingPunc) == false) {
				outStr = SplitObj.GetSplitStrBeforePunc(inWord, leadingPunc);
			}
			return outStr;
		}
		private static void TestExceptionA() {
			Console.WriteLine("----- Leading Punc Exception: ampersand -----");
			List<string> inWordList = new List<string>();
			inWordList.Add("AT&T");
			inWordList.Add("R&D");
			inWordList.Add("AT&");
			inWordList.Add("1&2");
			inWordList.Add("a&b");
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsException(" + inWord + "): " + IsException(inWord, LEADING_A));
			}
		}
		private static void TestExceptionLP() {
			Console.WriteLine("----- Leading Punc Exception: Left parenthesis -----");
			List<string> inWordList = new List<string>();
			// exception 1
			inWordList.Add("RS(3)PE");
			inWordList.Add("Ca(2+)");
			inWordList.Add("Ca(2+)-ATPase");
			inWordList.Add("P(450)");
			inWordList.Add("B(12)");
			inWordList.Add("δ(18)O");
			inWordList.Add("XX(2)YY(3)");
			// exception 2
			inWordList.Add("V(max)");
			inWordList.Add("C(min)");
			// exception 3
			inWordList.Add("D(+)HUS");
			inWordList.Add("GABA(A)");
			inWordList.Add("apolipoprotein(a)");
			inWordList.Add("beta(1)");
			inWordList.Add("homocyst(e)ine");
			inWordList.Add("poly(A)-binding");
			// exception 4
			inWordList.Add("finger(s)");
			inWordList.Add("fetus(es)");
			inWordList.Add("extremity(ies)");
			// exception 5
			inWordList.Add("poly-(ethylene");
			inWordList.Add("poly-(ADP-ribose)");
			inWordList.Add("C-(17:0)");
			inWordList.Add("I-(alpha)");
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsException(" + inWord + "): " + IsException(inWord, LEADING_LP));
			}
		}
		private static void TestExceptionLSB() {
			Console.WriteLine("----- Leading Punc Exception: right square brace -----");
			List<string> inWordList = new List<string>();
			inWordList.Add("benzo[a]pyrene");
			inWordList.Add("B[e]P");
			inWordList.Add("-[NAME]"); // 18175.txt
			inWordList.Add("~[NAME]"); // 4.txt
			inWordList.Add("[11C]MeG");
			inWordList.Add("[3H]-thymidine");
			inWordList.Add("[3H]tyrosine");
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsException(" + inWord + "): " + IsException(inWord, LEADING_LSB));
			}
		}
		private static void TestExceptionLCB() {
			Console.WriteLine("----- Leading Punc Exception: rigth curely braket -----");
			List<string> inWordList = new List<string>();
			inWordList.Add("X");
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsException(" + inWord + "): " + IsException(inWord, LEADING_LCB));
			}
		}
		private static void TestException() {
			TestExceptionA();
			TestExceptionLP();
			TestExceptionLSB();
			TestExceptionLCB();
		}
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java LeadingPunc");
				Environment.Exit(0);
			}

			// init
			// test case:  use CsText to test
			TestException();
		}
		// data member
		// exception pattern of leading punctuation of ampersand
		private const string patternStrA_ = "^[A-Z]+&[A-Z]+$";
		private static readonly Regex patternA_ = new JRegex(patternStrA_, RegexOptions.Compiled);
		// exception pattern of leading punctuation of right parenthesis )
		private const string patternStrLP_ = "^((\\S)*\\([\\d]+(\\+)?\\)(\\S)*)|((\\S)*\\((max|min)\\))|((\\S)*\\([+\\w]\\)(\\S)*)|([\\w]+((s\\(es\\))|(y\\(ies\\))))|((\\S)*-\\((\\S)*)$";
		private static readonly Regex patternLP_ = new JRegex(patternStrLP_, RegexOptions.Compiled);
		// exception pattern of leading punctuation of right square bracket
		private const string patternStrLSB_ = "^(\\S*\\[[a-z]\\]\\S*)|([~\\-]\\[\\S*)$";
		private static readonly Regex patternLSB_ = new JRegex(patternStrLSB_, RegexOptions.Compiled);
		// exception pattern of leading punctuation of right curly brace: None
		private const string patternStrLCB_ = "$^";
		private static readonly Regex patternLCB_ = new JRegex(patternStrLCB_, RegexOptions.Compiled);
		// exception pattern of leading punctuation of period
		public const string LEADING_A = "&"; // a: ampersand
		public const string LEADING_LP = "("; // lp: left parenthesis
		public const string LEADING_LSB = "["; // lsb: left square bracket
		public const string LEADING_LCB = "{"; // lcb: left curly brace
	}

}
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.NdCorrector {

	/// <summary>
	///***************************************************************************
	/// This class contains the collection for all ending punctuations.
	/// 
	/// Ending punctuation is an punctuation ends a word and followed by a space.
	/// It includes:
	/// - period [.]:  
	/// - Question mark [?]: 
	/// - Exclamation mark [!]: 
	/// - Comma [,]
	/// - Semicolon [;]
	/// - Colon [:]
	/// - Ampersand [&amp;]
	/// - Right Parenthesis [)]
	/// - Right square bracket []]
	/// - Right curlu Brace [}]
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
	public class EndingPunc {
		// private constructor
		private EndingPunc() { }

		public static bool IsException(string inWord, string endingPunc) {
			Regex pattern = null;
			switch (endingPunc) {
				case ENDING_P:
					pattern = patternP_;
					break;
				case ENDING_QM:
					pattern = patternQM_;
					break;
				case ENDING_EM:
					pattern = patternEM_;
					break;
				case ENDING_CA:
					pattern = patternCA_;
					break;
				case ENDING_SC:
					pattern = patternSC_;
					break;
				case ENDING_CL:
					pattern = patternCL_;
					break;
				case ENDING_A:
					pattern = patternA_;
					break;
				case ENDING_RP:
					pattern = patternRP_;
					break;
				case ENDING_RSB:
					pattern = patternRSB_;
					break;
				case ENDING_RCB:
					pattern = patternRCB_;
					break;
			}

			bool exceptionFlag = false;
			if (pattern != null) {
				var matcher = pattern.Match(inWord);
				exceptionFlag = matcher.Success;
			}
			return exceptionFlag;
		}
		public static string GetSplitStr(string inWord, string endingPunc) {
			string outStr = inWord;
			// split it if inWord is not an exception
			if (IsException(inWord, endingPunc) == false) {
				outStr = SplitObj.GetSplitStrAfterPunc(inWord, endingPunc);
			}
			return outStr;
		}
		private static void TestExceptionP() {
			Console.WriteLine("----- Ending Punc Exception: period -----");
			List<string> inWordPList = new List<string>();
			// exception 1
			inWordPList.Add("Dr.s");
			inWordPList.Add("Mr.s");
			// exception 2
			inWordPList.Add("16q22.1");
			inWordPList.Add("123.2");
			inWordPList.Add("123.234.4567");
			inWordPList.Add("1c3.2d4.4e6");
			inWordPList.Add("123.23a4.456");
			inWordPList.Add("123a.234.456");
			// exception 3
			inWordPList.Add("D.C.A.B.");
			inWordPList.Add("D.C.A.B");
			inWordPList.Add("d.c.a.");
			inWordPList.Add("d.c.a");
			inWordPList.Add("D.c");
			inWordPList.Add("D.CC.A.B.");
			inWordPList.Add("DD.C.A.B.");
			inWordPList.Add("d.1.a.");
			inWordPList.Add("D.123.A.B.");
			// exception 4
			inWordPList.Add("St.-John");
			inWordPList.Add("123.-John");
			inWordPList.Add("#$.-John");
			inWordPList.Add("St.$%^John");
			inWordPList.Add("St.John");
			inWordPList.Add("St.J.");
			inWordPList.Add("Test...123");
			foreach (string inWordP in inWordPList) {
				Console.WriteLine("- IsException(" + inWordP + "): " + EndingPunc.IsException(inWordP, ENDING_P));
			}
		}
		private static void TestExceptionQM() {
			Console.WriteLine("----- Ending Punc Exception: Question Mark -----");
			List<string> inWordList = new List<string>();
			inWordList.Add("ulcers?'"); // 12769.txt
			inWordList.Add("ulcers?\"");
			inWordList.Add("ulcers?]");
			inWordList.Add("XX?'test");
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsException(" + inWord + "): " + EndingPunc.IsException(inWord, ENDING_QM));
			}
		}
		private static void TestExceptionEM() {
			Console.WriteLine("----- Ending Punc Exception: Exclamation Mark -----");
			List<string> inWordList = new List<string>();
			inWordList.Add("ulcers!'");
			inWordList.Add("ulcers!\"");
			inWordList.Add("ulcers!]");
			inWordList.Add("XX!'test");
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsException(" + inWord + "): " + EndingPunc.IsException(inWord, ENDING_EM));
			}
		}
		private static void TestExceptionCA() {
			Console.WriteLine("----- Ending Punc Exception: Comma -----");
			List<string> inWordList = new List<string>();
			inWordList.Add("50,000");
			inWordList.Add("1,234,567");
			inWordList.Add("123");
			inWordList.Add("12,34");
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsException(" + inWord + "): " + EndingPunc.IsException(inWord, ENDING_CA));
			}
		}
		private static void TestExceptionCL() {
			Console.WriteLine("----- Ending Punc Exception: colon -----");
			List<string> inWordList = new List<string>();
			inWordList.Add("12:34");
			inWordList.Add("1a2:34");
			inWordList.Add("12:3a4");
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsException(" + inWord + "): " + IsException(inWord, ENDING_CL));
			}
		}
		private static void TestExceptionSC() {
			Console.WriteLine("----- Ending Punc Exception: semicolon -----");
			List<string> inWordList = new List<string>();
			inWordList.Add("XXX;\"");
			inWordList.Add("fusion;syrinx"); // 22.txt
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsException(" + inWord + "): " + IsException(inWord, ENDING_SC));
			}
		}
		private static void TestExceptionA() {
			Console.WriteLine("----- Ending Punc Exception: ampersand -----");
			List<string> inWordList = new List<string>();
			inWordList.Add("AT&T");
			inWordList.Add("R&D");
			inWordList.Add("AT&");
			inWordList.Add("1&2");
			inWordList.Add("a&b");
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsException(" + inWord + "): " + IsException(inWord, ENDING_A));
			}
		}
		private static void TestExceptionRP() {
			Console.WriteLine("----- Ending Punc Exception: Right parenthesis -----");
			List<string> inWordRPList = new List<string>();
			// exception 1
			inWordRPList.Add("homocyst(e)ine");
			inWordRPList.Add("RS(3)PE");
			inWordRPList.Add("NAD(P)H");
			inWordRPList.Add("D(+)HUS");
			inWordRPList.Add("XX(2)YY(3)");
			inWordRPList.Add("XXX( )YYY"); // can't be empty in ()
			inWordRPList.Add(" (A)Y"); // can't be empty before ()
			// exception 2
			inWordRPList.Add("Ca(2+)-ATPase");
			inWordRPList.Add("beta(2)-microglobulin");
			inWordRPList.Add("G(i)-protein");
			inWordRPList.Add("(Si)-synthase");
			inWordRPList.Add("(2)-integrin");
			inWordRPList.Add("Zn2(+)-binding");
			inWordRPList.Add("I(131)-albumin");
			inWordRPList.Add("poly(A)-binding");
			inWordRPList.Add("(asparaginyl)-β-hydroxylase");
			inWordRPList.Add("(ADP)-ribose");
			inWordRPList.Add("xxx(yyy-xxx)-zzz"); //can't have - inside ()
			inWordRPList.Add("xxx(yyy-xxx)-zzz-112"); //can't have - inside ()
			inWordRPList.Add("xxx(yyy)zzz"); //must have - after ()
			inWordRPList.Add("xxx(2)-a 123"); // can't have a space
			// exception 3
			inWordRPList.Add("VO(2)max");
			inWordRPList.Add("δ(18)O");
			inWordRPList.Add("(123)I-mIBG");
			inWordRPList.Add("(131)I");
			inWordRPList.Add("xx(1)");
			inWordRPList.Add("(1)yy");
			// exception 4
			inWordRPList.Add("1");
			foreach (string inWordRP in inWordRPList) {
				Console.WriteLine("- IsException(" + inWordRP + "): " + IsException(inWordRP, ENDING_RP));
			}
		}
		private static void TestExceptionRSB() {
			Console.WriteLine("----- Ending Punc Exception: right square brace -----");
			List<string> inWordList = new List<string>();
			inWordList.Add("[11C]MeG");
			inWordList.Add("[3H]-thymidine");
			inWordList.Add("[3H]tyrosine");
			inWordList.Add("benzo[a]pyrene");
			inWordList.Add("B[e]P");
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsException(" + inWord + "): " + IsException(inWord, ENDING_RSB));
			}
		}
		private static void TestExceptionRCB() {
			Console.WriteLine("----- Ending Punc Exception: rigth curely braket -----");
			List<string> inWordList = new List<string>();
			inWordList.Add("X");
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsException(" + inWord + "): " + IsException(inWord, ENDING_RCB));
			}
		}
		private static void TestException() {
			TestExceptionP();
			TestExceptionQM();
			TestExceptionEM();
			TestExceptionCA();
			TestExceptionCL();
			TestExceptionSC();
			TestExceptionA();
			TestExceptionRP();
			TestExceptionRSB();
			TestExceptionRCB();
		}
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java EndingPunc");
				Environment.Exit(0);
			}

			// init
			// test case:  use CsText to test
			TestException();
		}
		// data member
		// exception pattern of ending punctuation of period
		private const string patternStrP_ = "^(.*\\.s)|((\\w*\\d\\.\\d\\w*)+)|((\\D\\.)+\\D?)|(\\w*\\.-\\w*)|(.*\\.['\"])$";
		private static readonly Regex patternP_ = new JRegex(patternStrP_, RegexOptions.Compiled);
		// exception pattern of ending punctuation of question mark
		private const string patternStrQM_ = "^(.*\\?['\"])$";
		private static readonly Regex patternQM_ = new JRegex(patternStrQM_, RegexOptions.Compiled);
		// exception pattern of ending punctuation of exclamation mark
		private const string patternStrEM_ = "^(.*!['\"])$";
		private static readonly Regex patternEM_ = new JRegex(patternStrEM_, RegexOptions.Compiled);
		// exception pattern of ending punctuation of comma
		private const string patternStrCA_ = "^(\\d+(,[\\d]{3})+)$";
		private static readonly Regex patternCA_ = new JRegex(patternStrCA_, RegexOptions.Compiled);
		// exception pattern of ending punctuation of colon
		private const string patternStrCL_ = "^(\\d+:\\d+)$";
		private static readonly Regex patternCL_ = new JRegex(patternStrCL_, RegexOptions.Compiled);
		// exception pattern of ending punctuation of semicolon
		// no exception, always false, $ alwyas after ^, so it always false
		private const string patternStrSC_ = "$^";
		private static readonly Regex patternSC_ = new JRegex(patternStrSC_, RegexOptions.Compiled);
		// exception pattern of ending punctuation of ampersand
		private const string patternStrA_ = "^[A-Z]+&[A-Z]+$";
		private static readonly Regex patternA_ = new JRegex(patternStrA_, RegexOptions.Compiled);
		// exception pattern of ending punctuationof right parenthesis )
		private const string patternStrRP_ = "^((\\S)*\\([+\\w]\\)(\\S)*)|((\\S)*\\([+\\w]+\\)-(\\S)*)|((\\S)*\\(\\d+\\)(\\S)*)$";
		private static readonly Regex patternRP_ = new JRegex(patternStrRP_, RegexOptions.Compiled);
		// exception pattern of ending punctuation of right square bracket
		private const string patternStrRSB_ = "^(\\S*\\[\\d+[A-Z]\\]\\S*)|(\\S*\\[[a-z]\\]\\S*)$";
		private static readonly Regex patternRSB_ = new JRegex(patternStrRSB_, RegexOptions.Compiled);
		// exception pattern of ending punctuation of right curly brace
		private const string patternStrRCB_ = "$^";
		private static readonly Regex patternRCB_ = new JRegex(patternStrRCB_, RegexOptions.Compiled);
		// exception pattern of ending punctuation of period
		public const string ENDING_P = "."; // p: period
		public const string ENDING_QM = "?"; // qm: question mark
		public const string ENDING_EM = "!"; // em: exclamation mark
		public const string ENDING_CA = ","; // ca: comma
		public const string ENDING_SC = ";"; // sc: semicolon
		public const string ENDING_CL = ":"; // cl: colon
		public const string ENDING_A = "&"; // a: ampersand
		public const string ENDING_RP = ")"; // rp: right parenthesis
		public const string ENDING_RSB = "]"; // rsb: right square bracket
		public const string ENDING_RCB = "}"; // rcb: right curly brace
	}

}
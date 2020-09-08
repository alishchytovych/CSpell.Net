using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.NlpUtil {
	/// <summary>
	///***************************************************************************
	/// This NLP utility class checks if a token ends with the pattern of possessive. 
	/// The pattern includes (in both uppercase and lowercase):
	/// 
	/// <ul>
	/// <li>xxx's: Alzheimer's 
	/// <li>xxxs': Alzheimers'
	/// <li>yyyx': Bazex', Elixs', 
	/// <li>xxxz': Duroziez', Schultz', Vaquez', Malassez'
	/// </ul>
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
	public class PossessiveTokenUtil {
		// private constructor
		private PossessiveTokenUtil() { }
		// public methods
		public static string GetOrgWord(string inToken) {
			string outStr = inToken;
			if (IsPossessivePattern1(inToken) == true) {
				outStr = inToken.Substring(0, inToken.Length - 1);
			} else if (IsPossessivePattern2(inToken) == true) {
				outStr = inToken.Substring(0, inToken.Length - 2);
			}
			return outStr;
		}
		public static bool IsPossessive(string inToken) {
			bool flag = IsPossessivePattern1(inToken) || IsPossessivePattern2(inToken);
			return flag;
		}
		// true: if meet possessive pattern 
		// private method
		private static bool IsPossessivePattern1(string inToken) {
			bool flag = false;
			// check if ends with possessive
			// upperCase: 'S, S', X', Z'
			if ((TokenUtil.IsMatch(inToken, pattern1a_) == true) || (TokenUtil.IsMatch(inToken, pattern1b_) == true) || (TokenUtil.IsMatch(inToken, pattern1c_) == true) || (TokenUtil.IsMatch(inToken, pattern1d_) == true) || (TokenUtil.IsMatch(inToken, pattern1e_) == true) || (TokenUtil.IsMatch(inToken, pattern1f_) == true)) {
				flag = true;
			}
			return flag;
		}
		private static bool IsPossessivePattern2(string inToken) {
			bool flag = false;
			// check if ends with possessive
			// upperCase: 'S, S', X', Z'
			if ((TokenUtil.IsMatch(inToken, pattern2a_) == true) || (TokenUtil.IsMatch(inToken, pattern2b_) == true)) {
				flag = true;
			}
			return flag;
		}

		private static void Test() {
			// test case
			Console.WriteLine("=== Unit Test of PossessiveTokenUtil ===");
			List<string> inWordList = new List<string>();
			// uppercase
			inWordList.Add("");
			inWordList.Add(" ");
			inWordList.Add("Chris");
			inWordList.Add("CHRIS'S");
			inWordList.Add("Chris'S");
			inWordList.Add("CHRIS'");
			inWordList.Add("BAZEX'");
			inWordList.Add("SCHULTZ'");
			// lowercase
			inWordList.Add("Alzheimer's");
			inWordList.Add("mediator's");
			inWordList.Add("Alzheimers'");
			inWordList.Add("mediators'");
			inWordList.Add("Duroziez'");
			inWordList.Add("tiz'");
			inWordList.Add("Bazex'");
			inWordList.Add("Elixs'");
			// more test
			inWordList.Add("Chris's");
			inWordList.Add("That is Chris's");
			inWordList.Add("That is Chris's idea");
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsPossessive(" + inWord + "): [" + IsPossessive(inWord) + "], GetOrgWord: [" + GetOrgWord(inWord) + "]");
			}
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java PossessiveTokenUtil");
				Environment.Exit(0);
			}
			// test case and print out
			Test();
		}
		// data member
		// any not lowercase letters ([^a-z]) with S', X', Z'
		private const string patternStr1a_ = "^[^a-z]+S'$";
		private const string patternStr1b_ = "^[^a-z]+X'$";
		private const string patternStr1c_ = "^[^a-z]+Z'$";

		// any uppercase, lowercase letter or space with s', x', z'
		private const string patternStr1d_ = "^[a-zA-Z]+s'$";
		private const string patternStr1e_ = "^[a-zA-Z]+x'$";
		private const string patternStr1f_ = "^[a-zA-Z]+z'$";
		// any not lowercase letters ([^a-z]) with 'S
		private const string patternStr2a_ = "^[^a-z]+'S$";
		// any uppercase, lowercase letter or space with 's
		private const string patternStr2b_ = "^[a-zA-Z]+'s$";
		private static readonly Regex pattern1a_ = new JRegex(patternStr1a_, RegexOptions.Compiled);
		private static readonly Regex pattern1b_ = new JRegex(patternStr1b_, RegexOptions.Compiled);
		private static readonly Regex pattern1c_ = new JRegex(patternStr1c_, RegexOptions.Compiled);
		private static readonly Regex pattern1d_ = new JRegex(patternStr1d_, RegexOptions.Compiled);
		private static readonly Regex pattern1e_ = new JRegex(patternStr1e_, RegexOptions.Compiled);
		private static readonly Regex pattern1f_ = new JRegex(patternStr1f_, RegexOptions.Compiled);
		private static readonly Regex pattern2a_ = new JRegex(patternStr2a_, RegexOptions.Compiled);
		private static readonly Regex pattern2b_ = new JRegex(patternStr2b_, RegexOptions.Compiled);
	}

}
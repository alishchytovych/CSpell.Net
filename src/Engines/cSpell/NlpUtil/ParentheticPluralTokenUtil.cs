using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.NlpUtil {
	/// <summary>
	///***************************************************************************
	/// This NLP utility class checks if a token ends with the pattern of parenthetic
	/// plural forms, such as (s), (es), (ies). 
	/// 
	/// <ul>
	/// <li>xxx(s): finger(s), hand(s), book(s) 
	/// <li>xxx(es)': mass(es), fetus(es), box(es), waltz(es), match(es), splash(es) 
	/// <li>xxx(ies): fly(ies), extremity(ies)
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
	public class ParentheticPluralTokenUtil {
		// private constructor
		private ParentheticPluralTokenUtil() { }
		// public methods
		public static string GetOrgWord(string inToken) {
			string outStr = inToken;
			if (IsParentheticPluralS(inToken) == true) {
				outStr = inToken.Substring(0, inToken.Length - 3);
			} else if (IsParentheticPluralEs(inToken) == true) {
				outStr = inToken.Substring(0, inToken.Length - 4);
			} else if (IsParentheticPluralIes(inToken) == true) {
				outStr = inToken.Substring(0, inToken.Length - 5);
			}
			return outStr;
		}
		public static bool IsParentheticPlural(string inToken) {
			bool flag = IsParentheticPluralS(inToken) || IsParentheticPluralEs(inToken) || IsParentheticPluralIes(inToken);
			return flag;
		}
		// true: if meet possessive pattern 
		// private method
		private static bool IsParentheticPluralS(string inToken) {
			bool flag = false;
			if ((TokenUtil.IsMatch(inToken, patternS_) == true) && (TokenUtil.IsMatch(inToken, patternS1_) == false) && (TokenUtil.IsMatch(inToken, patternS2_) == false)) {
				flag = true;
			}
			return flag;
		}
		private static bool IsParentheticPluralEs(string inToken) {
			bool flag = false;
			if (TokenUtil.IsMatch(inToken, patternEs_) == true) {
				flag = true;
			}
			return flag;
		}
		private static bool IsParentheticPluralIes(string inToken) {
			bool flag = false;
			if (TokenUtil.IsMatch(inToken, patternIes_) == true) {
				flag = true;
			}
			return flag;
		}

		private static void Test() {
			// test case
			Console.WriteLine("=== Unit Test of ParentheticPluralTokenUtil ===");
			List<string> inWordList = new List<string>();
			// (s)
			inWordList.Add("");
			inWordList.Add(" ");
			inWordList.Add("finger(s)");
			inWordList.Add("toe(s)");
			inWordList.Add("rib(s)");
			inWordList.Add("match(s)");
			inWordList.Add("fly(s)");
			// (es)
			inWordList.Add("Mass(es)");
			inWordList.Add("exostosis(es)");
			inWordList.Add("plexus(es)");
			inWordList.Add("fetus(es)");
			inWordList.Add("illness(es)");
			inWordList.Add("waltz(es)");
			inWordList.Add("box(es)");
			inWordList.Add("match(es)");
			inWordList.Add("splash(es)");
			inWordList.Add("yyy(es)");
			inWordList.Add("graph(es)");
			// (ies)
			inWordList.Add("pneumonectomy(ies)");
			inWordList.Add("extremity(ies)");
			inWordList.Add("fly(ies)");
			inWordList.Add("fay(ies)");
			inWordList.Add("i9y(ies)");
			inWordList.Add("xUy(ies)");
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsParentheticPlural(" + inWord + "): [" + IsParentheticPlural(inWord) + "], GetOrgWord: [" + GetOrgWord(inWord) + "]");
			}
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java ParentheticPluralTokenUtil");
				Environment.Exit(0);
			}
			// test case and print out
			Test();
		}
		// data member
		// pattern: (s)
		private const string patternSStr_ = "^[a-zA-Z]+\\(s\\)$";
		// not regalar plural forms
		private const string patternS1Str_ = "^[a-zA-Z]+([sxz]|(ch)|(sh))\\(s\\)$";
		private const string patternS2Str_ = "^[a-zA-Z]+[^aeiouAEIOU0-9]y\\(s\\)$";
		// pattern: s|x|z|ch|sh (es)
		private const string patternEsStr_ = "^[a-zA-Z]+([sxz]|(ch)|(sh))\\(es\\)$";
		// pattern: Cy(s)
		// pattern: Cy(ies)
		// C: b-d,f-h,j-n,p-t,v-z,not Vowels: a,e,i,o,u
		private const string patternIesStr_ = "^[a-zA-Z]+[^aeiouAEIOU0-9]y\\(ies\\)$";

		private static readonly Regex patternS_ = new JRegex(patternSStr_, RegexOptions.Compiled);
		// not regular plural forms
		private static readonly Regex patternS1_ = new JRegex(patternS1Str_, RegexOptions.Compiled);
		private static readonly Regex patternS2_ = new JRegex(patternS2Str_, RegexOptions.Compiled);
		private static readonly Regex patternEs_ = new JRegex(patternEsStr_, RegexOptions.Compiled);
		private static readonly Regex patternIes_ = new JRegex(patternIesStr_, RegexOptions.Compiled);
	}

}
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SpellChecker.Engines.cSpell.Util;
namespace SpellChecker.Engines.cSpell.NlpUtil {
	/// <summary>
	///***************************************************************************
	/// This class is the java object for token utility.
	/// It includes lots of methods to validate the type of a token.
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
	public class TokenUtil {
		// public constructor
		/// <summary>
		/// Public constructor to initiate the HashMap table.
		/// </summary>
		private TokenUtil() { }
		// public methods
		public static bool IsSpaceToken(string inStr) {
			bool spaceFlag = spaceStrSet_.Contains(inStr);
			return spaceFlag;
		}
		public static bool IsEmptyToken(string inStr) {
			bool flag = ((string.ReferenceEquals(inStr, null)) || (inStr.Length == 0));
			return flag;
		}
		// to be deleted, no need
		// empty token is a token empty (no length) or null
		public static bool IsNotEmptyToken(string inToken) {
			bool flag = true;
			if ((string.ReferenceEquals(inToken, null)) || (inToken.Length == 0)) {
				flag = false;
			}
			return flag;
		}
		public static bool IsMatch(string inToken, Regex inPattern) {
			bool checkEmptyToken = true;
			return IsMatch(inToken, inPattern, checkEmptyToken);
		}
		public static bool IsMatch(string inToken, Regex inPattern, bool checkEmptyToken) {
			bool flag = false;
			if (checkEmptyToken == true) {
				// check if empty token
				//if(TokenUtil.IsNotEmptyToken(inToken) == true)
				if (TokenUtil.IsEmptyToken(inToken) == false) {
					var matcher = inPattern.Match(inToken);
					flag = matcher.Success;
				}
			} else { // option: not to check empty token
				var matcher = inPattern.Match(inToken);
				flag = matcher.Success;
			}
			return flag;
		}
		public static bool IsName(string inToken) {
			// TBD
			return true;
		}
		// private methods
		private static void Test() {
			Console.WriteLine("===== Unit Test of TokenUtil =====");
			Console.WriteLine("-------");
			string str1 = "­"; // unicode space
			Console.WriteLine("- IsSpaceToken: " + TokenUtil.IsSpaceToken(str1));
			Console.WriteLine("===== End of Unit Test =====");
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java TokenUtil");
				Environment.Exit(0);
			}

			// test case and print out 
			Test();
		}
		// data member
		// the unicode space str set must sync with TextObj.patternStrSpace_
		private static readonly HashSet<string> spaceStrSet_ = new HashSet<string>();
		static TokenUtil() {
			spaceStrSet_.Add(" "); // U+0020, SPACE
			spaceStrSet_.Add("\t");
			spaceStrSet_.Add("\n");
			spaceStrSet_.Add(" "); // U+00A0, NO-BREAK SPACE
			spaceStrSet_.Add("­"); // U+00AD, SOFT HYPHEN
		}
	}

}
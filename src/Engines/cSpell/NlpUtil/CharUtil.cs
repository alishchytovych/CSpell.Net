using System;
using System.Globalization;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.NlpUtil {

	/// <summary>
	///***************************************************************************
	/// This NLP utility class handles all operations of characters.
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
	public class CharUtil {
		// public constructor
		/// <summary>
		/// Private constructor 
		/// </summary>
		private CharUtil() { }
		// public methods
		// check if the inChar is in the specified chars (in a string format)
		public static bool IsSpecifiedChar(char inChar, string specChars) {
			bool isSpecCharFlag = false;
			if (!string.ReferenceEquals(specChars, null)) {
				for (int i = 0; i < specChars.Length; i++) {
					if (inChar == specChars[i]) {
						isSpecCharFlag = true;
						break;
					}
				}
			}
			return isSpecCharFlag;
		}
		//check if the inChar is a punctuation
		public static bool IsPunctuation(char inChar) {
			for (int i = 0; i < punctuations.Length; i++)
				if (punctuations[i] == inChar)
					return true;
			return false;
			//return Char.IsPunctuation(inChar);
		}
		// privage methods
		private static void Test() {
			Console.WriteLine("===== Unit Test of CharUtil =====");
			char inChar = '.';
			char inChar2 = 'A';
			string inStr = "12.ab%^";
			for (int i = 0; i < inStr.Length; i++) {
				char curChar = inStr[i];
				Console.WriteLine("- IsPunctuation(" + curChar + "): [" + IsPunctuation(curChar) + "]");
			}
			Console.WriteLine("-------");
			Console.WriteLine("- IsSpecifiedChar(" + inChar + ", " + inStr + "): [" + IsSpecifiedChar(inChar, inStr) + "]");
			Console.WriteLine("- IsSpecifiedChar(" + inChar2 + ", " + inStr + "): [" + IsSpecifiedChar(inChar2, inStr) + "]");
			Console.WriteLine("===== End of Unit Test =====");
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java CharUtil");
				Environment.Exit(0);
			}

			// test case and print out 
			Test();
		}
		// data member
		private static string punctuations = @"-({[)}]_!@#%&*\:;""',.?/~+=|<>$`^";
	}

}
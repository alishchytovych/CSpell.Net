using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Extensions;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.NlpUtil {
	/// <summary>
	///***************************************************************************
	/// This NLP utility class generates simplified regular inflection variants.
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
	public class InflVarsUtil {
		/// <summary>
		/// Private constructor 
		/// </summary>
		private InflVarsUtil() { }
		// public methods
		// A simplfied way to compare if two strings are inflectional varaint to 
		// each other
		public static bool IsInflectionVar(string str1, string str2) {
			bool flag = false;
			// 1. to assign base string by comparing length
			int len1 = str1.Length;
			int len2 = str2.Length;
			string baseStr = str1;
			string inflStr = str2;
			// same length, not inflectional vars, exclude irreg, such as see|saw
			if (len1 == len2) {
				return false;
			} else if (len1 > len2) { // assume the short string is the base
				baseStr = str2;
				inflStr = str1;
			}
			// check the inflections
			HashSet<string> inflSet = InflVarsUtil.GetInflVars(baseStr);
			flag = inflSet.Contains(inflStr);
			return flag;
		}
		public static HashSet<string> GetInflVarsVerb(string baseStr) {
			HashSet<string> inflVarSet = new HashSet<string>();
			string inflVar = null;
			char lastChar = GetLastChar(baseStr);
			char last2Char = GetLast2Char(baseStr);
			string lastCharStr = (new char?(lastChar)).ToString();
			string last2CharStr = (new char?(last2Char)).ToString();
			if ((baseStr.EndsWith("s", StringComparison.Ordinal)) || (baseStr.EndsWith("z", StringComparison.Ordinal)) || (baseStr.EndsWith("x", StringComparison.Ordinal)) || (baseStr.EndsWith("ch", StringComparison.Ordinal)) || (baseStr.EndsWith("sh", StringComparison.Ordinal))) {
				inflVar = baseStr + "es";
				inflVarSet.Add(inflVar);
				inflVar = baseStr + "ed";
				inflVarSet.Add(inflVar);
				inflVar = baseStr + "ing";
				inflVarSet.Add(inflVar);
			} else if (baseStr.EndsWith("ie", StringComparison.Ordinal)) {
				inflVar = baseStr + "s";
				inflVarSet.Add(inflVar);
				inflVar = baseStr + "d";
				inflVarSet.Add(inflVar);
				inflVar = baseStr.Substring(0, baseStr.Length - 2) + "ying";
				inflVarSet.Add(inflVar);
			} else if ((baseStr.EndsWith("ee", StringComparison.Ordinal)) || (baseStr.EndsWith("oe", StringComparison.Ordinal)) || (baseStr.EndsWith("ye", StringComparison.Ordinal))) {
				inflVar = baseStr + "s";
				inflVarSet.Add(inflVar);
				inflVar = baseStr + "d";
				inflVarSet.Add(inflVar);
				inflVar = baseStr + "ing";
				inflVarSet.Add(inflVar);
			} else if ((baseStr.EndsWith("y", StringComparison.Ordinal)) && (consonants_.Contains(last2CharStr))) {
				inflVar = baseStr.Substring(0, baseStr.Length - 1) + "ies";
				inflVarSet.Add(inflVar);
				inflVar = baseStr.Substring(0, baseStr.Length - 1) + "ied";
				inflVarSet.Add(inflVar);
				inflVar = baseStr.Substring(0, baseStr.Length - 1) + "ing";
				inflVarSet.Add(inflVar);
			} else if ((baseStr.EndsWith("e", StringComparison.Ordinal)) && (eioySets_.Contains(last2CharStr) == false)) {
				inflVar = baseStr + "s";
				inflVarSet.Add(inflVar);
				inflVar = baseStr + "d";
				inflVarSet.Add(inflVar);
				inflVar = baseStr + "n"; // give to given, irreg
				inflVarSet.Add(inflVar);
				inflVar = baseStr.Substring(0, baseStr.Length - 1) + "ing";
				inflVarSet.Add(inflVar);
			} else {
				inflVar = baseStr + "s";
				inflVarSet.Add(inflVar);
				inflVar = baseStr + "ed";
				inflVarSet.Add(inflVar);
				inflVar = baseStr + "ing";
				inflVarSet.Add(inflVar);
			}
			return inflVarSet;
		}
		public static HashSet<string> GetInflVarsNoun(string baseStr) {
			HashSet<string> inflVarSet = new HashSet<string>();
			string inflVar = null;
			char lastChar = GetLastChar(baseStr);
			char last2Char = GetLast2Char(baseStr);
			string lastCharStr = (new char?(lastChar)).ToString();
			string last2CharStr = (new char?(last2Char)).ToString();
			if ((baseStr.EndsWith("s", StringComparison.Ordinal)) || (baseStr.EndsWith("z", StringComparison.Ordinal)) || (baseStr.EndsWith("x", StringComparison.Ordinal)) || (baseStr.EndsWith("ch", StringComparison.Ordinal)) || (baseStr.EndsWith("sh", StringComparison.Ordinal))) {
				inflVar = baseStr + "es";
				inflVarSet.Add(inflVar);
			} else if ((baseStr.EndsWith("y", StringComparison.Ordinal)) && (consonants_.Contains(last2CharStr))) {
				inflVar = baseStr.Substring(0, baseStr.Length - 1) + "ies";
				inflVarSet.Add(inflVar);
			} else if (baseStr.EndsWith("ie", StringComparison.Ordinal)) {
				inflVar = baseStr + "s";
				inflVarSet.Add(inflVar);
			} else {
				inflVar = baseStr + "s";
				inflVarSet.Add(inflVar);
			}
			return inflVarSet;
		}
		public static HashSet<string> GetInflVarsAdj(string baseStr) {
			HashSet<string> inflVarSet = new HashSet<string>();
			string inflVar = null;
			char lastChar = GetLastChar(baseStr);
			char last2Char = GetLast2Char(baseStr);
			string last2CharStr = (new char?(last2Char)).ToString();
			if ((lastChar == 'y') && (consonants_.Contains(last2CharStr))) {
				inflVar = baseStr.Substring(0, baseStr.Length - 1) + "ier";
				inflVarSet.Add(inflVar);
				inflVar = baseStr.Substring(0, baseStr.Length - 1) + "iest";
				inflVarSet.Add(inflVar);
			} else if (lastChar == 'e') {
				inflVar = baseStr + "r";
				inflVarSet.Add(inflVar);
				inflVar = baseStr + "st";
				inflVarSet.Add(inflVar);
			} else {
				inflVar = baseStr + "er";
				inflVarSet.Add(inflVar);
				inflVar = baseStr + "est";
				inflVarSet.Add(inflVar);
			}
			return inflVarSet;
		}
		// get all possible regular inflVars for a base 
		public static HashSet<string> GetInflVars(string baseStr) {
			HashSet<string> inflVarSet = GetInflVarsAdj(baseStr);
			inflVarSet.addAll(GetInflVarsNoun(baseStr));
			inflVarSet.addAll(GetInflVarsVerb(baseStr));
			return inflVarSet;
		}
		// get all possible base
		private static char GetLastChar(string inStr) {
			int length = inStr.Length;
			char @out = inStr.ToLower() [length - 1];
			return @out;
		}
		private static char GetLast2Char(string inStr) {
			char @out = ' '; // an empty character
			int length = inStr.Length;
			if (length >= 2) {
				@out = inStr.ToLower() [length - 2];
			}
			return @out;
		}
		// privage methods
		private static void Test(string inStr) {
			Console.WriteLine("-------------------------------------");
			HashSet<string> inflVarSetN = GetInflVarsNoun(inStr);
			Console.WriteLine("- noun(" + inStr + "): " + inflVarSetN);
			HashSet<string> inflVarSetV = GetInflVarsVerb(inStr);
			Console.WriteLine("- verb(" + inStr + "): " + inflVarSetV);
			HashSet<string> inflVarSetA = GetInflVarsAdj(inStr);
			Console.WriteLine("- adj(" + inStr + "): " + inflVarSetA);
			HashSet<string> inflVarSet = GetInflVars(inStr);
			Console.WriteLine("- All(" + inStr + "): " + inflVarSet);
		}
		private static void Tests() {
			Console.WriteLine("===== Unit Test of InflVarsUtil =====");
			Test("study");
			Test("test");
			Test("tie");
			Test("church");
			Console.WriteLine("===== End of Unit Test =====");
		}
		private static void TestIsInflVars() {
			Console.WriteLine("age|aged|" + IsInflectionVar("age", "aged"));
			Console.WriteLine("aged|age|" + IsInflectionVar("aged", "age"));
			Console.WriteLine("give|given|" + IsInflectionVar("give", "given"));
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java InflVarsUtil");
				Environment.Exit(0);
			}

			// test case and print out 
			Tests();
			TestIsInflVars();
		}
		// data member
		private static HashSet<string> vowels_ = new HashSet<string>();
		private static HashSet<string> eioySets_ = new HashSet<string>();
		private static HashSet<string> consonants_ = new HashSet<string>();
		static InflVarsUtil() {
			vowels_.Add("a");
			vowels_.Add("e");
			vowels_.Add("i");
			vowels_.Add("o");
			vowels_.Add("u");
			eioySets_.Add("e");
			eioySets_.Add("i");
			eioySets_.Add("o");
			eioySets_.Add("y");
			consonants_.Add("b");
			consonants_.Add("c");
			consonants_.Add("d");
			consonants_.Add("f");
			consonants_.Add("g");
			consonants_.Add("h");
			consonants_.Add("j");
			consonants_.Add("k");
			consonants_.Add("l");
			consonants_.Add("m");
			consonants_.Add("n");
			consonants_.Add("p");
			consonants_.Add("q");
			consonants_.Add("r");
			consonants_.Add("s");
			consonants_.Add("t");
			consonants_.Add("v");
			consonants_.Add("w");
			consonants_.Add("x");
			consonants_.Add("y");
			consonants_.Add("z");
		}
	}

}
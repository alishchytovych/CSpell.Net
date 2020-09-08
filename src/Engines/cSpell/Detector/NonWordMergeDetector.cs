using System;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Detector {
	/// <summary>
	///***************************************************************************
	/// This class is to detect a non-word for non-word merge correction.
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
	public class NonWordMergeDetector {
		// private constructor
		private NonWordMergeDetector() { }

		// public methods
		// rmEndPuncStr is include me gre.
		public static bool IsDetect(string inWord, string rmEndPuncStr, CSpellApi cSpellApi, bool debugFlag) {
			// both inWord and rmEndPunc are non-word merge
			bool nonWordMergeFlag = IsNonWordMerge(inWord, cSpellApi, debugFlag) && IsNonWordMerge(rmEndPuncStr, cSpellApi, debugFlag);
			return nonWordMergeFlag;
		}
		public static bool IsNonWordMerge(string inWord, CSpellApi cSpellApi, bool debugFlag) {
			// init
			RootDictionary checkDic = cSpellApi.GetSplitWordDic(); // merge Dic
			RootDictionary unitDic = cSpellApi.GetUnitDic();
			// non-word merge must be:
			// 1. not known in the dictionary
			// 2. not exception, such as url, email, digit, ...
			// => if excpetion, even is a non-word, still not a misspelt
			bool nonWordMergeFlag = (!checkDic.IsValidWord(inWord)) && (!IsNonWordMergeExceptions(inWord, unitDic));
			// print out debug
			if (debugFlag == true) {
				bool wordDicFlag = checkDic.IsValidWord(inWord);
				bool wordExceptionFlag = IsNonWordMergeExceptions(inWord, unitDic);
				DebugPrint.PrintNwMergeDetect(inWord, nonWordMergeFlag, wordDicFlag, wordExceptionFlag, debugFlag);
			}
			return nonWordMergeFlag;
		}
		// TBD: remove svDic, pnDic, aaDic
		// Valid Exceptions: valid English words, but not in the dictionary.
		// Such as digit, punc, digitPunc (no letter), Url, eMail
		// measurement, unit, abbreviation, acronym, proper noun, sp vars
		private static bool IsNonWordMergeExceptions(string inWord, RootDictionary unitDic) {
			bool exceptionFlag = (DigitPuncTokenUtil.IsDigit(inWord) == true) || (DigitPuncTokenUtil.IsPunc(inWord) == true) || (DigitPuncTokenUtil.IsDigitPunc(inWord) == true) || (InternetTokenUtil.IsUrl(inWord) == true) || (InternetTokenUtil.IsEmail(inWord) == true) || (IsEmptyString(inWord) == true) || (IsSingleCharString(inWord) == true) || (IsUpperCaseString(inWord) == true) || (MeasurementTokenUtil.IsMeasurements(inWord, unitDic) == true);
			return exceptionFlag;
		}
		// should be moved to Util
		private static bool IsUpperCaseString(string inWord) {
			string inWordUc = inWord.ToUpper();
			return (inWord.Equals(inWordUc));
		}
		private static bool IsEmptyString(string inWord) {
			return (inWord.Trim().Length == 0);
		}
		private static bool IsSingleCharString(string inWord) {
			return (inWord.Trim().Length == 1);
		}
		// private
		private static void Tests(CSpellApi cSpellApi) {
			// test Words
			Test("ment.", cSpellApi);
			Test("123", cSpellApi);
		}
		private static void Test(string inWord, CSpellApi cSpellApi) {
			bool debugFlag = false;
			Console.WriteLine("[" + inWord + "]: " + IsNonWordMerge(inWord, cSpellApi, debugFlag));
		}

		// public test driver
		public static void MainTest(string[] args) {
			string configFile = "../data/Config/cSpell.properties";
			if (args.Length == 1) {
				configFile = args[0];
			}
			if (args.Length > 0) {
				Console.Error.WriteLine("Usage: java NonWordMergeDetector <config>");
				Environment.Exit(1);
			}
			// init, read in from config
			CSpellApi cSpellApi = new CSpellApi(configFile);
			// Test
			Tests(cSpellApi);
		}
		// data member
	}

}
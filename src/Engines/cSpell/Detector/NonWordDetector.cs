using System;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Detector {
	/// <summary>
	///***************************************************************************
	/// This class is to detect a non-word for non-word split or 1To1 correction.
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
	public class NonWordDetector {
		// private constructor
		private NonWordDetector() { }

		// public methods
		// Is spelling error, error word candidates, need to be corrected
		// - not in the checking dictionary
		// - not one of expcetion, such as URl, measurement, ...
		public static bool IsDetect(string inWord, CSpellApi cSpellApi) {
			bool debugFlag = false;
			return IsDetect(inWord, cSpellApi, debugFlag);
		}
		public static bool IsDetect(string inWord, CSpellApi cSpellApi, bool debugFlag) {
			return IsNonWord(inWord, cSpellApi, debugFlag);
		}
		// check both the inWord and coreStr of inWord
		// the coreStr can be derived from CoreTermObj.GetCoreTerm();
		public static bool IsDetect(string inWord, string coreStr, CSpellApi cSpellApi) {
			bool debugFlag = false;
			return IsDetect(inWord, coreStr, cSpellApi, debugFlag);
		}
		// Check both inWord and coreTerm
		public static bool IsDetect(string inWord, string coreStr, CSpellApi cSpellApi, bool debugFlag) {
			bool nonWordFlag = IsNonWord(inWord, cSpellApi, debugFlag) && IsNonWord(coreStr, cSpellApi, debugFlag);
			return nonWordFlag;
		}
		public static bool IsNonWord(string inWord, CSpellApi cSpellApi, bool debugFlag) {
			// init
			RootDictionary checkDic = cSpellApi.GetCheckDic();
			RootDictionary unitDic = cSpellApi.GetUnitDic();
			// non-word must be:
			// 1. not known in the dictionary
			// 2. not exception, such as url, email, digit, ...
			// => if excpetion, even is a nor-word, still not a misspelt
			bool nonWordFlag = (!checkDic.IsValidWord(inWord)) && (!IsNonWordExceptions(inWord, unitDic));
			if (debugFlag == true) {
				bool wordDicFlag = checkDic.IsValidWord(inWord);
				bool wordExceptionFlag = IsNonWordExceptions(inWord, unitDic);
				DebugPrint.PrintNwDetect(inWord, nonWordFlag, wordDicFlag, wordExceptionFlag, debugFlag);
			}
			return nonWordFlag;
		}
		// TBD: remove pnDic, aaDic
		// Valid Exceptions: valid English words, but not in the dictionary.
		// Such as digit, punc, digitPunc (no letter), Url, eMail
		// measurement, unit, 
		// abbreviation, acronym, proper nouns: do not change the F1 after test
		private static bool IsNonWordExceptions(string inWord, RootDictionary unitDic) {
			bool validExceptionFlag = (DigitPuncTokenUtil.IsDigit(inWord) == true) || (DigitPuncTokenUtil.IsPunc(inWord) == true) || (DigitPuncTokenUtil.IsDigitPunc(inWord) == true) || (InternetTokenUtil.IsUrl(inWord) == true) || (InternetTokenUtil.IsEmail(inWord) == true) || (IsEmptyString(inWord) == true) || (MeasurementTokenUtil.IsMeasurements(inWord, unitDic) == true);
			return validExceptionFlag;
		}
		private static bool IsAbbAcr(string inWord, RootDictionary aaDic) {
			// Check abbreviation and acronym from Lexicon, case sensitive
			bool aaFlag = aaDic.IsDicWord(inWord);
			return aaFlag;
		}
		private static bool IsProperNoun(string inWord, RootDictionary pnDic) {
			// Check proper noun from Lexicon, case sensitive
			bool pnFlag = pnDic.IsDicWord(inWord);
			return pnFlag;
		}
		private static bool IsEmptyString(string inWord) {
			return (inWord.Trim().Length == 0);
		}
		// private
		private static void Tests(CSpellApi cSpellApi) {
			// test Words
			Test("test123", cSpellApi);
			Test("123", cSpellApi); // digit
			Test("@#$", cSpellApi);
			Test("123@#$", cSpellApi);
			Test("123.34", cSpellApi);
			Test("@#%123", cSpellApi);
			Test("12.3%", cSpellApi); // digit
			Test("-0.5mg", cSpellApi);
			Test("70kg", cSpellApi); //measurement
			Test("cm3", cSpellApi); //measurement: include pure unit
			Test("Gbit", cSpellApi); //measurement: include pure unit
			Test("12cm", cSpellApi);
			Test("30mg/50kg", cSpellApi);
			Test("45Chris", cSpellApi);
			Test("http://www.amia.org", cSpellApi); // url
			Test("jeff@amia.org", cSpellApi); // email
			Test("dur", cSpellApi); // test for merge
			Test("dur", cSpellApi); // test for merge
			Test("ing", cSpellApi); // test for merge
			Test("ing", cSpellApi); // test for merge
			Test("eighteen", cSpellApi); // test for merge
			Test("eighteen", cSpellApi); // test for merge
			Test("Ilost", cSpellApi); // test for split
		}
		private static void Test(string inWord, CSpellApi cSpellApi) {
			bool debugFlag = false;
			Console.WriteLine("[" + inWord + "]: " + IsDetect(inWord, cSpellApi, debugFlag));
		}

		// public test driver
		public static void MainTest(string[] args) {
			string configFile = "../data/Config/cSpell.properties";
			if (args.Length == 1) {
				configFile = args[0];
			}
			if (args.Length > 0) {
				Console.Error.WriteLine("Usage: java NonWordDetector <config>");
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
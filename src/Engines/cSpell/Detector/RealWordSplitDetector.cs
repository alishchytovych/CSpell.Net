using System;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Ranker;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Detector {
	/// <summary>
	///***************************************************************************
	/// This class is to detect a real-word for real-word split correction. 
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
	public class RealWordSplitDetector {
		// private constructor
		private RealWordSplitDetector() { }

		// public methods
		public static bool IsDetect(string inWord, CSpellApi cSpellApi) {
			bool debugFlag = false;
			return IsRealWord(inWord, cSpellApi, debugFlag);
		}
		public static bool IsDetect(string inWord, CSpellApi cSpellApi, bool debugFlag) {
			return IsRealWord(inWord, cSpellApi, debugFlag);
		}
		// check both the inWord and coreStr of inWord
		// the coreStr can be derived from CoreTermObj.GetCoreTerm();
		public static bool IsDetect(string inWord, string coreStr, CSpellApi cSpellApi) {
			bool debugFlag = false;
			return IsDetect(inWord, coreStr, cSpellApi, debugFlag);
		}
		// check both inWord and coreTerm, either them is a real-word
		public static bool IsDetect(string inWord, string coreStr, CSpellApi cSpellApi, bool debugFlag) {
			bool validFlag = IsRealWord(inWord, cSpellApi, debugFlag) || IsRealWord(coreStr, cSpellApi, debugFlag);
			return validFlag;
		}
		public static bool IsRealWord(string inWord, CSpellApi cSpellApi, bool debugFlag) {
			// init
			RootDictionary checkDic = cSpellApi.GetCheckDic();
			RootDictionary unitDic = cSpellApi.GetUnitDic();
			WordWcMap wordWcMap = cSpellApi.GetWordWcMap();
			Word2Vec word2VecOm = cSpellApi.GetWord2VecOm();
			int inWordLen = inWord.Length;
			// TBD, change method name
			int rwSplitWordMinLength = cSpellApi.GetDetectorRwSplitWordMinLength();
			int rwSplitWordMinWc = cSpellApi.GetDetectorRwSplitWordMinWc();
			// realword must be:
			// 1. known in the dictionary
			// 2. not exception, such as url, email, digit, ...
			// => if excpetion, even is a non-word, no correction
			// 3. must have word2Vector value (inWord is auto converted to LC)
			// 4. frequency must be above a threshhold (inWord is auto to LC)
			// TBD, need to be configureable 200 
			bool realWordFlag = (checkDic.IsValidWord(inWord)) && (!IsRealWordExceptions(inWord, unitDic) && (inWordLen >= rwSplitWordMinLength) && (word2VecOm.HasWordVec(inWord) == true) && (WordCountScore.GetWc(inWord, wordWcMap) >= rwSplitWordMinWc));
			if (debugFlag == true) {
				bool wordInDicFlag = checkDic.IsValidWord(inWord);
				bool wordExceptionFlag = IsRealWordExceptions(inWord, unitDic);
				bool lengthFlag = (inWordLen >= rwSplitWordMinLength);
				bool word2VecFlag = word2VecOm.HasWordVec(inWord);
				bool wcFlag = (WordCountScore.GetWc(inWord, wordWcMap) >= rwSplitWordMinWc);
				DebugPrint.PrintRwSplitDetect(inWord, realWordFlag, wordInDicFlag, wordExceptionFlag, lengthFlag, word2VecFlag, wcFlag, debugFlag);
			}
			return realWordFlag;
		}
		// Valid Exceptions: valid English words, but not in the dictionary.
		// Such as digit, punc, digitPunc (no letter), Url, eMail
		// measurement, unit, abbreviation, acronym, proper noun, sp vars
		private static bool IsRealWordExceptions(string inWord, RootDictionary unitDic) {
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
			Test("-http://www.ncbi.nlm.nih.gov/sites/ga?disorder=Androgen%20Insensitivity%20Syndrome", cSpellApi); // test for split
			Test("http://www.ncbi.nlm.nih.gov/sites/ga?disorder=Androgen%20Insensitivity%20Syndrome", cSpellApi); // test for split
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
				Console.Error.WriteLine("Usage: java RealWordSplitDetector <config>");
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
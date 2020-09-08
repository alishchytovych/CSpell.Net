using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Ranker;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Detector {
	/// <summary>
	///***************************************************************************
	/// This class is to detect a real-word for real-word 1To1 correction. 
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
	public class RealWord1To1Detector {
		// private constructor
		private RealWord1To1Detector() { }

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
		// core process for detect rewal-word 1-to-1 
		public static bool IsRealWord(string inWord, CSpellApi cSpellApi, bool debugFlag) {
			// init
			RootDictionary checkDic = cSpellApi.GetCheckDic();
			RootDictionary pnDic = cSpellApi.GetPnDic();
			RootDictionary aaDic = cSpellApi.GetAaDic();
			RootDictionary unitDic = cSpellApi.GetUnitDic();
			int inWordLen = inWord.Length;
			string inWordLc = inWord.ToLower(); // no need, TBD
			WordWcMap wordWcMap = cSpellApi.GetWordWcMap();
			Word2Vec word2VecOm = cSpellApi.GetWord2VecOm();
			int rw1To1WordMinLength = cSpellApi.GetDetectorRw1To1WordMinLength();
			int rw1To1WordMinWc = cSpellApi.GetDetectorRw1To1WordMinWc();
			// realword 1-to-1 must be:
			// 1. known in the dictionary
			// 2. not exception, such as url, email, digit, ...
			// => if excpetion, even is a non-word, no correction
			// 3. must have word2Vector value (inWord is auto converted to LC)
			// 4. frequency must be above a threshhold (inWord is auto to LC)
			// TBD, need to be configureable, 3 and 65
			bool realWordFlag = (checkDic.IsValidWord(inWord)) && (!IsRealWordExceptions(inWord, pnDic, aaDic, unitDic)) && (inWordLen >= rw1To1WordMinLength) && (word2VecOm.HasWordVec(inWord) == true) && (WordCountScore.GetWc(inWord, wordWcMap) >= rw1To1WordMinWc);
			if (debugFlag == true) {
				bool wordInDicFlag = checkDic.IsValidWord(inWord);
				bool wordExceptionFlag = IsRealWordExceptions(inWord, pnDic, aaDic, unitDic);
				bool legnthFlag = (inWordLen >= rw1To1WordMinLength);
				bool word2VecFlag = word2VecOm.HasWordVec(inWord);
				bool wcFlag = (WordCountScore.GetWc(inWord, wordWcMap) >= rw1To1WordMinWc);
				DebugPrint.PrintRw1To1Detect(inWord, realWordFlag, wordInDicFlag, wordExceptionFlag, legnthFlag, word2VecFlag, wcFlag, debugFlag);
			}
			return realWordFlag;
		}
		// Valid Exceptions: valid English words, but not in the dictionary.
		// Such as digit, punc, digitPunc (no letter), Url, eMail
		// measurement, unit, abbreviation, acronym, proper noun, sp vars
		private static bool IsRealWordExceptions(string inWord, RootDictionary pnDic, RootDictionary aaDic, RootDictionary unitDic) {
			bool validExceptionFlag = (DigitPuncTokenUtil.IsDigit(inWord) == true) || (DigitPuncTokenUtil.IsPunc(inWord) == true) || (DigitPuncTokenUtil.IsDigitPunc(inWord) == true) || (InternetTokenUtil.IsUrl(inWord) == true) || (InternetTokenUtil.IsEmail(inWord) == true) || (IsEmptyString(inWord) == true) || (MeasurementTokenUtil.IsMeasurements(inWord, unitDic) == true) || (IsProperNoun(inWord, pnDic) == true) || (IsAbbAcr(inWord, aaDic) == true);
			return validExceptionFlag;
		}
		private static bool IsSpVar(string inWord, RootDictionary svDic) {
			// Check spVar from Lexicon, case sensitive
			bool svFlag = svDic.IsDicWord(inWord);
			return svFlag;
		}
		private static bool IsAbbAcr(string inWord, RootDictionary aaDic) {
			// Check abbreviation and acronym from Lexicon, case sensitive
			// should be case sensitive, but here, we implmented aggresive match
			bool aaFlag = aaDic.IsDicWord(inWord);
			return aaFlag;
		}
		// check if it is a prperNoun
		private static bool IsProperNoun(string inWord, RootDictionary pnDic) {
			// Check proper noun from Lexicon, case sensitive
			// should be case sensitive, but here, we implmented aggresive match
			// and ignroe the case
			bool pnFlag = pnDic.IsDicWord(inWord);
			return pnFlag;
		}
		private static bool IsEmptyString(string inWord) {
			return (inWord.Trim().Length == 0);
		}
		// private
		private static void Tests(CSpellApi cSpellApi) {
			List<string> testList = new List<string>();
			testList.Add("it");
			testList.Add("ORIGEN");
			testList.Add("haberman");
			testList.Add("robins");
			testList.Add("guys");
			testList.Add("its");
			testList.Add("too");
			testList.Add("Cyra");
			testList.Add("its");
			testList.Add("then");
			testList.Add("anderson");
			testList.Add("stereotypy");
			testList.Add("thing");
			testList.Add("multi");
			testList.Add("Noonan's");
			testList.Add("gardeners");
			testList.Add("mg");
			testList.Add("if");
			testList.Add("sisters");
			testList.Add("husbands");
			testList.Add("if");
			testList.Add("fathers");
			testList.Add("kg");
			testList.Add("know");
			testList.Add("tried");
			testList.Add("to");
			testList.Add("to");
			testList.Add("canyon");
			testList.Add("specially");
			testList.Add("law");
			testList.Add("domestic");
			testList.Add("repot");
			testList.Add("Weather");
			testList.Add("there");
			testList.Add("then");
			testList.Add("fine");
			testList.Add("m");
			testList.Add("Siemens");
			testList.Add("month's");
			testList.Add("medication's");
			testList.Add("quantity's");
			testList.Add("undisguised");
			testList.Add("bowl");
			testList.Add("its");
			testList.Add("its");
			testList.Add("ti");
			testList.Add("i");
			testList.Add("off");
			testList.Add("Dies");
			testList.Add("therefor");
			testList.Add("hank");
			testList.Add("adema");
			testList.Add("lesson");
			testList.Add("adema");
			testList.Add("ajd");
			testList.Add("are");
			testList.Add("are");
			testList.Add("vs,");
			testList.Add("wont");
			testList.Add("its");
			testList.Add("descended");
			testList.Add("nt");
			testList.Add("dose");
			testList.Add("Its");
			testList.Add("do");
			testList.Add("our");
			testList.Add("in");
			testList.Add("effect");
			testList.Add("pregnancy");
			testList.Add("cn");
			testList.Add("d");
			testList.Add("leave");
			testList.Add("BB");
			testList.Add("gastrologist");
			testList.Add("lounger");
			testList.Add("its");
			testList.Add("wat");
			testList.Add("d");
			testList.Add("av");
			testList.Add("AV");
			testList.Add("relaxers");
			testList.Add("day's");
			testList.Add("nd");
			testList.Add("wat");
			testList.Add("the");
			testList.Add("Aisa");
			testList.Add("wat");
			testList.Add("dnt");
			testList.Add("affects");
			testList.Add("their");
			testList.Add("you");
			testList.Add("Ito");
			testList.Add("prego");
			testList.Add("medical");
			testList.Add("hoe");
			testList.Add("hoe");
			testList.Add("medical");
			testList.Add("hoe");
			testList.Add("an");
			testList.Add("swimmers");
			testList.Add("swollen");
			testList.Add("swollen");
			testList.Add("William's");
			testList.Add("Williams'");
			testList.Add("tent");
			testList.Add("its");
			testList.Add("well");
			testList.Add("lanais");
			testList.Add("FRIENDS");
			testList.Add("access");
			testList.Add("rply");
			testList.Add("bed");
			testList.Add("tiered");
			testList.Add("its");
			testList.Add("loosing");
			testList.Add("loosing");
			testList.Add("can");
			testList.Add("physician's");
			testList.Add("to");
			testList.Add("where");
			testList.Add("twine");
			testList.Add("aim");
			testList.Add("lease");
			testList.Add("4");
			testList.Add("spot");
			testList.Add("weather");
			testList.Add("its");
			testList.Add("is");
			testList.Add("devises");
			testList.Add("were");
			testList.Add("versa");
			testList.Add("Gud");
			testList.Add("doner");
			testList.Add("MT");
			testList.Add("ITS");
			testList.Add("small");
			testList.Add("bond");
			testList.Add("hav");
			testList.Add("abd");
			testList.Add("do");
			testList.Add("sever");
			testList.Add("then");
			testList.Add("leave");
			testList.Add("meningitidis");
			testList.Add("gey");
			testList.Add("its");
			testList.Add("hav");
			testList.Add("Mam");
			testList.Add("vertebras");
			testList.Add("amd");
			// test 
			int detectNo = 0;
			int notDetectNo = 0;
			foreach (string testStr in testList) {
				if (Test(testStr, cSpellApi) == true) {
					Console.WriteLine(testStr);
					detectNo++;
				} else {
					notDetectNo++;
				}
			}
			Console.WriteLine(testList.Count + "|" + detectNo + "|" + notDetectNo);
		}
		private static bool Test(string inWord, CSpellApi cSpellApi) {
			bool debugFlag = false;
			string inWordLc = inWord.ToLower();
			bool isDetect = IsDetect(inWordLc, cSpellApi, debugFlag);
			return isDetect;
		}

		// public test driver
		public static void MainTest(string[] args) {
			string configFile = "../data/Config/cSpell.properties";
			if (args.Length == 1) {
				configFile = args[0];
			}
			if (args.Length > 0) {
				Console.Error.WriteLine("Usage: java RealWord1To1Detector <config>");
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
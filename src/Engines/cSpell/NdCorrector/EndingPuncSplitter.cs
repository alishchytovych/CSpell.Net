using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.NdCorrector {

	/// <summary>
	///***************************************************************************
	/// This class is the ending punctuation splitter, by adding space before them.
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
	public class EndingPuncSplitter {
		// private constructor
		private EndingPuncSplitter() { }

		// recursively process
		public static TokenObj Process(TokenObj inTokenObj, int maxProcess) {
			bool debugFlag = false;
			return Process(inTokenObj, maxProcess, debugFlag);
		}
		public static TokenObj Process(TokenObj inTokenObj, int maxProcess, bool debugFlag) {
			string inTokenStr = inTokenObj.GetTokenStr();
			string outTokenStr = Process(inTokenStr, maxProcess);
			TokenObj outTokenObj = new TokenObj(inTokenObj);
			//update info if there is a process
			if (inTokenStr.Equals(outTokenStr) == false) {
				outTokenObj.SetTokenStr(outTokenStr);
				outTokenObj.AddProcToHist(TokenObj.HIST_ND_S_E_P);
				DebugPrint.PrintCorrect("ND", "EndingPuncSplitter", inTokenStr, outTokenStr, debugFlag);
			}
			return outTokenObj;
		}
		public static TokenObj Process(TokenObj inTokenObj) {
			string inTokenStr = inTokenObj.GetTokenStr();
			string outTokenStr = Process(inTokenStr);
			TokenObj outTokenObj = new TokenObj(inTokenObj);
			//update info if there is a process
			if (inTokenStr.Equals(outTokenStr) == false) {
				outTokenObj.SetTokenStr(outTokenStr);
				outTokenObj.AddProcToHist(TokenObj.HIST_ND_S_E_P);
			}
			return outTokenObj;
		}
		// recursively process
		public static string Process(string inWord, int maxProcess) {
			string lastText = inWord;
			string outText = Process(inWord);
			while ((maxProcess > 0) && (outText.Equals(lastText) == false)) {
				// recusively process
				lastText = outText;
				// converts to textObj for recursively process
				TextObj textObj = new TextObj(lastText);
				List<TokenObj> inTokenList = textObj.GetTokenList();
				List<TokenObj> outTokenList = new List<TokenObj>(inTokenList.Select(tokenObj => tokenObj.GetTokenStr()).Select(tokenStr => Process(tokenStr)).Select(outStr => new TokenObj(outStr)).ToList());

				outText = TextObj.TokenListToText(outTokenList);
				maxProcess--;
			}
			return outText;
		}
		/// <summary>
		/// This method splits the input word by adding a space after ending 
		/// punctuation.  The input must be single word (no space).
		/// The process method splits the inWord by adding space(s) after endingPunc.
		/// Current algorithm can only handle max. up to 3 endignPuncs.
		/// One in each component of coreTermObj: coreTerm, prefix, and suffix.
		/// - prefix: leading str with punc|spac|number
		/// - coreterm: = the original str - prefix - suffix
		/// - suffix: ending str with punc|space|number
		/// This can be improved by using recursive algorithm in the coreTerm.
		/// For example: "ankle,before.The" in 15737.txt will be split twice in 
		/// recursive algorithm.
		/// </summary>
		/// <param name="inWord">  the input token (single word)
		/// </param>
		/// <returns>   the splited word. </returns>
		public static string Process(string inWord) {
			string outWord = inWord;
			bool debugFlag = false;
			// eProcess: check if can skip
			int ctType = CoreTermUtil.CT_TYPE_SPACE_PUNC_DIGIT;
			if (IsQualified(inWord) == true) {
				// 0. convert to coreTerm object
				bool splitFlag = false;
				CoreTermObj cto = new CoreTermObj(inWord, ctType);
				// 1. update coreTerm
				string inCoreTerm = cto.GetCoreTerm();
				string lastEndingPunc = FindLastEndingPunc(inCoreTerm);
				// add a space after the last endingPunc
				if (!string.ReferenceEquals(lastEndingPunc, null)) {
					// get the splitObj and then the split string
					string outCoreTerm = EndingPunc.GetSplitStr(inCoreTerm, lastEndingPunc);
					cto.SetCoreTerm(outCoreTerm);
					splitFlag = true;
				}
				// 2. update the prefix when it ends with a endingPunc
				// prefix contains punc and numbers
				string prefix = cto.GetPrefix();
				if ((prefix.Length != 0) && (EndsWithEndingPunc(prefix) == true)) { // ends with endingPunc
					prefix = prefix + GlobalVars.SPACE_STR;
					cto.SetPrefix(prefix);
					splitFlag = true;
				}
				// 3. update the suffix and add a space after the last endingPunc
				// suffix contians punctuation and numbers
				string suffix = cto.GetSuffix();
				if ((suffix.Length != 0) && (ContainsEndingPunc(suffix) == true) && (IsPureEndingPunc(suffix) == false)) { // can't be pure endingPuncs
					// add space after the last endingPunc
					string lastEndingPunc2 = FindLastEndingPunc(suffix);
					if (!string.ReferenceEquals(lastEndingPunc2, null)) {
						// get the splitObj and then the split string
						string outSuffix = EndingPunc.GetSplitStr(suffix, lastEndingPunc2);
						cto.SetSuffix(outSuffix);
						splitFlag = true;
					}
				}
				// update outWord
				if (splitFlag == true) {
					outWord = cto.ToString();
				}
			}
			return outWord;
		}
		// check if the token can be skipped
		private static string FindFirstEndingPunc(string inWord) {
			int minFirstIndex = int.MaxValue;
			string firstEndingPunc = null;
			foreach (string endingPunc in endingPuncList_) {
				int firstIndex = inWord.IndexOf(endingPunc, StringComparison.Ordinal);
				if ((firstIndex != -1) && (firstIndex < minFirstIndex)) {
					firstEndingPunc = endingPunc;
					minFirstIndex = firstIndex;
				}
			}
			return firstEndingPunc;
		}
		private static string FindLastEndingPunc(string inWord) {
			int maxLastIndex = -1;
			string lastEndingPunc = null;
			foreach (string endingPunc in endingPuncList_) {
				int lastIndex = inWord.LastIndexOf(endingPunc, StringComparison.Ordinal);
				if (lastIndex > maxLastIndex) {
					lastEndingPunc = endingPunc;
					maxLastIndex = lastIndex;
				}
			}
			return lastEndingPunc;
		}
		// broader matcher
		private static bool IsQualified(string inWord) {
			bool qFlag = false;
			// use coreTerm for URL and eMail
			int ctType = CoreTermUtil.CT_TYPE_SPACE_PUNC_DIGIT;
			CoreTermObj cto = new CoreTermObj(inWord, ctType);
			string inCoreTerm = cto.GetCoreTerm();
			// check if pass the matcher to be qualified
			if ((ContainsEndingPunc(inWord) == true) && (InternetTokenUtil.IsEmail(inCoreTerm) == false) && (InternetTokenUtil.IsUrl(inCoreTerm) == false) && (DigitPuncTokenUtil.IsDigitPunc(inWord) == false)) { //skip if digitPunc
				qFlag = true;
			}
			return qFlag;
		}
		private static bool IsPureEndingPunc(string inWord) {
			var matcher = patternEP_.Match(inWord);
			return matcher.Success;
		}
		private static bool LeadsWithEndingPunc(string inWord) {
			var matcher = patternLEP_.Match(inWord);
			return matcher.Success;
		}
		private static bool EndsWithEndingPunc(string inWord) {
			var matcher = patternEEP_.Match(inWord);
			return matcher.Success;
		}
		private static bool ContainsEndingPunc(string inWord) {
			var matcher = patternCEP_.Match(inWord);
			return matcher.Success;
		}
		// test driver
		private static void TestProcessWord() {
			Console.WriteLine("----- Test Process Word: -----");
			List<string> inWordList = new List<string>();
			inWordList.Add("ankle,before.The"); // 15737.txt
			inWordList.Add(",before.the");
			inWordList.Add("before.the");
			inWordList.Add("Dr.[NAME]");
			inWordList.Add("&quot;");
			inWordList.Add("&lt;");
			inWordList.Add("Dr.s");
			inWordList.Add("i.e.,");
			inWordList.Add("Help...?");
			inWordList.Add("SS!.");
			inWordList.Add("male.read");
			inWordList.Add("operation.,");
			inWordList.Add("operation.,he");
			inWordList.Add("Test.?");
			inWordList.Add("Test.\"");
			inWordList.Add("clinicaltrials.gov");
			inWordList.Add("trombolysis..?");
			inWordList.Add("i.e.?");
			inWordList.Add(").");
			inWordList.Add(".)");
			inWordList.Add(".?");
			inWordList.Add("50,000");
			inWordList.Add("(50,000");
			inWordList.Add("<[CONTACT]>.");
			inWordList.Add("<[CONTACT]>");
			inWordList.Add("you,[NAME]");
			inWordList.Add("doctors.Thanks,");
			inWordList.Add("http://www.ncbi.nlm.nih.gov/sites/ga?disorder=androgen%20insensitivity%20syndrome");
			inWordList.Add("-http://www.ncbi.nlm.nih.gov/sites/ga?disorder=androgen%20insensitivity%20syndrome");
			inWordList.Add("1q21.1"); //13.txt
			inWordList.Add("16P-13.11"); // 7.txt
			inWordList.Add("ulcers?'"); // .txt
			inWordList.Add("ulcers!]"); // .txt
			inWordList.Add("R&D"); // .txt
			inWordList.Add("Research&development"); // .txt
			/// <summary>
			/// inWordList.add("times).");
			/// inWordList.add("123.234.456");
			/// inWordList.add("xxx(2)-yyy");
			/// inWordList.add("12:34");
			/// inWordList.add("Test.I");
			/// inWordList.add("Test...123");
			/// 
			/// </summary>
			int MaxRecursive = 5;
			foreach (string inWord in inWordList) {
				Console.WriteLine("- Process(" + inWord + "): " + Process(inWord, MaxRecursive));
			}
		}
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java EndingPuncSplitter");
				Environment.Exit(0);
			}

			TestProcessWord();
		}
		// data member
		// pure ending punctuation: add > 
		private const string patternStrEP_ = "^[\\.\\?!,;:&\\)\\]\\}>]*$";
		private static readonly Regex patternEP_ = new JRegex(patternStrEP_, RegexOptions.Compiled);
		// contains ending punctuation 
		private const string patternStrCEP_ = "^.*[\\.\\?!,;:&\\)\\]\\}].*$";
		private static readonly Regex patternCEP_ = new JRegex(patternStrCEP_, RegexOptions.Compiled);
		// leads with ending punctuation, must have more than 1 chars 
		private const string patternStrLEP_ = "^[\\.\\?!,;:&\\)\\]\\}].+$";
		private static readonly Regex patternLEP_ = new JRegex(patternStrLEP_, RegexOptions.Compiled);
		// ends with ending punctuation 
		private const string patternStrEEP_ = "^.*[\\.\\?!,;:&\\)\\]\\}]$";
		private static readonly Regex patternEEP_ = new JRegex(patternStrEEP_, RegexOptions.Compiled);
		private static readonly List<string> endingPuncList_ = new List<string>();
		static EndingPuncSplitter() {
			endingPuncList_.Add(EndingPunc.ENDING_P);
			endingPuncList_.Add(EndingPunc.ENDING_QM);
			endingPuncList_.Add(EndingPunc.ENDING_EM);
			endingPuncList_.Add(EndingPunc.ENDING_CA);
			endingPuncList_.Add(EndingPunc.ENDING_SC);
			endingPuncList_.Add(EndingPunc.ENDING_CL);
			endingPuncList_.Add(EndingPunc.ENDING_A);
			endingPuncList_.Add(EndingPunc.ENDING_RP);
			endingPuncList_.Add(EndingPunc.ENDING_RSB);
			endingPuncList_.Add(EndingPunc.ENDING_RCB);
		}
	}

}
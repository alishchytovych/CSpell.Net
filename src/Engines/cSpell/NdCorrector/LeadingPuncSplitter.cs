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
	/// This class is the leading punctuation splitter, by adding space after them.
	/// 
	/// Leading punctuation is a punctuation leads a word after a space.
	/// It includes:
	/// - Ampersand [&amp;]
	/// - Left Parenthesis [(]
	/// - Left square bracket [[]
	/// - Left curlu Brace [{]
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
	public class LeadingPuncSplitter {
		// private constructor
		private LeadingPuncSplitter() { }

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
				outTokenObj.AddProcToHist(TokenObj.HIST_ND_S_L_P);
				DebugPrint.PrintCorrect("ND", "LeadingPuncSplitter", inTokenStr, outTokenStr, debugFlag);
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
				outTokenObj.AddProcToHist(TokenObj.HIST_ND_S_L_P);
			}
			return outTokenObj;
		}
		public static string Process(string inWord, int maxProcess) {
			string lastText = inWord;
			string outText = Process(inWord);
			while ((maxProcess > 0) && (outText.Equals(lastText) == false)) {
				// recusively process
				lastText = outText;
				// converts to textObj for recursively process
				List<TokenObj> inTokenList = TextObj.TextToTokenList(lastText);
				List<TokenObj> outTokenList = new List<TokenObj>(inTokenList.Select(token => Process(token)).ToList());

				outText = TextObj.TokenListToText(outTokenList);
				maxProcess--;
			}
			return outText;
		}
		/// <summary>
		/// This method splits the input word by adding a space after leading 
		/// punctuation.  The input must be single word (no space).
		/// The process method splits the inWord by adding space(s) after leadingPunc.
		/// Current algorithm can only handle max. up to 3 endignPuncs.
		/// One in each component of coreTermObj: coreTerm, prefix, and suffix.
		/// - prefix: leading str with punc|spac|number
		/// - coreterm: = the original str - prefix - suffix
		/// - suffix: leading str with punc|space|number
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
			// preProcess: check if can skip
			int ctType = CoreTermUtil.CT_TYPE_SPACE_PUNC_DIGIT;
			if (IsQualified(inWord) == true) {
				// 0. convert to coreTerm object
				bool splitFlag = false;
				CoreTermObj cto = new CoreTermObj(inWord, ctType);
				// 1. update coreTerm
				string inCoreTerm = cto.GetCoreTerm();
				string firstLeadingPunc = FindFirstLeadingPunc(inCoreTerm);
				// add a space before the first leadingPunc
				if (!string.ReferenceEquals(firstLeadingPunc, null)) {
					// get the splitObj and then the split string
					string outCoreTerm = LeadingPunc.GetSplitStr(inCoreTerm, firstLeadingPunc);
					cto.SetCoreTerm(outCoreTerm);
					splitFlag = true;
				}
				// 2. update the prefix when it ends with a leadingPunc
				// prefix contains punc and numbers
				string prefix = cto.GetPrefix();
				if ((prefix.Length != 0) && (ContainsLeadingPunc(prefix) == true) && (IsPureLeadingPunc(prefix) == false)) { //can't be pure leadingPuncs
					// add space before the first leadingPunc
					string firstLeadingPunc2 = FindFirstLeadingPunc(prefix);
					if (!string.ReferenceEquals(firstLeadingPunc2, null)) {
						// get the splitObj and then the split string
						string outPrefix = LeadingPunc.GetSplitStr(prefix, firstLeadingPunc2);
						cto.SetPrefix(outPrefix);
						splitFlag = true;
					}
				}
				// 3. update the suffix and add a space after the last leadingPunc
				// suffix contians punctuation and numbers
				string suffix = cto.GetSuffix();
				if ((suffix.Length != 0) && (LeadsWithLeadingPunc(suffix) == true)) { //leads with leadingPunc
					suffix = GlobalVars.SPACE_STR + suffix;
					cto.SetSuffix(suffix);
					splitFlag = true;
				}
				// update outWord
				if (splitFlag == true) {
					outWord = cto.ToString();
				}
			}
			return outWord;
		}
		// check if the token can be skipped
		private static string FindFirstLeadingPunc(string inWord) {
			int minFirstIndex = int.MaxValue;
			string firstLeadingPunc = null;
			foreach (string leadingPunc in leadingPuncList_) {
				int firstIndex = inWord.IndexOf(leadingPunc, StringComparison.Ordinal);
				if ((firstIndex != -1) && (firstIndex < minFirstIndex)) {
					firstLeadingPunc = leadingPunc;
					minFirstIndex = firstIndex;
				}
			}
			return firstLeadingPunc;
		}
		private static string FindLastLeadingPunc(string inWord) {
			int maxLastIndex = -1;
			string lastLeadingPunc = null;
			foreach (string leadingPunc in leadingPuncList_) {
				int lastIndex = inWord.LastIndexOf(leadingPunc, StringComparison.Ordinal);
				if (lastIndex > maxLastIndex) {
					lastLeadingPunc = leadingPunc;
					maxLastIndex = lastIndex;
				}
			}
			return lastLeadingPunc;
		}
		// broader matcher
		private static bool IsQualified(string inWord) {
			bool qFlag = false;
			// use coreTerm for URL and eMail
			int ctType = CoreTermUtil.CT_TYPE_SPACE_PUNC_DIGIT;
			CoreTermObj cto = new CoreTermObj(inWord, ctType);
			string inCoreTerm = cto.GetCoreTerm();
			// check if pass the matcher to be qualified
			if (ContainsLeadingPunc(inWord) == true) { // contains leadingPunc
				qFlag = true;
			}
			return qFlag;
		}
		private static bool IsPureLeadingPunc(string inWord) {
			var matcher = patternLP_.Match(inWord);
			return matcher.Success;
		}
		private static bool LeadsWithLeadingPunc(string inWord) {
			var matcher = patternLLP_.Match(inWord);
			return matcher.Success;
		}
		private static bool EndsWithLeadingPunc(string inWord) {
			var matcher = patternELP_.Match(inWord);
			return matcher.Success;
		}
		private static bool ContainsLeadingPunc(string inWord) {
			var matcher = patternCLP_.Match(inWord);
			return matcher.Success;
		}
		// test driver
		private static void TestProcessWord() {
			Console.WriteLine("----- Test Process Word: -----");
			List<string> inWordList = new List<string>();
			inWordList.Add("~[NAME]"); // 42.txt
			inWordList.Add("-[NAME]"); // 18175.txt
			inWordList.Add("1-plug&"); // 12271.txt
			inWordList.Add("genes[transposons]"); // 78.txt
			inWordList.Add("epilepsy(left"); // 12353.txt
			inWordList.Add("test(HLA-B27)"); // 18186.txt
			inWordList.Add("you,[NAME]"); // 50.txt
			inWordList.Add("dr.[NAME]");
			inWordList.Add("Dr.[NAME]");
			inWordList.Add("<[CONTACT]>.");
			inWordList.Add("you,[NAME]");
			inWordList.Add("R&D"); // .txt
			inWordList.Add("Research&development"); // .txt
			inWordList.Add("xxx(2)-yyy");
			int MaxRecursive = 5;
			foreach (string inWord in inWordList) {
				Console.WriteLine("- Process(" + inWord + "): " + Process(inWord, MaxRecursive));
			}
		}
		private static void TestProcessText() {
			// init
			Console.WriteLine("----- Test Process Text: -----");
			string inText = "Head rolling &amp;amp; rock(5'8&quot;).";
			int MaxRecursive = 5;
			// test process:  must use ArrayList<TextObj>
			List<TokenObj> inTokenList = TextObj.TextToTokenList(inText);
			List<TokenObj> outTokenList = new List<TokenObj>(inTokenList.Select(token => XmlHtmlHandler.Process(token)).Select(token => LeadingPuncSplitter.Process(token, MaxRecursive)).ToList());
			// result
			string outText = TextObj.TokenListToText(outTokenList);
			// print out
			Console.WriteLine("--------- LeadingPuncSplitter( ) Test -----------");
			Console.WriteLine("In: [" + inText + "]");
			Console.WriteLine("Out: [" + outText + "]");
			Console.WriteLine("----- Details -----------");
			int index = 0;
			foreach (TokenObj tokenObj in outTokenList) {
				Console.WriteLine(index + "|" + tokenObj.ToHistString());
				index++;
			}
		}
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java LeadingPuncSplitter");
				Environment.Exit(0);
			}

			//TestProcessWord();
			TestProcessText();
		}
		// data member

		// pure leading punctuation: add <
		private const string patternStrLP_ = "^[&\\(\\[\\{<]*$";
		private static readonly Regex patternLP_ = new JRegex(patternStrLP_, RegexOptions.Compiled);
		// contains leading punctuation 
		private const string patternStrCLP_ = "^.*[&\\(\\[\\{].*$";
		private static readonly Regex patternCLP_ = new JRegex(patternStrCLP_, RegexOptions.Compiled);
		// leads with leading punctuation, must have 1 char at least 
		private const string patternStrLLP_ = "^[&\\(\\[\\{].*$";
		private static readonly Regex patternLLP_ = new JRegex(patternStrLLP_, RegexOptions.Compiled);
		// ends with leading punctuation 
		private const string patternStrELP_ = "^.*[&\\(\\[\\{]$";
		private static readonly Regex patternELP_ = new JRegex(patternStrELP_, RegexOptions.Compiled);
		private static readonly List<string> leadingPuncList_ = new List<string>();
		static LeadingPuncSplitter() {
			leadingPuncList_.Add(LeadingPunc.LEADING_A);
			leadingPuncList_.Add(LeadingPunc.LEADING_LP);
			leadingPuncList_.Add(LeadingPunc.LEADING_LSB);
			leadingPuncList_.Add(LeadingPunc.LEADING_LCB);
		}
	}

}
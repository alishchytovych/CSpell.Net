using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.NdCorrector {

	/// <summary>
	///***************************************************************************
	/// This class is the ending digit splitter, adding a space before the digit.
	/// Such as "from2007" to "from 2007". There are legit word ends with digits
	/// and they are consider as exceptionss. They are:
	/// - (1st|2nd|3rd|\dth)
	/// - (\d+)[a-zA-Z]]
	/// - (\d+)[a-zA-z]+-[a-zA-Z]+] to ["]
	/// - (\d+)[A-Z]+[A-Z0-9]+
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
	public class EndingDigitSplitter {
		// private constructor
		private EndingDigitSplitter() { }

		public static TokenObj Process(TokenObj inTokenObj) {
			bool debugFlag = false;
			return Process(inTokenObj, debugFlag);
		}
		public static TokenObj Process(TokenObj inTokenObj, bool debugFlag) {
			string inTokenStr = inTokenObj.GetTokenStr();
			string outTokenStr = Process(inTokenStr);
			TokenObj outTokenObj = new TokenObj(inTokenObj);
			//update info if there is a process
			if (inTokenStr.Equals(outTokenStr) == false) {
				outTokenObj.SetTokenStr(outTokenStr);
				outTokenObj.AddProcToHist(TokenObj.HIST_ND_S_E_D);
				DebugPrint.PrintCorrect("ND", "EndingDigitSplitter", inTokenStr, outTokenStr, debugFlag);
			}
			return outTokenObj;
		}
		/// <summary>
		/// This method handle leading digit by adding a space after the digit
		/// It is desgined to work on the input of single word.
		/// </summary>
		/// <param name="inWord">  the input token (single word)
		/// </param>
		/// <returns>   the corrected word, does nto change the case,
		///           the original input token is returned if no mapping is found. </returns>
		public static string Process(string inWord) {
			string outWord = inWord;
			// convert to coreterm, such as hereditary2)
			bool splitFlag = false;
			int ctType = CoreTermUtil.CT_TYPE_SPACE_PUNC;
			CoreTermObj cto = new CoreTermObj(inWord, ctType);
			string inCoreTerm = cto.GetCoreTerm();
			// update core term: check if the token leads with digit
			var matcherLd = patternED_.Match(inCoreTerm);

			if ((matcherLd.Success == true) && (DigitPuncTokenUtil.IsDigitPunc(inCoreTerm) == false)) { // can't be digit
				// update core term: split if it is an exception
				if (IsException(inCoreTerm) == false) {
					string outCoreTerm = matcherLd.Groups[1].Value + matcherLd.Groups[2].Value + GlobalVars.SPACE_STR + matcherLd.Groups[3].Value;
					cto.SetCoreTerm(outCoreTerm);
					splitFlag = true;
				}
			}
			// get outWord from coreTermObj if split happens
			if (splitFlag == true) {
				outWord = cto.ToString();
			}
			return outWord;
		}
		private static bool IsException(string inWord) {
			bool checkEmptyToken = false;
			bool expFlag = false;
			if ((TokenUtil.IsMatch(inWord, patternEDE1_, checkEmptyToken) == true) || (TokenUtil.IsMatch(inWord, patternEDE2_, checkEmptyToken) == true) || (TokenUtil.IsMatch(inWord, patternEDE3_, checkEmptyToken) == true) || (TokenUtil.IsMatch(inWord, patternEDE4_, checkEmptyToken) == true)) {
				expFlag = true;
			}
			return expFlag;
		}
		// test driver
		private static void TestProcessWord() {
			Console.WriteLine("----- Test Process Word: -----");
			List<string> inWordList = new List<string>();
			inWordList.Add("100.1)"); // 26.txt
			inWordList.Add("questions.1)"); // 26.txt
			inWordList.Add("hereditary2)"); // 26.txt
			inWordList.Add("disease3)"); // 26.txt
			inWordList.Add("from2007");
			inWordList.Add("Iam21year");
			inWordList.Add("shuntfrom2007."); // 14849.txt
			inWordList.Add("jk5"); // 73.txt
			inWordList.Add("-.1)");
			inWordList.Add("+.1)");
			inWordList.Add(".1)");
			inWordList.Add("0.112)");
			// exception 1
			inWordList.Add("A1");
			inWordList.Add("A2780");
			inWordList.Add("UPD14"); // 94.txt
			inWordList.Add("CAD106"); // 14240.txt
			// exception 2
			inWordList.Add("NCI-H460");
			inWordList.Add("CCRF-HSB2");
			inWordList.Add("Co-Q10");
			inWordList.Add("saframycin-Yd2");
			// exception 3
			inWordList.Add("alpha1");
			inWordList.Add("beta2");
			inWordList.Add("gamma2");
			inWordList.Add("delta1");
			inWordList.Add("epsilon4");
			// exception 4
			inWordList.Add("c7"); // 18055.txt
			foreach (string inWord in inWordList) {
				Console.WriteLine("- Process(" + inWord + "): " + Process(inWord));
			}
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java EndingDigitSplitter");
				Environment.Exit(0);
			}

			// init
			TestProcessWord();
		}
		// data member
		// pattern of leading digits
		private const string patternStrED_ = "^(.*)([a-zA-Z\\.]+)(\\d+)$";
		private static readonly Regex patternED_ = new JRegex(patternStrED_, RegexOptions.Compiled);
		// pattern of exception 1: [chars][digit]+
		private const string patternStrEDE1_ = "^([A-Z]+)(\\d+)$";
		private static readonly Regex patternEDE1_ = new JRegex(patternStrEDE1_, RegexOptions.Compiled);
		// pattern of exception 2: [a-zA-Z]-[a-zA-z][digit]
		private const string patternStrEDE2_ = "^([a-zA-Z]+)-([a-zA-Z]+)(\\d+)$";
		private static readonly Regex patternEDE2_ = new JRegex(patternStrEDE2_, RegexOptions.Compiled);
		// pattern of exception 3: [.][alpha|beta|gamma][digit]
		private const string patternStrEDE3_ = "^(.*)(alpha|beta|gamma|delta|epsilon)(\\d)$";
		private static readonly Regex patternEDE3_ = new JRegex(patternStrEDE3_, RegexOptions.Compiled);
		// pattern of exception 4: [single chars][digit]+
		private const string patternStrEDE4_ = "^([a-zA-Z])(\\d+)$";
		private static readonly Regex patternEDE4_ = new JRegex(patternStrEDE4_, RegexOptions.Compiled);
	}

}
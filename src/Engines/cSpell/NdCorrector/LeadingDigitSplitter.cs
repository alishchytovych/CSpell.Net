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
	/// This class is the leading digit splitter, by adding a space after the digit.
	/// Such as "20years" to "20 years". There are legit word starts with digits
	/// and they are consider as exceptionss. They are:
	/// - (1st|2nd|3rd|\dth)
	/// - (\d+)[a-zA-Z]]
	/// - (\d+)[a-zA-z]+-[a-zA-Z]+] to ["]
	/// - (\d+)[A-Z]+[A-Z0-9]+
	/// - (\d+)([a-zA-Z])(punct, digit)*
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
	public class LeadingDigitSplitter {
		// private constructor
		private LeadingDigitSplitter() { }

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
				outTokenObj.AddProcToHist(TokenObj.HIST_ND_S_L_D);
				DebugPrint.PrintCorrect("ND", "LeadingDigitSplitter", inTokenStr, outTokenStr, debugFlag);
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
			// convert to coreterm, such as 30th.
			bool splitFlag = false;
			int ctType = CoreTermUtil.CT_TYPE_SPACE_PUNC;
			CoreTermObj cto = new CoreTermObj(inWord, ctType);
			string inCoreTerm = cto.GetCoreTerm();
			// update core term: check if the token leads with digit
			var matcherLd = patternLD_.Match(inCoreTerm);
			if (matcherLd.Success == true) {
				// update core term: split if it is an exception
				if (IsException(inCoreTerm) == false) {
					string outCoreTerm = matcherLd.Groups[1].Value + GlobalVars.SPACE_STR + matcherLd.Groups[2].Value + matcherLd.Groups[3].Value;
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
			bool expFlag = false;
			bool checkEmptyToken = false;
			if ((TokenUtil.IsMatch(inWord, patternLDE1_, checkEmptyToken) == true) || (TokenUtil.IsMatch(inWord, patternLDE2_, checkEmptyToken) == true) || (TokenUtil.IsMatch(inWord, patternLDE3_, checkEmptyToken) == true) || (TokenUtil.IsMatch(inWord, patternLDE4_, checkEmptyToken) == true) || (TokenUtil.IsMatch(inWord, patternLDE5_, checkEmptyToken) == true)) {
				expFlag = true;
			}
			return expFlag;
		}
		// test driver
		private static void TestProcessWord(Dictionary<string, string> informalExpMap) {
			Console.WriteLine("----- Test Process Word: -----");
			List<string> inWordList = new List<string>();
			// split
			inWordList.Add("21year");
			inWordList.Add("5mg");
			inWordList.Add("5and");
			inWordList.Add("Iam21year");
			// exception 1
			inWordList.Add("1st");
			inWordList.Add("42nd");
			inWordList.Add("3rd");
			inWordList.Add("11th");
			inWordList.Add("31st");
			inWordList.Add("31th");
			inWordList.Add("30th."); // 73.txt
			inWordList.Add("30th");
			// exception 2
			inWordList.Add("31D");
			inWordList.Add("9L");
			inWordList.Add("5q");
			// exception 3
			inWordList.Add("67LR");
			inWordList.Add("3Y1");
			inWordList.Add("7PA2");
			inWordList.Add("5FU");
			// exception 4
			inWordList.Add("111In-Cl");
			inWordList.Add("5q-syndrome");
			inWordList.Add("38C-13");
			// exception 5
			inWordList.Add("1q21.1."); // 13.txt
			inWordList.Add("1q21.1"); // 13.txt
			inWordList.Add("1q21");
			inWordList.Add("16P-13.11"); // 77.txt
			inWordList.Add("16P-13");
			// others
			inWordList.Add("15years");
			inWordList.Add("1.5years");

			foreach (string inWord in inWordList) {
				Console.WriteLine("- Process(" + inWord + "): " + Process(inWord));
			}
		}
		private static void TestProcess(Dictionary<string, string> informalExpMap) {
			// init
			Console.WriteLine("----- Test Process Text: -----");
			string inText = "u rolling &amp;amp; pls(12years).";
			// test process:  must use ArrayList<TextObj>
			List<TokenObj> inTokenList = TextObj.TextToTokenList(inText);
			List<TokenObj> outTokenList = new List<TokenObj>(inTokenList.Select(token => XmlHtmlHandler.Process(token))
			.Select(token => LeadingPuncSplitter.Process(token)).SelectMany(token => TextObj.FlatTokenToArrayList(token))
			.Select(token => LeadingDigitSplitter.Process(token)).SelectMany(token => TextObj.FlatTokenToArrayList(token))
			.Select(token => InformalExpHandler.Process(token, informalExpMap)).ToList());

			// result
			string outText = TextObj.TokenListToText(outTokenList);
			// print out
			Console.WriteLine("--------- LeadingDigitSplitter( ) Test ----------");
			Console.WriteLine("In: [" + inText + "]");
			Console.WriteLine("Out: [" + outText + "]");
			Console.WriteLine("----- Details -----------");
			int index = 0;
			foreach (TokenObj tokenObj in outTokenList) {
				Console.WriteLine(index + "|" + tokenObj.ToHistString());
				index++;
			}
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java LeadingDigitSplitter");
				Environment.Exit(0);
			}

			// init
			string inFile = "../data/Misc/informalExpression.data";
			Dictionary<string, string> informalExpMap = InformalExpHandler.GetInformalExpMapFromFile(inFile);
			// init
			TestProcessWord(informalExpMap);
			TestProcess(informalExpMap);
		}
		// data member
		// pattern of leading digits: must have t char after leading digits
		private const string patternStrLD_ = "^(\\d*\\.?\\d+)([a-zA-Z]{2,})(.*)$";
		private static readonly Regex patternLD_ = new JRegex(patternStrLD_, RegexOptions.Compiled);
		// pattern of exception 1: ordinal number
		private const string patternStrLDE1_ = "^((\\d*)(1st|2nd|3rd))|((\\d+)(th))$";
		private static readonly Regex patternLDE1_ = new JRegex(patternStrLDE1_, RegexOptions.Compiled);
		// pattern of exception 2: [digit][single character]
		private const string patternStrLDE2_ = "^(\\d+)([a-zA-Z])$";
		private static readonly Regex patternLDE2_ = new JRegex(patternStrLDE2_, RegexOptions.Compiled);
		// pattern of exception 3: [digit][Uppercase][uppercase, digit]
		private const string patternStrLDE3_ = "^(\\d+)([A-Z]+)([A-Z0-9]*)$";
		private static readonly Regex patternLDE3_ = new JRegex(patternStrLDE3_, RegexOptions.Compiled);
		// pattern of exception 4: [digit][Uppercase][uppercase, digit]
		private const string patternStrLDE4_ = "^(\\d+)([a-zA-Z]+)-(\\w*)$";
		private static readonly Regex patternLDE4_ = new JRegex(patternStrLDE4_, RegexOptions.Compiled);
		// pattern of exception 5: [digit][single character][digit, punc]
		private const string patternStrLDE5_ = "^(\\d+)([a-zA-Z])([!\"#$%&'()*+,\\-./:;<=>?@\\[\\\\\\]^_`{|}~\\d]*)$";
		private static readonly Regex patternLDE5_ = new JRegex(patternStrLDE5_, RegexOptions.Compiled);
	}

}
using System;
using System.Collections.Generic;
using System.Linq;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;
using SpellChecker.Engines.cSpell.Extensions;

namespace SpellChecker.Engines.cSpell.NdCorrector {

	/// <summary>
	///***************************************************************************
	/// This class is the informal expression handler. It matches and maps informal 
	/// expression to a pre-specified term (formal expression).
	/// 
	/// <para><b>History:</b>
	/// <ul>
	/// <li>2018 baseline
	/// <li>SCR-4, chlu, 03-05-19, handle [term] with ending punctuation of .?!
	/// </ul>
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ****************************************************************************
	/// </para>
	/// </summary>
	public class InformalExpHandler {
		// private constructor
		/// <summary>
		/// Private constructor so no one can instantiate
		/// </summary>
		private InformalExpHandler() { }
		// public method
		/// <summary>
		/// A method to process mapping frominformal expression to corrected word.
		/// The lowercase of inWord is used as key for the mapping.
		/// </summary>
		/// <param name="inTokenObj">    the input tokenObj (single word) </param>
		/// <param name="informalExpMap"> the map of informal expression
		/// </param>
		/// <returns>    the mapped corrected word (lowercase only) if mappnig found, 
		///             toherwise, the original input token is returned. </returns>
		public static TokenObj Process(TokenObj inTokenObj, Dictionary<string, string> informalExpMap) {
			bool debugFlag = false;
			return Process(inTokenObj, informalExpMap, debugFlag);
		}
		public static TokenObj Process(TokenObj inTokenObj, Dictionary<string, string> informalExpMap, bool debugFlag) {
			string inTokenStr = inTokenObj.GetTokenStr();
			string outTokenStr = ProcessWord(inTokenStr, informalExpMap);
			TokenObj outTokenObj = new TokenObj(inTokenObj);
			//update info if there is a process
			if (inTokenStr.Equals(outTokenStr) == false) {
				outTokenObj.SetTokenStr(outTokenStr);
				outTokenObj.AddProcToHist(TokenObj.HIST_ND_INFORMAL_EXP);
				DebugPrint.PrintCorrect("ND", "InformalExpHandler", inTokenStr, outTokenStr, debugFlag);
			}
			return outTokenObj;
		}
		// private Methods
		/// <summary>
		/// A method to process mapping frominformal expression to corrected word.
		/// The lowercase of inWord is used as key for the mapping.
		/// </summary>
		/// <param name="inWord">    the input token (single word) </param>
		/// <param name="informalExpMap"> the map of informal expression
		/// </param>
		/// <returns>    the mapped corrected word (lowercase only), 
		///             the original input token is returned if no mapping is found. </returns>
		private static string ProcessWord(string inWord, Dictionary<string, string> informalExpMap) {
			string outWord = inWord;
			if (informalExpMap != null) {
				// 1. map to the inWord
				string mapWord = informalExpMap.GetValueOrNull(inWord.ToLower());
				if (!string.ReferenceEquals(mapWord, null)) {
					outWord = mapWord;
				}
				// SCR-4, handle ending punctuation .?!
				// 2. map to the inWord. inWord? inWord!
				else if ((inWord.EndsWith(".", StringComparison.Ordinal)) || (inWord.EndsWith("?", StringComparison.Ordinal)) || (inWord.EndsWith("!", StringComparison.Ordinal))) {
					string endWord = inWord.Substring(inWord.Length - 1);
					string coreWord = inWord.Substring(0, inWord.Length - 1);
					mapWord = informalExpMap.GetValueOrNull(coreWord.ToLower());
					if (!string.ReferenceEquals(mapWord, null)) {
						outWord = mapWord + endWord;
					}
				}
			}

			return outWord;
		}
		private static string ProcessWord1(string inWord, Dictionary<string, string> informalExpMap) {
			string outWord = inWord;
			if (informalExpMap != null) {
				string mapWord = informalExpMap.GetValueOrNull(inWord.ToLower());
				if (!string.ReferenceEquals(mapWord, null)) {
					outWord = mapWord;
				}
			}
			return outWord;
		}
		public static Dictionary<string, string> GetInformalExpMapFromFile(string inDataFile) {
			bool verboseFlag = false;
			return GetInformalExpMapFromFile(inDataFile, verboseFlag);
		}
		public static Dictionary<string, string> GetInformalExpMapFromFile(string inDataFile, bool verboseFlag) {
			// read in informal express map from a file
			Dictionary<string, string> informalExpMap = FileInToMap.GetHashMapByFields(inDataFile, verboseFlag);
			return informalExpMap;
		}
		private static void TestProcessWord(Dictionary<string, string> informalExpMap) {
			string inText = "? pls";
			string outText = InformalExpHandler.ProcessWord(inText, informalExpMap);
			Console.WriteLine("- in: [" + inText + "], out:[" + outText + "]");
		}
		private static void TestProcess(Dictionary<string, string> informalExpMap) {
			// init
			string inText = "u rolling &amp;amp; pls(12years).";
			// test process:  must use ArrayList<TextObj>
			List<TokenObj> inTokenList = TextObj.TextToTokenList(inText);
			List<TokenObj> outTokenList = new List<TokenObj>(inTokenList.Select(tokenObj => InformalExpHandler.Process(tokenObj, informalExpMap)).ToList());
			// result    
			string outText = TextObj.TokenListToText(outTokenList);
			// print out
			Console.WriteLine("--------- InformalExpHandler( ) -----------");
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
			string inFile = "../data/Misc/informalExpression.data";
			if (args.Length == 1) {
				inFile = args[0];
			} else if (args.Length > 0) {
				Console.WriteLine("Usage: java InformalExpHandler <inFile>");
				Environment.Exit(0);
			}

			// init
			Dictionary<string, string> informalExpMap = InformalExpHandler.GetInformalExpMapFromFile(inFile);

			// test
			TestProcessWord(informalExpMap);
			TestProcess(informalExpMap);
		}
		// data member
	}

}
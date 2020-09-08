using System;
using System.Collections.Generic;
using System.Linq;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;
using SpellChecker.Engines.cSpell.Extensions;

namespace SpellChecker.Engines.cSpell.NdCorrector {
	/// <summary>
	///***************************************************************************
	/// This class converts Html/Xml entity to ASCII by handling escape characters.
	/// It includes:
	/// - [&amp;lt;] to [&lt;]
	/// - [&amp;gt;] to [&gt;]
	/// - [&amp;amp;] to [&amp;]
	/// - [&amp;quot;] to [&quot;]
	/// - [&amp;nbsp;] to [&nbsp;]
	/// 
	/// - Does not handle &amp;#dd; to ASCII, might need it if they are in test data 
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
	public class XmlHtmlHandler {
		// private constructor
		private XmlHtmlHandler() { }

		// public methods
		/// <summary>
		/// This method converts Html/Xml entityto ASCII code. 
		/// It is desgined to work on the input of a TokenObj (single word or text).
		/// </summary>
		/// <param name="inTokenObj">  the input TokenObj
		/// </param>
		/// <returns>   the corrected word in the format TokenObj with process 
		///             information in the history if mapping is found. Otherwise,
		///           the original input token is returned. </returns>
		public static TokenObj Process(TokenObj inTokenObj) {
			bool debugFlag = false;
			return Process(inTokenObj, debugFlag);
		}
		public static TokenObj Process(TokenObj inTokenObj, bool debugFlag) {
			// get string from tokenObj
			string inTokenStr = inTokenObj.GetTokenStr();
			string outTokenStr = ProcessWord(inTokenStr);
			//update info if there is a XMl/Html process
			TokenObj outTokenObj = new TokenObj(inTokenObj);
			if (inTokenStr.Equals(outTokenStr) == false) {
				outTokenObj.SetTokenStr(outTokenStr);
				outTokenObj.AddProcToHist(TokenObj.HIST_ND_XML_HTML);
				DebugPrint.PrintCorrect("ND", "XmlHtmlHandler", inTokenStr, outTokenStr, debugFlag);
			}
			return outTokenObj;
		}
		// private methods
		/// <summary>
		/// This method converts Html/Xml entityto ASCII code. 
		/// It is desgined to work on the input of single word.
		/// However, it should also work on a text.
		/// </summary>
		/// <param name="inWord">  the input token (single word)
		/// </param>
		/// <returns>   the corrected word, does not change the case,
		///           the original input token is returned if no mapping is found. </returns>
		private static string ProcessWord(string inWord) {
			string outWord = inWord;
			ISet<string> keySet = escapeCharMap_.Keys.ToHashSet();
			foreach (string key in keySet) {
				int pos = outWord.IndexOf(key, StringComparison.Ordinal);
				while (pos >= 0) {
					int keyLength = key.Length;
					string mapStr = escapeCharMap_.GetValueOrNull(key);
					outWord = outWord.Substring(0, pos) + mapStr + outWord.Substring(pos + keyLength);
					pos = outWord.IndexOf(key, StringComparison.Ordinal); //this resolves recursive mapping
				}
			}

			return outWord;
		}
		// the element is Word (String)
		private static void TestProcessWord() {
			Console.WriteLine("----- Test Process Word: -----");
			List<string> inWordList = new List<string>();
			inWordList.Add(",do");
			inWordList.Add("&lt;tag&gt;");
			foreach (string inWord in inWordList) {
				Console.WriteLine("- Process(" + inWord + "): " + ProcessWord(inWord));
			}
		}
		// the element is TokenObj
		private static void TestProcess() {
			// init
			Console.WriteLine("----- Test Process Text: -----");
			string inText = "Xml: head rolling &amp;amp; rock, (5'8&quot;).";
			// test process:  must use ArrayList<TextObj>
			List<TokenObj> inTokenList = TextObj.TextToTokenList(inText);
			List<TokenObj> outTokenList = new List<TokenObj>(inTokenList.Select(token => XmlHtmlHandler.Process(token)).ToList());
			// result
			string outText = TextObj.TokenListToText(outTokenList);

			// print out
			Console.WriteLine("--------- XmlHtmlHandler( ) Test -----------");
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
				Console.WriteLine("Usage: java XmlHtmlHandler");
				Environment.Exit(0);
			}

			TestProcessWord();
			TestProcess();
		}
		// data members
		private static readonly Dictionary<string, string> escapeCharMap_ = new Dictionary<string, string>();
		static XmlHtmlHandler() {
			escapeCharMap_["&amp;"] = "&";
			escapeCharMap_["&lt;"] = "<";
			escapeCharMap_["&gt;"] = ">";
			escapeCharMap_["&quot;"] = "\"";
			escapeCharMap_["&nbsp;"] = " ";
			//escapeCharMap_.put("&nbsp;","\u00a0");
		}
	}

}
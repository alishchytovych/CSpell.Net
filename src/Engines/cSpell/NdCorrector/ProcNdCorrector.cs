using System;
using System.Collections.Generic;
using System.Linq;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.NdCorrector {

	/// <summary>
	///***************************************************************************
	/// This class is core for processing non-dictionary-based correction in CSpell.
	/// Including:
	/// <ul>
	/// <li>XmlHtmlHandler
	/// <li>LeadingDigitSplitter
	/// <li>LeadingPuncSplitter
	/// <li>EndingDigitSplitter
	/// <li>EndingPuncSplitter
	/// <li>InformalExpHandler
	/// </ul>
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
	public class ProcNdCorrector {
		// private constructor
		/// <summary>
		/// Private constructor includes only static methods and no one can 
		/// instaniate.
		/// </summary>
		private ProcNdCorrector() { }
		// pre-Correction
		// the core process of pre-correction
		// use Java 8 stream
		public static List<TokenObj> Process(List<TokenObj> inTokenList, int ndMaxSplitNo, Dictionary<string, string> infExpMap) {
			bool debugFlag = false;
			return Process(inTokenList, ndMaxSplitNo, infExpMap, debugFlag);
		}
		public static List<TokenObj> Process(List<TokenObj> inTokenList, int ndMaxSplitNo, Dictionary<string, string> infExpMap, bool debugFlag) {
			DebugPrint.PrintProcess("1. NonDictionary", debugFlag);
			DebugPrint.PrintInText(TextObj.TokenListToText(inTokenList), debugFlag);
			// process on each tokenObj

			List<TokenObj> outTokenList = new List<TokenObj>(inTokenList
				.Select(token => XmlHtmlHandler.Process(token, debugFlag))
				.Select(token => EndingPuncSplitter.Process(token, ndMaxSplitNo, debugFlag))
				.SelectMany(token => TextObj.FlatTokenToArrayList(token))
				.Select(token => LeadingPuncSplitter.Process(token, ndMaxSplitNo, debugFlag))
				.SelectMany(token => TextObj.FlatTokenToArrayList(token))
				.Select(token => LeadingDigitSplitter.Process(token, debugFlag))
				.SelectMany(token => TextObj.FlatTokenToArrayList(token))
				.Select(token => EndingDigitSplitter.Process(token, debugFlag))
				.SelectMany(token => TextObj.FlatTokenToArrayList(token))
				.Select(token => InformalExpHandler.Process(token, infExpMap, debugFlag)).ToList());
			/*
			List<TokenObj> outTokenList2 = inTokenList
				.Select(token => XmlHtmlHandler.Process(token, debugFlag))
				.Select(token => EndingPuncSplitter.Process(token, ndMaxSplitNo, debugFlag)).ToList();
foreach(var item in outTokenList2) {
				Console.WriteLine(item.GetTokenStr());
			}
			List<TokenObj> outTokenList3 = inTokenList
				.Select(token => XmlHtmlHandler.Process(token, debugFlag))
				.Select(token => EndingPuncSplitter.Process(token, ndMaxSplitNo, debugFlag))
				.SelectMany(token => TextObj.FlatTokenToArrayList(token)).ToList();
foreach(var item in outTokenList3) {
				Console.WriteLine(item.GetTokenStr());
			}


			List<TokenObj> outTokenList = inTokenList
				.Select(token => XmlHtmlHandler.Process(token, debugFlag))
				.Select(token => EndingPuncSplitter.Process(token, ndMaxSplitNo, debugFlag))
				.Select(token => LeadingPuncSplitter.Process(token, ndMaxSplitNo, debugFlag))
				.Select(token => LeadingDigitSplitter.Process(token, debugFlag))
				.Select(token => EndingDigitSplitter.Process(token, debugFlag))
				.Select(token => InformalExpHandler.Process(token, infExpMap, debugFlag)).ToList();
*/
			return outTokenList;
		}
		// privat methods
		private static void Test(string configFile) {
			// init
			Console.WriteLine("----- Test Pre-Correction Text: -----");
			string inText = "We  cant theredve hell.Plz u r good123. ";
			CSpellApi cSpellApi = new CSpellApi(configFile);
			int ndMaxSplitNo = cSpellApi.GetCanNdMaxSplitNo();
			Dictionary<string, string> infExpMap = cSpellApi.GetInformalExpressionMap();
			bool debugFlag = false;

			// 1. convert input to TokenObjs
			List<TokenObj> inTokenList = TextObj.TextToTokenList(inText);
			List<TokenObj> outTokenList = ProcNdCorrector.Process(inTokenList, ndMaxSplitNo, infExpMap, debugFlag);
			string outText = TextObj.TokenListToText(outTokenList);
			// print out
			Console.WriteLine("--------------------");
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
			string configFile = "../data/Config/cSpell.properties";
			if (args.Length == 1) {
				configFile = args[0];
			} else if (args.Length > 0) {
				Console.WriteLine("Usage: java NdCorrector <configFile>");
				Environment.Exit(0);
			}
			Test(configFile);
		}
		// data member
		// processes related data
	}

}
using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NdCorrector;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Api {
	/// <summary>
	///***************************************************************************
	/// This class is API of non-dictionary-based correction in CSpell. It is called
	/// by CSpellApi.
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
	public class NdCorrectionApi {
		// public constructor
		/// <summary>
		/// Private constructor for NdCorrectionApi so no one can instantiate.
		/// </summary>
		private NdCorrectionApi() { }
		// pre-Correction
		public static string ProcessByStr(string inText, int maxSpRecursiveNo, Dictionary<string, string> infExpMap) {
			bool debugFlag = false;
			return ProcessByStr(inText, maxSpRecursiveNo, infExpMap, debugFlag);
		}
		// use TextObj (instead of TextIoObj)
		public static string ProcessByStr(string inText, int maxSpRecursiveNo, Dictionary<string, string> infExpMap, bool debugFlag) {
			List<TokenObj> outTokenList = Process(inText, maxSpRecursiveNo, infExpMap, debugFlag);
			// result text: convert from TokenObj to str
			string outText = TextObj.TokenListToText(outTokenList);
			return outText;
		}
		public static List<TokenObj> Process(string inText, int maxSpRecursiveNo, Dictionary<string, string> infExpMap) {
			List<TokenObj> inTokenList = TextObj.TextToTokenList(inText);
			bool debugFlag = false;
			return Process(inText, maxSpRecursiveNo, infExpMap, debugFlag);
		}
		//the core of pre-correction api
		public static List<TokenObj> Process(string inText, int maxSpRecursiveNo, Dictionary<string, string> infExpMap, bool debugFlag) {
			// 1. input
			List<TokenObj> inTokenList = TextObj.TextToTokenList(inText);
			// 2. process on each tokenObj
			return ProcNdCorrector.Process(inTokenList, maxSpRecursiveNo, infExpMap, debugFlag);
		}
		// privat methods
		private static void Test(string configFile) {
			// init
			Console.WriteLine("----- Test Pre-Correction Text: -----");
			string inText = "We  cant theredve hell.Plz u r good123. ";
			CSpellApi cSpellApi = new CSpellApi(configFile);
			int ndMaxSplitNo = cSpellApi.GetCanNdMaxSplitNo();
			Dictionary<string, string> infExpMap = cSpellApi.GetInformalExpressionMap();
			string outText = ProcessByStr(inText, ndMaxSplitNo, infExpMap);
			// print out
			Console.WriteLine("--------------------");
			Console.WriteLine("In: [" + inText + "]");
			Console.WriteLine("Out: [" + outText + "]");
		}
		// test driver
		public static void MainTest(string[] args) {
			string configFile = "../data/Config/cSpell.properties";
			if (args.Length == 1) {
				configFile = args[0];
			} else if (args.Length > 0) {
				Console.WriteLine("Usage: java NdCorrectionApi <configFile>");
				Environment.Exit(0);
			}
			Test(configFile);
		}
		// data member
		// processes related data
	}

}
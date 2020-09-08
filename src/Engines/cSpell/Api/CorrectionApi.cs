using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Candidates;
using SpellChecker.Engines.cSpell.Corrector;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NdCorrector;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Ranker;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Api {
	/// <summary>
	///***************************************************************************
	/// This class is API for all spelling error correction in CSpell.
	/// It is called by CSpellApi.
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
	public class CorrectionApi {
		// private constructor
		/// <summary>
		/// private constructor for CorrectionApi, no one can instantiate.
		/// </summary>
		private CorrectionApi() { }
		// Correction
		public static string ProcessToStr(string inText, CSpellApi cSpellApi) {
			bool debugFlag = false;
			return ProcessToStr(inText, cSpellApi, debugFlag);
		}
		// use String
		public static string ProcessToStr(string inText, CSpellApi cSpellApi, bool debugFlag) {
			// 1. input
			List<TokenObj> inTokenList = TextObj.TextToTokenList(inText);
			List<TokenObj> outTokenList = ProcessByTokenObj(inTokenList, cSpellApi, debugFlag);
			// 2. convert results to text
			string outText = TextObj.TokenListToText(outTokenList);
			return outText;
		}

		public static (string, List<TokenObj>) ProcessToStrExt(string inText, CSpellApi cSpellApi, bool debugFlag) {
			// 1. input
			List<TokenObj> inTokenList = TextObj.TextToTokenList(inText);
			List<TokenObj> outTokenList = ProcessByTokenObj(inTokenList, cSpellApi, debugFlag);
			// 2. convert results to text
			string outText = TextObj.TokenListToText(outTokenList);
			return (outText, outTokenList);
		}

		public static List<TokenObj> ProcessByTokenObj(List<TokenObj> inTokenList, CSpellApi cSpellApi) {
			bool debugFlag = false;
			return ProcessByTokenObj(inTokenList, cSpellApi, debugFlag);
		}
		// Core method: of spell-correction, include split
		public static List<TokenObj> ProcessByTokenObj(List<TokenObj> inTokenList, CSpellApi cSpellApi, bool debugFlag) {
			return ProcCorrector.ProcessByTokenObj(inTokenList, cSpellApi, debugFlag);
		}
		// privat methods
		private static void Test(CSpellApi cSpellApi) {
			Console.WriteLine("----- Test Pre-Correction Text: -----");
			string inText = "We cant spel ACHindex 987Pfimbria dianosed.";
			//CSpellApi cSpellApi = new CSpellApi(configFile);
			List<TokenObj> inTokenList = TextObj.TextToTokenList(inText);
			List<TokenObj> outTokenList = ProcessByTokenObj(inTokenList, cSpellApi);
			string outText = TextObj.TokenListToText(outTokenList);
			// print out
			Console.WriteLine("--------------------");
			Console.WriteLine("In: [" + inText + "]");
			Console.WriteLine("Out: [" + outText + "]");
			Console.WriteLine("----- Details -----------");
			int index = 0;
			foreach (TokenObj tokenObj in outTokenList) {
				Console.WriteLine(index + "|" + tokenObj.ToString());
				index++;
			}
		}
		// test driver
		public static void MainTest(string[] args) {
			string configFile = "../data/Config/cSpell.properties";
			if (args.Length == 1) {
				configFile = args[0];
			} else if (args.Length > 0) {
				Console.WriteLine("Usage: java CorrectionApi <configFile>");
				Environment.Exit(0);
			}
			// init, read in from config
			CSpellApi cSpellApi = new CSpellApi(configFile);
			Test(cSpellApi);
		}
		// data member
		// processes related data
	}

}
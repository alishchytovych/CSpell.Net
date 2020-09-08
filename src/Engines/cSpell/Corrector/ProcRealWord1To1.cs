using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Candidates;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Ranker;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Corrector {
	/// <summary>
	///***************************************************************************
	/// This class is to process real-word 1-to-1 correction.
	/// 
	/// <para><b>History:</b>
	/// <ul>
	/// <li>2018 baseline
	/// <li>SCR-3, chlu, 03-05-19, add word length for legit token
	/// </ul>
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ****************************************************************************
	/// </para>
	/// </summary>
	public class ProcRealWord1To1 {
		// private constructor
		/// <summary>
		/// Private constructor so no one can instantiate
		/// </summary>
		private ProcRealWord1To1() { }
		// public method
		// Use: for loop, the latest and greatest implementation
		// original implementation with for loop, To be deleted
		// the core of spell-correction, include split
		// inTokenList is the whole text
		public static List<TokenObj> Process(List<TokenObj> inTokenList, CSpellApi cSpellApi, bool debugFlag) {
			DebugPrint.PrintProcess("7. RealWord-1To1", debugFlag);
			DebugPrint.PrintInText(TextObj.TokenListToText(inTokenList), debugFlag);
			// init the output TokenList
			List<TokenObj> outTokenList = new List<TokenObj>();
			// process: go through each token for detection and correction
			// for real-word 1-to-1 correction
			int tarPos = 0; // the position of the tokenObj in the inTokenList
			// remove space token from the list
			List<TokenObj> nonSpaceTokenList = TextObj.GetNonSpaceTokenObjList(inTokenList);
			// use the inTokenList to keep the same space token
			TokenObj outTokenObj = null;
			int maxLegitTokenLength = cSpellApi.GetMaxLegitTokenLength();
			foreach (TokenObj tokenObj in inTokenList) {
				// SCR-3, use legit token
				if (tokenObj.IsLegitToken(maxLegitTokenLength) == true) {
					// correct term is the highest ranked candidate
					outTokenObj = RealWord1To1Corrector.GetCorrectTerm(tokenObj, cSpellApi, debugFlag, tarPos, nonSpaceTokenList);
					// used tarPos for context score
					tarPos++;
				} else { // skip space tokens
					outTokenObj = tokenObj;
				}
				// add the corrected tokenObj to the output token list
				// use FlatMap because there might be a split
				// TBD ...
				Split1To1Corrector.AddSplit1To1Correction(outTokenList, outTokenObj);
			}
			return outTokenList;
		}
		private static void TestProcess(CSpellApi cSpellApi) {
			// init
			// all lowerCase
			string inText = "You would thing that this is good.";
			List<TokenObj> inTokenList = TextObj.TextToTokenList(inText);
			bool debugFlag = false;
			// process
			List<TokenObj> outTokenList = Process(inTokenList, cSpellApi, debugFlag);
			// result
			string outText = TextObj.TokenListToText(outTokenList);
			// print out
			Console.WriteLine("------ GetCorrection by Process( ) ------");
			Console.WriteLine("In: [" + inText + "]");
			Console.WriteLine("Out: [" + outText + "]");
			Console.WriteLine("----- Details -----------");
			// print out operation details
			Console.WriteLine(TextObj.TokenListToOperationDetailStr(outTokenList));
		}
		// test driver
		public static void MainTest(string[] args) {
			string configFile = "../data/Config/cSpell.properties";
			if (args.Length == 1) {
				configFile = args[0];
			} else if (args.Length > 0) {
				Console.WriteLine("Usage: java ProcessNonWord1To1 <configFile>");
				Environment.Exit(0);
			}

			// init
			CSpellApi cSpellApi = new CSpellApi(configFile);
			// test
			TestProcess(cSpellApi);
		}
		// data member
	}

}
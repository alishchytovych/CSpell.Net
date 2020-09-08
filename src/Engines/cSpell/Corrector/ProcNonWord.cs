using System;
using System.Collections.Generic;
using System.Linq;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Candidates;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Ranker;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Corrector {

	/// <summary>
	///***************************************************************************
	/// This class is to process non-word 1To1 and split correction.
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
	public class ProcNonWord {
		// private constructor
		/// <summary>
		/// Private constructor so no one can instantiate
		/// </summary>
		private ProcNonWord() { }
		// public method
		// Use: for loop, the latest and greatest implementation
		// original implementation with for loop, To be deleted
		// the core of spell-correction, include split
		// inTokenList is the whole text
		public static List<TokenObj> Process(List<TokenObj> inTokenList, CSpellApi cSpellApi, bool debugFlag) {
			DebugPrint.PrintProcess("3-4. NonWord-Split & 1To1", debugFlag);
			DebugPrint.PrintInText(TextObj.TokenListToText(inTokenList), debugFlag);
			// init the output TokenList
			List<TokenObj> outTokenList = new List<TokenObj>();
			// process: go through each token for detection and correction
			// for the 1-to-1 and split correction
			int tarPos = 0; // the position of the tokenObj in the inTokenList
			// remove space token from the list
			List<TokenObj> nonSpaceTokenList = TextObj.GetNonSpaceTokenObjList(inTokenList);
			// use the inTokenList to keep the same spcae token
			TokenObj outTokenObj = null;
			int maxLegitTokenLength = cSpellApi.GetMaxLegitTokenLength();
			foreach (TokenObj tokenObj in inTokenList) {
				/// <summary>
				/// no context
				/// TokenObj outTokenObj = SpellCorrector.GetCorrectTerm(tokenObj,
				///    cSpellApi, debugFlag);
				///    
				/// </summary>
				// skip empty space tokens and long tokens
				// SCR-3, use legit token
				if (tokenObj.IsLegitToken(maxLegitTokenLength) == true) {
					// correct term is the highest ranked candidate
					outTokenObj = NonWordCorrector.GetCorrectTerm(tokenObj, cSpellApi, debugFlag, tarPos, nonSpaceTokenList);
					// used tarPos for context module
					tarPos++;
				} else {
					outTokenObj = tokenObj;
				}
				// add the corrected tokenObj to the output token list
				// use FlatMap because there might be a split
				Split1To1Corrector.AddSplit1To1Correction(outTokenList, outTokenObj);
			}
			return outTokenList;
		}

		private static void TestProcess(CSpellApi cSpellApi) {
			// init
			// all lowerCase
			string inText = "hotflashes and knowaboutare not forr playsure.";
			// test process:  must use ArrayList<TextObj>
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
			if (args.Length > 0) {
				Console.WriteLine("Usage: java ProcessNonWord <configFile>");
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
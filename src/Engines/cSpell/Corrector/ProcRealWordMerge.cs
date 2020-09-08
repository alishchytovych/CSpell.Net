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
	/// This class is to process real-word merge correction.
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
	public class ProcRealWordMerge {
		// private constructor
		/// <summary>
		/// Private constructor so no one can instantiate
		/// </summary>
		private ProcRealWordMerge() { }
		// public method
		// process
		public static List<TokenObj> Process(List<TokenObj> inTokenList, CSpellApi cSpellApi, bool debugFlag) {
			DebugPrint.PrintProcess("5. RealWord-Merge", debugFlag);
			DebugPrint.PrintInText(TextObj.TokenListToText(inTokenList), debugFlag);
			// pre-porcess
			// update Pos for the inTokenList
			TextObj.UpdateIndexPos(inTokenList);
			// 1. remove non space-token and convert to non-space-token list
			List<TokenObj> nonSpaceTokenList = TextObj.GetNonSpaceTokenObjList(inTokenList);
			// 2. process: go through each token for detection and correction
			// to find merge corrections (mergeObjList)
			int index = 0;
			List<MergeObj> mergeObjList = new List<MergeObj>();
			int maxLegitTokenLength = cSpellApi.GetMaxLegitTokenLength();
			while (index < inTokenList.Count) {
				TokenObj curTokenObj = inTokenList[index];

				// update the tarPos
				// SCR-3, use legit token
				if (curTokenObj.IsLegitToken(maxLegitTokenLength) == true) {
					int tarPos = inTokenList[index].GetPos();
					// correct term is the highest ranked candidates
					MergeObj mergeObj = RealWordMergeCorrector.GetCorrectTerm(tarPos, nonSpaceTokenList, cSpellApi, debugFlag);
					if (mergeObj == null) { // no merge correction
						index++;
					} else { // has merge correction
						mergeObjList.Add(mergeObj);
						// next token after end token, this ensures no overlap merge
						index = mergeObj.GetEndIndex() + 1;
					}
				} else { // space token
					// update index 
					index++;
				}
			}
			// update the output for merge for the whole inTokenList, 
			// has to update after the loop bz merge might 
			// happen to the previous token
			// update the tokenObj up to the merge, then go to the next token
			// update operation info also
			List<TokenObj> outTokenList = MergeCorrector.CorrectTokenListByMerge(inTokenList, mergeObjList, TokenObj.HIST_RW_M, debugFlag, cSpellApi);
			return outTokenList;
		}
		private static void TestProcess(CSpellApi cSpellApi) {
			// init
			// all lowerCase
			//String inText = "She had problems dur ing her pregnancies. That is a dis appoint ment. Good!";
			string inText = "He was diagnosed early on set deminita 3 year ago.";
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
				Console.WriteLine("Usage: java SpellCorrection <configFile>");
				Environment.Exit(0);
			}

			// init
			CSpellApi cSpellApi = new CSpellApi(configFile);
			// test
			TestProcess(cSpellApi);
		}
		// data member
		private const string INFO_M = "MERGE";
	}

}
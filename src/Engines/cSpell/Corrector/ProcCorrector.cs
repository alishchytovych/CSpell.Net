using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Candidates;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.NdCorrector;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Ranker;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Corrector {
	/// <summary>
	///***************************************************************************
	/// This class is to core program to process correction.
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
	public class ProcCorrector {
		// private constructor
		/// <summary>
		/// Private constructor so no one can instantiate
		/// </summary>
		private ProcCorrector() { }
		// public method
		public static List<TokenObj> ProcessByTokenObj(List<TokenObj> inTokenList, CSpellApi cSpellApi, bool debugFlag) {
			// fucnMode
			int funcMode = cSpellApi.GetFuncMode();
			// init non-dictionary correction, this process is always on
			int ndMaxSplitNo = cSpellApi.GetCanNdMaxSplitNo(); // ND split
			Dictionary<string, string> infExpMap = cSpellApi.GetInformalExpressionMap();
			List<TokenObj> ndTokenList = ProcNdCorrector.Process(inTokenList, ndMaxSplitNo, infExpMap, debugFlag);
			List<TokenObj> outTokenList = null;
			switch (funcMode) {
				case CSpellApi.FUNC_MODE_ND:
					outTokenList = ndTokenList;
					break;
					// TBD, NW 1-to-1 and split need to be seperated    
				case CSpellApi.FUNC_MODE_NW_1:
				case CSpellApi.FUNC_MODE_NW_S:
				case CSpellApi.FUNC_MODE_NW_S_1:
					// non-word one-to-one and split
					outTokenList = ProcNonWord.Process(ndTokenList, cSpellApi, debugFlag);
					break;
				case CSpellApi.FUNC_MODE_NW_M:
					// non-word merge
					outTokenList = ProcNonWordMerge.Process(ndTokenList, cSpellApi, debugFlag);
					break;
					// non-word all	
				case CSpellApi.FUNC_MODE_NW_A:
					// 1. non-word merge
					List<TokenObj> nwMergeList = ProcNonWordMerge.Process(ndTokenList, cSpellApi, debugFlag);
					// 2. non-word one-to-one and split
					outTokenList = ProcNonWord.Process(nwMergeList, cSpellApi, debugFlag);
					break;
					// real-word one-to-one
				case CSpellApi.FUNC_MODE_RW_1:
					// 1. non-word merge
					nwMergeList = ProcNonWordMerge.Process(ndTokenList, cSpellApi, debugFlag);
					// 2. non-word one-to-one and split
					List<TokenObj> nw1To1SplitList = ProcNonWord.Process(nwMergeList, cSpellApi, debugFlag);
					// 3. real-word, 1-to-1 ...    
					outTokenList = ProcRealWord1To1.Process(nw1To1SplitList, cSpellApi, debugFlag);
					break;
					// real-word split
				case CSpellApi.FUNC_MODE_RW_S:
					// 1. non-word merge
					nwMergeList = ProcNonWordMerge.Process(ndTokenList, cSpellApi, debugFlag);
					// 2. non-word one-to-one and split
					nw1To1SplitList = ProcNonWord.Process(nwMergeList, cSpellApi, debugFlag);
					// 3. real-word one-to-one and split
					outTokenList = ProcRealWordSplit.Process(nw1To1SplitList, cSpellApi, debugFlag);
					break;
					// real-word merge
				case CSpellApi.FUNC_MODE_RW_M:
					// 1. non-word merge
					nwMergeList = ProcNonWordMerge.Process(ndTokenList, cSpellApi, debugFlag);
					// 2. non-word one-to-one and split
					nw1To1SplitList = ProcNonWord.Process(nwMergeList, cSpellApi, debugFlag);
					// 3. real-word merge
					outTokenList = ProcRealWordMerge.Process(nw1To1SplitList, cSpellApi, debugFlag);
					break;
					// real-word merge and split
				case CSpellApi.FUNC_MODE_RW_M_S:
					// 1. non-word merge
					nwMergeList = ProcNonWordMerge.Process(ndTokenList, cSpellApi, debugFlag);
					// 2. non-word one-to-one and split
					nw1To1SplitList = ProcNonWord.Process(nwMergeList, cSpellApi, debugFlag);
					// 3. real-word merge
					List<TokenObj> rwMergeList = ProcRealWordMerge.Process(nw1To1SplitList, cSpellApi, debugFlag);
					// 4. real-word split
					outTokenList = ProcRealWordSplit.Process(rwMergeList, cSpellApi, debugFlag);
					break;
					// real-word all: merge, split, 1-to-1
				case CSpellApi.FUNC_MODE_RW_A:
					// 1. non-word merge
					nwMergeList = ProcNonWordMerge.Process(ndTokenList, cSpellApi, debugFlag);
					// 2. non-word one-to-one and split
					nw1To1SplitList = ProcNonWord.Process(nwMergeList, cSpellApi, debugFlag);
					// 3. real-word merge
					rwMergeList = ProcRealWordMerge.Process(nw1To1SplitList, cSpellApi, debugFlag);
					// 4. real-word split
					List<TokenObj> rwSplitList = ProcRealWordSplit.Process(rwMergeList, cSpellApi, debugFlag);
					// 5. real-word, 1-to-1
					outTokenList = ProcRealWord1To1.Process(rwSplitList, cSpellApi, debugFlag);
					break;
			}
			return outTokenList;
		}
		// private method
		private static void TestProcess(CSpellApi cSpellApi) {
			// init
			// test non-word, one-to-one, split, and merge correction, all lowerCase
			string inText = "hotflashes and knowaboutare not forr playsure dur ing my disa ppoint ment.";
			// test process:  must use ArrayList<TextObj>
			List<TokenObj> inTokenList = TextObj.TextToTokenList(inText);
			bool debugFlag = false;
			// process
			List<TokenObj> outTokenList = ProcessByTokenObj(inTokenList, cSpellApi, debugFlag);
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
	}

}
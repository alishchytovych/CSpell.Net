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
	/// This class is to correct merge and update the in token list by the specified
	/// mergeObjList and inTokenList.
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
	public class MergeCorrector {
		// private constructor
		/// <summary>
		/// Private constructor so no one can instantiate
		/// </summary>
		private MergeCorrector() { }
		// clean up mergeObjList:
		// 1. contain, remove the previous one
		// 2. overlap, remove the latter one
		// This is a quick fix for window = 2. the permanemnt fix should be a 
		// real-time update on each merge
		private static List<MergeObj> CleanUpMergeObjList(List<MergeObj> mergeObjList) {
			List<MergeObj> outMergeObjList = new List<MergeObj>();
			bool skipNext = false;
			for (int i = 0; i < mergeObjList.Count; i++) {
				MergeObj mergeObj1 = mergeObjList[i];
				if (i < mergeObjList.Count - 1) {
					MergeObj mergeObj2 = mergeObjList[i + 1]; // next mergeObj
					int startPos1 = mergeObj1.GetStartPos();
					int startPos2 = mergeObj2.GetStartPos();
					int endPos1 = mergeObj1.GetEndPos();
					int endPos2 = mergeObj2.GetEndPos();
					// mergeObj2 contains mergeObj1
					if ((startPos1 == startPos2) && (endPos1 < endPos2)) {
						continue;
					}
					// merObj2 has overlap with mergeObj1
					else if ((startPos2 > startPos1) && (startPos2 < endPos1)) {
						outMergeObjList.Add(mergeObj1);
						skipNext = true;
					} else {
						if (skipNext == true) {
							skipNext = false;
						} else {
							outMergeObjList.Add(mergeObj1);
						}
					}
				} else {
					// add the last mergeObj
					if (skipNext == false) {
						outMergeObjList.Add(mergeObj1);
					}
				}
			}
			return outMergeObjList;
		}
		// public method
		// the input mergeObjList is in the same order of index as inTokenList
		// TBD: has bug: "imple ment ation" => implementimplementation
		public static List<TokenObj> CorrectTokenListByMerge(List<TokenObj> inTokenList, List<MergeObj> mergeObjList, string procHistStr, bool debugFlag, CSpellApi cSpellApi) {
			// 0. unify the mergeObjList to remove contain and overlap
			List<MergeObj> mergeObjListC = CleanUpMergeObjList(mergeObjList);

			List<TokenObj> outTokenList = new List<TokenObj>();
			// 1. go through all mergeObj
			int curIndex = 0;
			foreach (MergeObj mergeObj in mergeObjListC) {
				//System.out.println(mergeObj.ToString());
				int startIndex = mergeObj.GetStartIndex();
				int endIndex = mergeObj.GetEndIndex();
				// 1. update tokens before merge start
				for (int i = curIndex; i < startIndex; i++) {
					outTokenList.Add(inTokenList[i]);
				}
				// 2. update merge at target
				string mergeWord = mergeObj.GetMergeWord();
				string orgMergeWord = mergeObj.GetOrgMergeWord();
				string tarWord = mergeObj.GetTarWord();
				TokenObj mergeTokenObj = new TokenObj(orgMergeWord, mergeWord);
				// update process history
				for (int i = startIndex; i <= endIndex; i++) {
					// merge focus token
					if (i == mergeObj.GetTarIndex()) {
						cSpellApi.UpdateCorrectNo();
						mergeTokenObj.AddProcToHist(procHistStr + TokenObj.MERGE_START_STR + tarWord + TokenObj.MERGE_END_STR);
						//DebugPrint.PrintCorrect("NW", 
						DebugPrint.PrintCorrect(procHistStr, "MergeCorrector (" + tarWord + ")", orgMergeWord, mergeWord, debugFlag);
					} else { // not merge focus token, context
						TokenObj contextToken = inTokenList[i];
						List<string> contextProcHist = contextToken.GetProcHist();
						foreach (string procHist in contextProcHist) {
							mergeTokenObj.AddProcToHist(procHist + TokenObj.MERGE_START_STR + contextToken.GetTokenStr() + TokenObj.MERGE_END_STR);
						}
					}
				}
				outTokenList.Add(mergeTokenObj);
				curIndex = endIndex + 1;
			}
			// 2. add tokens after the last merge Obj
			for (int i = curIndex; i < inTokenList.Count; i++) {
				outTokenList.Add(inTokenList[i]);
			}
			return outTokenList;
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java MergeCorrector <configFile>");
				Environment.Exit(0);
			}

			// init
		}
		// data member
	}

}
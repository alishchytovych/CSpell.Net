using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.Ranker;

namespace SpellChecker.Engines.cSpell.Candidates {
	/// <summary>
	///***************************************************************************
	/// This class generates non-word 1To1 candidates.
	/// 
	/// <para><b>History:</b>
	/// <ul>
	/// <li>2018 baseline
	/// </ul>
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ***************************************************************************
	/// </para>
	/// </summary>
	public class NonWord1To1Candidates {
		// private constructor
		private NonWord1To1Candidates() { }
		// public method
		// Get candidates from dictionary by Edit-distance:
		// 1. get all possible combinations from insert, remove, replace, switch
		//    chars. However, it does not include space (so no split).
		// 2. check if the combination is in dictionary
		public static HashSet<string> GetCandidates(string inWord, CSpellApi cSpellApi) {
			int maxLength = cSpellApi.GetCanNw1To1WordMaxLength();
			// find all possibility
			HashSet<string> candidatesByEd = CandidatesUtil1To1.GetCandidatesByEd(inWord, maxLength);
			// filter out those are not valid words
			HashSet<string> candidates = new HashSet<string>();
			foreach (string candByEd in candidatesByEd) {
				// check if valid one-to-one candidate word
				if (IsValid1To1Cand(inWord, candByEd, cSpellApi) == true) {
					candidates.Add(candByEd);
				}
			}
			return candidates;
		}
		private static bool IsValid1To1Cand(string inWord, string cand, CSpellApi cSpellApi) {
			RootDictionary suggestDic = cSpellApi.GetSuggestDic();
			// real-word, check phonetic and suggDic
			// non-word, check if it is in the suggestion Dic
			bool flag = suggestDic.IsDicWord(cand);
			return flag;
		}
		private static void Test(string inStr) {
			// get candidates with dictionary
			string configFile = "../data/Config/cSpell.properties";
			CSpellApi cSpellApi = new CSpellApi(configFile);
			HashSet<string> candSet = GetCandidates(inStr, cSpellApi);
			Console.WriteLine("-- canSet.size(): " + candSet.Count);
			Console.WriteLine(candSet);
		}
		// test driver
		public static void MainTest(string[] args) {
			string inStr = "abc";
			if (args.Length == 1) {
				inStr = args[0];
			} else if (args.Length > 0) {
				Console.Error.WriteLine("*** Usage: java Candidates <inStr>");
				Environment.Exit(1);
			}
			Test(inStr);
		}
	}

}
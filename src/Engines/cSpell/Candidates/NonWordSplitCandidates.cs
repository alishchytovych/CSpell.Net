using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Extensions;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Ranker;

namespace SpellChecker.Engines.cSpell.Candidates {
	/// <summary>
	///***************************************************************************
	/// This class generates non-word split candidates. 
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
	public class NonWordSplitCandidates {
		// private constructor
		private NonWordSplitCandidates() { }
		// public method
		// filter out with dictionary
		// Use no Abb/Acr dictionary to exclude terms are abb/acr
		// The inWord must be a coreTerm.
		public static HashSet<string> GetCandidates(string inWord, CSpellApi cSpellApi, int maxSplitNo) {
			// init from cSpellApi
			RootDictionary mwDic = cSpellApi.GetMwDic();
			// 1. find all possibie split combination by spaces
			// must be <= maxSplitNo
			HashSet<string> splitSet = CandidatesUtilSplit.GetSplitSet(inWord, maxSplitNo);
			// filter out those are OOV
			HashSet<string> candidates = new HashSet<string>();
			// 2. multiwords: check the whole list of split terms
			// only inlcude dictionary that have multiword - lexicon
			// TBD: this will find "perse" to "per se", however, "perse" is
			// a valid word in eng_medical.dic so cSpell can't correct it.
			// Need to refine the dictionary later!
			foreach (string split in splitSet) {
				if (mwDic.IsDicWord(split) == true) {
					candidates.Add(split);
				}
			}
			// 3. if no multiwords found from step 2.
			// check each split terms, mark as candidate if they are in Dic,
			// Acr/Abb are excluded to eliminate noise such as 'a', 'ab', etc.
			if (candidates.Count == 0) {
				foreach (string split in splitSet) {
					// add to candidate if all split words are valid
					if (IsValidSplitWords(split, cSpellApi) == true) {
						candidates.Add(split);
					}
				}
			}
			return candidates;
		}
		// check all split words form a term to verify it is a valid
		// inTerm is the term to be split
		// the inTerm is a coreTerm
		public static bool IsValidSplitWords(string inTerm, CSpellApi cSpellApi) {
			//RootDictionary unitDic = cSpellApi.GetUnitDic();
			List<string> splitWordList = TermUtil.ToWordList(inTerm);
			bool validFlag = true;
			// go through all split words, they can be:
			// 1. digit (pure number)
			// 2. unit
			// 3. word in the split word dictionary: English + ProperNoun (not Aa)
			// if any splitWord is not above, the split is false
			foreach (string splitWord in splitWordList) {
				/* remove unit and digit beacuse:
				 * 1. they are handled in ND
				 * 2. some unit are Aa, such as ng, cause noise [FP]
				 * - seing => se i ng, no good
				if((DigitPuncTokenUtil.IsDigit(splitWord) == false) // digit 
				&& (unitDic.IsDicWord(splitWord) == false) // unit
				&& (IsValidSplitWord(splitWord, cSpellApi) == false))// split word
				*/
				if (IsValidSplitWord(splitWord, cSpellApi) == false) {
					validFlag = false;
					break;
				}
			}
			return validFlag;
		}
		// for the split, we don't want Aa as a valid word
		// because it will cause too much noise (less precision)
		private static bool IsValidSplitWord(string inWord, CSpellApi cSpellApi) {
			// splitWord uses LexiconNoAa for Dic
			RootDictionary splitWordDic = cSpellApi.GetSplitWordDic();
			// 1. check if in the splitWordDic, No Aa
			bool flag = splitWordDic.IsDicWord(inWord);
			// 2. is obsolete code because Aa is check in splitWordDic
			// 2. check pure Aa, further remove Aa
			// pureAaDic are words exlcude those overlap with not-Aa
			// such as cel is an overlap, it is aa or not-aa
			if (flag == true) {
				// if Aa and length < Mix. Split Aa word length
				// Set minSplitAaWordLength to a large number for excluding all paa
				//
				// This is already done in splitWordDic
				// no need, it reduced recall and precision (ofcourse => incourse)
				/// <summary>
				/// if((inWord.length() < minSplitAaWordLength)
				/// && (aaDic.IsDicWord(inWord) == true))
				/// {
				///    flag = false;
				/// }
				/// 
				/// </summary>
			}

			return flag;
		}
		// test all split combonation
		private static void TestSplitCandidates(string inStr) {
			// init dictionary
			string configFile = "../data/Config/cSpell.properties";
			CSpellApi cSpellApi = new CSpellApi(configFile);
			int maxSplitNo = cSpellApi.GetCanNwMaxSplitNo();
			// test 2 for candidate
			Console.WriteLine("====== test candidates (with Dic check) ======");
			Console.WriteLine("----- Final Candidate for split -----");
			Console.WriteLine("----- inStr: [" + inStr + "]");
			HashSet<string> candSet1 = GetCandidates(inStr, cSpellApi, maxSplitNo);
			Console.WriteLine("-- canSet1.size(): " + candSet1.Count);
			Console.WriteLine(candSet1.ToStringList());
			// other tests 3 for not multiword case
			Console.WriteLine("====== test candidates (with Dic check) ======");
			inStr = "perse";
			Console.WriteLine("----- inStr: [" + inStr + "]");
			HashSet<string> candSet11 = GetCandidates(inStr, cSpellApi, maxSplitNo);
			Console.WriteLine("-- canSet11.size(): " + candSet11.Count);
			Console.WriteLine(candSet11.ToStringList());
			Console.WriteLine("-------------------------------------");
			// other test 4 for not multiword case
			Console.WriteLine("====== test candidates (with Dic check) ======");
			inStr = "iloveyou";
			Console.WriteLine("----- inStr: [" + inStr + "]");
			HashSet<string> candSet2 = GetCandidates(inStr, cSpellApi, maxSplitNo);
			Console.WriteLine("-- canSet2.size(): " + candSet2.Count);
			Console.WriteLine(candSet2.ToStringList());
			Console.WriteLine("-------------------------------------");
		}
		// test driver
		public static void MainTest(string[] args) {
			// example: knowabout, viseversa, hotflashes, testsplit,
			// Amlodipine5mgs
			string inStr = "Amlodipine5mgs";
			int maxSplitNo = 2;
			if (args.Length == 1) {
				inStr = args[0];
			} else if (args.Length > 0) {
				Console.Error.WriteLine("*** Usage: java SplitCandidates <inStr>");
				Environment.Exit(1);
			}
			// 1. test
			TestSplitCandidates(inStr);
		}
	}

}
using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Ranker;

namespace SpellChecker.Engines.cSpell.Candidates {

	/// <summary>
	///***************************************************************************
	/// This class generates real-word split candidates 
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
	public class RealWordSplitCandidates {
		// private constructor
		private RealWordSplitCandidates() { }
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
			// filter out those are not valid
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
				// go through each split words
				foreach (string split in splitSet) {
					// add to candidate if all split words are valid
					if (IsValidSplitCand(split, cSpellApi) == true) {
						candidates.Add(split);
					}
				}
			}
			return candidates;
		}
		// check all split words form a term to verify it is a valid
		// inTerm is the term to be split
		// the inTerm is a coreTerm
		private static bool IsValidSplitCand(string inTerm, CSpellApi cSpellApi) {
			// 1. check the sort split words
			// 2. check Split words
			bool validFlag = ((CheckShortSplitWords(inTerm, cSpellApi) == true) && (CheckSplitWords(inTerm, cSpellApi) == true));
			return validFlag;
		}
		// check all split words
		private static bool CheckSplitWords(string inTerm, CSpellApi cSpellApi) {
			// convert to word list
			List<string> splitWordList = TermUtil.ToWordList(inTerm);
			// go through all split words, they can be:
			// 1. digit (pure number)
			// 2. unit
			// 3. word in the split word dictionary: English + ProperNoun (not Aa)
			// if any splitWord is not above, the split is false
			bool flag = true;
			foreach (string splitWord in splitWordList) {
				// check each split word
				if (IsValidSplitWord(splitWord, cSpellApi) == false) {
					flag = false;
					break;
				}
			}
			return flag;
		}
		// These are hueristic rule for real-wrod split
		// check the total no of short word for split words in inTerm (candidate)
		// short word is configurable, such as 2 or 3
		// the total no of split shot word must less than a number, default is 2
		// This rule is added to filter out: some -> so me, 
		// filter out: another -> a not her (shortSplitWordNo = 3)
		// filter out: anyone -> any one (shortSplitWordNo = 2)
		// 1. keep: away -> a way (shortSplitWordNo = 1)
		// 2. filter: out soon -> so on (shortSplitWordNo = 2)
		// 3. filter: out anyway -> any way (shortSplitWordNo = 2)
		private static bool CheckShortSplitWords(string inTerm, CSpellApi cSpellApi) {
			// init
			int shortSplitWordLength = cSpellApi.GetCanRwShortSplitWordLength();
			int maxShortSplitWordNo = cSpellApi.GetCanRwMaxShortSplitWordNo();
			// convert to word list
			List<string> wordList = TermUtil.ToWordList(inTerm);
			bool flag = true;
			int shortSplitWordNo = 0; // total no of short split word 1
			foreach (string word in wordList) {
				// find shor word
				if (word.Length <= shortSplitWordLength) {
					shortSplitWordNo++;
				}
			}
			// check the total no of short split words (length <= 2)
			if (shortSplitWordNo >= maxShortSplitWordNo) {
				flag = false;
			}
			return flag;
		}
		// for the split, we don't want Aa as a valid word
		// because it will cause too much noise (less precision)
		// TBD ... re-organize
		private static bool IsValidSplitWord(string inWord, CSpellApi cSpellApi) {
			// splitWord uses LexiconNoAa for Dic
			RootDictionary splitWordDic = cSpellApi.GetSplitWordDic();
			WordWcMap wordWcMap = cSpellApi.GetWordWcMap();
			Word2Vec word2VecOm = cSpellApi.GetWord2VecOm();
			RootDictionary unitDic = cSpellApi.GetUnitDic();
			RootDictionary pnDic = cSpellApi.GetPnDic();
			//RootDictionary aaDic = cSpellApi.GetAaDic();
			int rwSplitCandMinWc = cSpellApi.GetCanRwSplitCandMinWc();
			// real-word cand split word must:
			// 1. check if in the splitWordDic, No Aa with a small length
			// such as cel is an overlap, it is aa or not-aa
			// 2. has word2Vec
			// 3. has WC
			// 4. not unit, mg -> ...
			// 5. not properNoun, human -> Hu man, where Hu is pn
			// children -> child ren, where ren is pn
			bool flag = (splitWordDic.IsDicWord(inWord)) && (word2VecOm.HasWordVec(inWord) == true) && (WordCountScore.GetWc(inWord, wordWcMap) >= rwSplitCandMinWc) && (!unitDic.IsDicWord(inWord)) && (!pnDic.IsDicWord(inWord));

			return flag;
		}
		private static void TestSplitCandidates(string inStr) {
			// init dictionary
			string configFile = "../data/Config/cSpell.properties";
			CSpellApi cSpellApi = new CSpellApi(configFile);
			int maxSplitNo = cSpellApi.GetCanRwMaxSplitNo();
			// test 2 for candidate
			Console.WriteLine("====== test candidates (with Dic check) ======");
			Console.WriteLine("----- Final Candidate for split -----");
			Console.WriteLine("----- inStr: [" + inStr + "]");
			HashSet<string> candSet1 = GetCandidates(inStr, cSpellApi, maxSplitNo);
			Console.WriteLine("-- canSet1.size(): " + candSet1.Count);
			Console.WriteLine(candSet1);
			// other tests 3 for not multiword case
			Console.WriteLine("====== test candidates (with Dic check) ======");
			inStr = "perse";
			Console.WriteLine("----- inStr: [" + inStr + "]");
			HashSet<string> candSet11 = GetCandidates(inStr, cSpellApi, maxSplitNo);
			Console.WriteLine("-- canSet11.size(): " + candSet11.Count);
			Console.WriteLine(candSet11);
			Console.WriteLine("-------------------------------------");
			// other test 4 for not multiword case
			Console.WriteLine("====== test candidates (with Dic check) ======");
			inStr = "iloveyou";
			Console.WriteLine("----- inStr: [" + inStr + "]");
			HashSet<string> candSet2 = GetCandidates(inStr, cSpellApi, maxSplitNo);
			Console.WriteLine("-- canSet2.size(): " + candSet2.Count);
			Console.WriteLine(candSet2);
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
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
	/// This utility class generates split candidates.
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
	public class CandidatesUtilSplit {
		// private constructor
		private CandidatesUtilSplit() { }
		// protected method
		// get candidates 
		// Do not split hyphen
		protected internal static HashSet<string> GetSplitSet(string inWord, int maxSplitNo) {
			bool hyphenSplitFlag = false;
			return GetSplitSet(inWord, maxSplitNo, hyphenSplitFlag);
		}
		// - split by space, 1 or 2 splits
		// - split by replacing hyphens to spaces
		protected internal static HashSet<string> GetSplitSet(string inWord, int maxSplitNo, bool hyphenSplitFlag) {
			// get split by space
			HashSet<string> splitSet = GetSplitSetBySpaces(inWord, maxSplitNo);
			// TBD: get split string by replacing hyphen by space
			// This feature is disable because the R+, P-, F-
			// Need look at the data to see hwo to improve!

			if (hyphenSplitFlag == true) {
				string splitStrByHyphen = GetSplitByPunc(inWord, '-');
				if (splitStrByHyphen.Equals(inWord) == false) {
					splitSet.Add(splitStrByHyphen);
				}
			}
			return splitSet;
		}
		// get possible split set by replacing hyphen with space
		protected internal static string GetSplitByPunc(string inWord, char puncChar) {
			char[] temp = inWord.ToCharArray();
			for (int i = 0; i < temp.Length; i++) {
				if (temp[i] == puncChar) {
					temp[i] = ' ';
				}
			}
			string splitStr = TermUtil.Trim(new string(temp));
			return splitStr;
		}
		// private methods
		// the inWord should be lowerCase()
		// return all possible split combo within edit distance of N
		// maxSplitNo must >= 1 to get come result
		protected internal static HashSet<string> GetSplitSetBySpaces(string inWord, int maxSplitNo) {
			HashSet<string> splitSet = new HashSet<string>();
			// check iuputs
			if ((string.ReferenceEquals(inWord, null)) || (inWord.Length == 0) || (maxSplitNo < 1)) {
				return splitSet;
			}
			HashSet<string> curSplitSet = GetSplitSetBy1Space(inWord);
			splitSet.addAll(curSplitSet);
			int spaceNo = 1;
			// recursively for more than 1 split by space
			while (spaceNo < maxSplitNo) {
				HashSet<string> nextSplitSet = new HashSet<string>();
				// generate next level of split based on current split
				foreach (string curSplit in curSplitSet) {
					HashSet<string> tempSplitSet = GetSplitSetBy1Space(curSplit);
					nextSplitSet.addAll(tempSplitSet);
				}
				// updates
				curSplitSet = new HashSet<string>(nextSplitSet);
				splitSet.addAll(nextSplitSet);
				spaceNo++;
			}
			return splitSet;
		}
		// get all possible split combination by 1 space
		// lowercase only
		// not include duplicates
		// This is the core split process by space
		protected internal static HashSet<string> GetSplitSetBy1Space(string inWord) {
			HashSet<string> splitSet = new HashSet<string>();
			string word = inWord.ToLower();
			// Insert space inside the word, not on either ends
			for (int i = 1; i < word.Length; i++) {
				// Insert space for split
				string insertWord = word.Substring(0, i) + GlobalVars.SPACE_STR + word.Substring(i);
				// remove multiple spaces    
				// needed when inserting a space to a space
				// Use this to convert "a  b" to "a b"
				splitSet.Add(TermUtil.StringTrim(insertWord));
			}
			return splitSet;
		}
		// test all split 
		private static void TestSplitUtil(string inStr) {
			Console.WriteLine("====== test split util (no Dic check) ======");
			Console.WriteLine("----- inStr: [" + inStr + "]");
			// Test possible split combination with 1 split
			HashSet<string> splitSet1 = GetSplitSetBy1Space(inStr);
			bool caseFlag = false; // not case sensitive
			int dupNo = 0;
			// print out
			Console.WriteLine("----- Check on candList1 -----");
			Console.WriteLine("-- splitSet1.size(): " + splitSet1.Count);
			Console.WriteLine(splitSet1.ToStringList());
			// check if the candList1 correct by edit distance
			foreach (string split1 in splitSet1) {
				int ed = EditDistance.GetEditDistance(inStr, split1, caseFlag);
				// these are errors: because 1 split should have ed = 1 
				if ((ed != 1) && (inStr.Equals(split1) == false)) {
					Console.WriteLine("**ERR: " + inStr + "|" + split1 + "|" + ed);
				}
				// candidate are same as inStr, produced by replace, not 0
				// check duplicate no.
				else if (inStr.Equals(split1) == true) {
					dupNo++;
				}
			}
			Console.WriteLine("-- dupNo: " + dupNo);
			HashSet<string> candSet1a = GetSplitSetBySpaces(inStr, 1);
			Console.WriteLine("-- candSet1a.size(): " + candSet1a.Count);
			Console.WriteLine(candSet1a.ToStringList());
			HashSet<string> candSet2a = GetSplitSetBySpaces(inStr, 2);
			Console.WriteLine("-- candList2a.size(): " + candSet2a.Count);
			Console.WriteLine(candSet2a.ToStringList());
			// other test 5 for spliting candidates by hyphen
			string testStr = "-123--45-test-123-45.6-";
			Console.WriteLine("- testStr: " + testStr);
			Console.WriteLine("- splitByHyphen: [" + GetSplitByPunc(testStr, '-') + "]");
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
				Console.Error.WriteLine("*** Usage: java SplitCandidatesUtil <inStr>");
				Environment.Exit(1);
			}
			// 1. test
			TestSplitUtil(inStr);
		}
	}

}
using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Extensions;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.Ranker;

namespace SpellChecker.Engines.cSpell.Candidates {
	/// <summary>
	///***************************************************************************
	/// This utility class generates 1To1 candidates from an input string.
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
	public class CandidatesUtil1To1 {
		// private constructor
		private CandidatesUtil1To1() { }
		// protected method
		// get candidates of edit distance 1 & 2
		// all candidates are operated in lowerCase 
		// inWord should be lowerCase
		// maxLength: do not proceed if length if too big to save time
		protected internal static HashSet<string> GetCandidatesByEd(string inWord, int maxLength) {
			HashSet<string> candidates = new HashSet<string>();
			if (inWord.Length <= maxLength) {
				candidates = GetCandidatesByEd1(inWord);
				HashSet<string> candList2 = GetCandidatesByEd2(candidates);
				candidates.addAll(candList2);
				// remove the inWordLc
				candidates.Remove(inWord.ToLower());
			}
			return candidates;
		}
		// private methods
		// the inWord must be lowerCase()
		// return all possible candidate within edit distance of 1
		private static HashSet<string> GetCandidatesByEd2(HashSet<string> candSet1) {
			// get all candidates with edit distance 2
			HashSet<string> candSet2 = new HashSet<string>();
			foreach (string cand1 in candSet1) {
				HashSet<string> candSet12 = GetCandidatesByEd1(cand1);
				candSet2.addAll(candSet12);
			}
			// remvoe the original term.LC
			return candSet2;
		}
		// lowercase only
		// inWord must be lowercase
		// Done: change the return to HashSet might increase the performance speed?
		protected internal static HashSet<string> GetCandidatesByEd1(string inWord) {
			HashSet<string> candSet = new HashSet<string>();
			string inWordLc = inWord.ToLower();
			// Delete
			for (int i = 0; i < inWordLc.Length; i++) {
				string deleteWord = inWordLc.Substring(0, i) + inWordLc.Substring(i + 1);
				candSet.Add(deleteWord);
			}
			// Insert
			for (int i = 0; i <= inWordLc.Length; i++) {
				// Insert: a - z
				for (char c = 'a'; c <= 'z'; c++) {
					string insertWord = inWordLc.Substring(0, i) + c.ToString() + inWordLc.Substring(i);
					candSet.Add(insertWord);
				}
				// Insert space
				/*
				String insertWord = inWordLc.substring(0, i) + GlobalVars.SPACE_STR
				    + inWordLc.substring(i);
				candSet.add(insertWord);
				*/
			}
			// replace: include the origianl inWord
			for (int i = 0; i < inWordLc.Length; i++) {
				for (char c = 'a'; c <= 'z'; c++) {
					string alterWord = inWordLc.Substring(0, i) + c.ToString() + inWordLc.Substring(i + 1);
					candSet.Add(alterWord);
				}
			}
			// transpose, ed: 1, 1.5,or 2.0?
			for (int i = 0; i < inWordLc.Length - 1; i++) {
				string transWord = inWordLc.Substring(0, i) + inWordLc.Substring(i + 1, (i + 2) - (i + 1)) + inWordLc.Substring(i, 1) + inWordLc.Substring(i + 2);
				candSet.Add(transWord);
			}
			// remvoe the original term.LC
			candSet.Remove(inWordLc);
			return candSet;
		}
		// private
		private static void Test(string inStr) {
			// candidates with ED: 1
			HashSet<string> candSet1 = GetCandidatesByEd1(inStr);
			// print out
			Console.WriteLine("-- inStr: [" + inStr + "]");
			Console.WriteLine("-- candSet1.size(): " + candSet1.Count);
			bool caseFlag = false; // not case sensitive
			int dupNo = 0;
			// check if the candList correct
			foreach (string cand1 in candSet1) {
				int ed = EditDistance.GetEditDistance(inStr, cand1, caseFlag);
				// these are errors, should not have any
				if ((ed != 1) && (inStr.Equals(cand1) == false)) {
					Console.WriteLine(inStr + "|" + cand1 + "|" + ed);
				}
				// candidate are  same as inStr, produced by replace, not 0
				if (inStr.ToLower().Equals(cand1) == true) {
					dupNo++;
				}
			}
			Console.WriteLine("-- dupNo: " + dupNo);
			//System.out.println(candSet1);
			// candidates with ED: 2
			/// <summary>
			/// HashSet<String> candSet2 = GetCandidatesByEd2(candList1);
			/// int exceedNo = 0;
			/// for(String cand2:candSet2)
			/// {
			///    int ed = EditDistance.GetEditDistance(inStr, cand2, caseFlag);
			///    if((ed > 2) && (inStr.equals(cand2) == false))
			///    {
			///        System.out.println(inStr + "|" + cand2 + "|" + ed);
			///        exceedNo++;
			///    }
			/// }
			/// System.out.println("-- candSet2.size(): " + candSet2.size()); 
			/// System.out.println("-- exceedNo: " + exceedNo); 
			/// 
			/// </summary>
		}
		// test driver
		public static void MainTest(string[] args) {
			string inStr = "Regards";
			if (args.Length == 1) {
				inStr = args[0];
			} else if (args.Length > 0) {
				Console.Error.WriteLine("*** Usage: java OneToOneCandidatesUtil <inStr>");
				Environment.Exit(1);
			}
			Test(inStr);
		}
	}

}
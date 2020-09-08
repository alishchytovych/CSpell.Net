using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Candidates;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Lib;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///***************************************************************************
	/// This class ranks and finds the best ranked candidates by FrequencyScore.
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
	public class RankByFrequency {
		// private constructor
		private RankByFrequency() { }
		// return candidate str list sorted by score, higher first
		public static List<string> GetCandidateStrList(HashSet<string> candidates, WordWcMap wordWcMap) {
			List<FrequencyScore> candScoreList = GetCandidateScoreList(candidates, wordWcMap);
			List<string> candStrList = new List<string>();
			foreach (FrequencyScore fs in candScoreList) {
				candStrList.Add(fs.GetWord());
			}
			return candStrList;
		}
		// return candidate scoreObj list sorted by score, higher first
		public static List<FrequencyScore> GetCandidateScoreList(HashSet<string> candidates, WordWcMap wordWcMap) {
			HashSet<FrequencyScore> candScoreSet = GetCandidateScoreSet(candidates, wordWcMap);
			List<FrequencyScore> candScoreList = new List<FrequencyScore>(candScoreSet);
			// sort the list, higher fo first
			FrequencyScoreComparator<FrequencyScore> fsc = new FrequencyScoreComparator<FrequencyScore>();
			candScoreList.Sort(fsc);
			return candScoreList;
		}
		// return candidate set with frequency score
		// wordWcMap is the word|WC map to calculate the score
		// Not sorted, because it is a set
		public static HashSet<FrequencyScore> GetCandidateScoreSet(HashSet<string> candidates, WordWcMap wordWcMap) {
			HashSet<FrequencyScore> candScoreSet = new HashSet<FrequencyScore>();
			// find scores for all candidates
			foreach (string cand in candidates) {
				FrequencyScore fs = new FrequencyScore(cand, wordWcMap);
				candScoreSet.Add(fs);
			}
			return candScoreSet;
		}
		// return the best ranked str from candidates using frequency score
		public static string GetTopRankStr(HashSet<string> candidates, WordWcMap wordWcMap) {
			string topRankStr = "";
			List<FrequencyScore> candScoreList = GetCandidateScoreList(candidates, wordWcMap);
			if (candScoreList.Count > 0) {
				topRankStr = candScoreList[0].GetWord();
			}
			return topRankStr;
		}
		// return the best ranked str from candidates using frequency score
		public static string GetTopRankStrByScore(HashSet<string> candidates, WordWcMap wordWcMap) {
			string topRankStr = "";
			double maxScore = 0.0;
			foreach (string cand in candidates) {
				FrequencyScore fs = new FrequencyScore(cand, wordWcMap);
				double score = fs.GetScore();
				if (score > maxScore) {
					topRankStr = cand;
					maxScore = score;
				}
			}
			return topRankStr;
		}
		// private methods
		private static int RunTest(bool detailFlag, long limitNo) {
			// init dic
			string configFile = "../data/Config/cSpell.properties";
			CSpellApi cSpellApi = new CSpellApi(configFile);
			cSpellApi.SetRankMode(CSpellApi.RANK_MODE_FREQUENCY);
			WordWcMap wordWcMap = cSpellApi.GetWordWcMap();
			// provide cmdLine interface
			int returnValue = 0;
			FrequencyScoreComparator<FrequencyScore> fsc = new FrequencyScoreComparator<FrequencyScore>();
			try {
				StreamReader stdInput = new StreamReader(Console.OpenStandardInput());
				try {
					string inText = null;
					Console.WriteLine("- Please input a text (type \"Ctl-d\" to quit) > ");
					while (!string.ReferenceEquals((inText = stdInput.ReadLine()), null)) {
						// ---------------------------------
						// Get spell correction on the input
						// ---------------------------------
						// get all possible candidates
						HashSet<string> candSet = NonWord1To1Candidates.GetCandidates(inText, cSpellApi);
						Console.WriteLine("-- canSet.size(): " + candSet.Count);
						// get final suggestion
						string topRankStr = GetTopRankStr(candSet, wordWcMap);
						Console.WriteLine("- top rank str: " + topRankStr);
						// print details
						if (detailFlag == true) {
							HashSet<FrequencyScore> candScoreSet = GetCandidateScoreSet(candSet, wordWcMap);
							Console.WriteLine("------ Suggestion List ------");
							var list = candScoreSet.OrderBy(x => x, fsc).Take((int) limitNo).Select(obj => obj.ToString()).ToList();
							foreach (var item in list)
								Console.WriteLine(item);
						}
					}
				} catch (Exception e2) {
					Console.Error.WriteLine(e2.Message);
					returnValue = -1;
				}
			} catch (Exception e) {
				Console.Error.WriteLine(e.Message);
				returnValue = -1;
			}
			return returnValue;
		}

		// test Driver
		public static void MainTest(string[] args) {
			bool detailFlag = false;
			long limitNo = 10;
			if (args.Length == 2) {
				string option = args[0];
				if (option.Equals("-d") == true) {
					detailFlag = true;
				}
				limitNo = long.Parse(args[1]);
			} else if (args.Length == 1) {
				string option = args[0];
				if (option.Equals("-d") == true) {
					detailFlag = true;
				}
			} else if (args.Length > 0) {
				Console.WriteLine("Usage: java RankByFrequency <-d> <limitNo>");
				Environment.Exit(0);
			}
			// test
			int returnValue = RunTest(detailFlag, limitNo);
			Environment.Exit(returnValue);
		}
		// data member
	}

}
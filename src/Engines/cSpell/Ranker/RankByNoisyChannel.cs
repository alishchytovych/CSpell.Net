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
	/// This class ranks and finds the best ranked candidates by Noisy Channel score.
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
	public class RankByNoisyChannel {
		// private constructor
		private RankByNoisyChannel() { }
		// return candidate str list sorted by wordNo score, higher first
		public static List<string> GetCandidateStrList(string wordStr, HashSet<string> candidates, WordWcMap wordWcMap, double wf1, double wf2, double wf3) {
			List<NoisyChannelScore> candScoreList = GetCandidateScoreList(wordStr, candidates, wordWcMap, wf1, wf2, wf3);
			List<string> candStrList = new List<string>();
			foreach (NoisyChannelScore ncs in candScoreList) {
				candStrList.Add(ncs.GetCandStr());
			}
			return candStrList;
		}
		// return candidate scoreObj list sorted by score, higher first
		public static List<NoisyChannelScore> GetCandidateScoreList(string wordStr, HashSet<string> candidates, WordWcMap wordWcMap, double wf1, double wf2, double wf3) {
			HashSet<NoisyChannelScore> candScoreSet = GetCandidateScoreSet(wordStr, candidates, wordWcMap, wf1, wf2, wf3);
			List<NoisyChannelScore> candScoreList = new List<NoisyChannelScore>(candScoreSet);
			// sort the set to list
			NoisyChannelScoreComparator<NoisyChannelScore> ncsc = new NoisyChannelScoreComparator<NoisyChannelScore>();
			candScoreList.Sort(ncsc);
			return candScoreList;
		}
		// return candidate set with noisy channel score
		// wordStr is the srcTxt used to calculate the score between it and cand
		public static HashSet<NoisyChannelScore> GetCandidateScoreSet(string wordStr, HashSet<string> candidates, WordWcMap wordWcMap, double wf1, double wf2, double wf3) {
			HashSet<NoisyChannelScore> candScoreSet = new HashSet<NoisyChannelScore>();
			foreach (string cand in candidates) {
				NoisyChannelScore ncs = new NoisyChannelScore(wordStr, cand, wordWcMap, wf1, wf2, wf3);
				candScoreSet.Add(ncs);
			}
			return candScoreSet;
		}
		// return the best ranked str from candidates using noisy channel score
		public static string GetTopRankStr(string inStr, HashSet<string> candidates, WordWcMap wordWcMap, double wf1, double wf2, double wf3) {
			string topRankStr = inStr;
			// get the sorted list
			List<NoisyChannelScore> candScoreList = GetCandidateScoreList(inStr, candidates, wordWcMap, wf1, wf2, wf3);
			if (candScoreList.Count > 0) {
				topRankStr = candScoreList[0].GetCandStr();
			}
			return topRankStr;
		}
		// private methods
		private static int RunTest(bool detailFlag, long limitNo) {
			// init dic
			string configFile = "../data/Config/cSpell.properties";
			CSpellApi cSpellApi = new CSpellApi(configFile);
			WordWcMap wordWcMap = cSpellApi.GetWordWcMap();
			double wf1 = cSpellApi.GetOrthoScoreEdDistFac();
			double wf2 = cSpellApi.GetOrthoScorePhoneticFac();
			double wf3 = cSpellApi.GetOrthoScoreOverlapFac();
			cSpellApi.SetRankMode(CSpellApi.RANK_MODE_NOISY_CHANNEL);
			// provide cmdLine interface
			int returnValue = 0;
			NoisyChannelScoreComparator<NoisyChannelScore> ncsc = new NoisyChannelScoreComparator<NoisyChannelScore>();
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
						string topRankStr = GetTopRankStr(inText, candSet, wordWcMap, wf1, wf2, wf3);
						Console.WriteLine("- top tank str: " + topRankStr);
						// print details
						if (detailFlag == true) {
							HashSet<NoisyChannelScore> candScoreSet = GetCandidateScoreSet(inText, candSet, wordWcMap, wf1, wf2, wf3);
							Console.WriteLine("------ Suggestion List ------");
							var list = candScoreSet.OrderBy(x => x, ncsc).Take((int) limitNo).Select(obj => obj.ToString()).ToList();
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
				Console.WriteLine("Usage: java RankByNoisyChannel <-d> <limitNo>");
				Environment.Exit(0);
			}
			// test
			int returnValue = RunTest(detailFlag, limitNo);
			Environment.Exit(returnValue);
		}
		// data member
	}

}
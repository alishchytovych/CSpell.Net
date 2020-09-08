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
	/// This class ranks and finds the best ranked candidates by OrthographicScore.
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
	public class RankByOrthographic {
		// private constructor
		private RankByOrthographic() { }
		// return candidate str list sorted by orthographic score, higher first
		public static List<string> GetCandidateStrList(string inStr, HashSet<string> candidates, double wf1, double wf2, double wf3) {
			List<OrthographicScore> candScoreList = GetCandidateScoreList(inStr, candidates, wf1, wf2, wf3);
			List<string> candStrList = new List<string>();
			foreach (OrthographicScore os in candScoreList) {
				candStrList.Add(os.GetTarStr());
			}
			return candStrList;
		}
		// specify the top no for the candidate list
		public static List<string> GetTopCandidateStrList(string inStr, HashSet<string> candidates, double wf1, double wf2, double wf3, int topNo) {
			List<OrthographicScore> candScoreList = GetCandidateScoreList(inStr, candidates, wf1, wf2, wf3);
			List<string> candStrList = new List<string>();
			int curNo = 0;
			//double lastScore = 0.0d;
			foreach (OrthographicScore os in candScoreList) {
				if (curNo < topNo) {
					//|| (os.GetScore() == lastScore))    // add if same score, > topNo
					candStrList.Add(os.GetTarStr());
					curNo++;
					//lastScore = os.GetScore();
				}
			}
			return candStrList;
		}
		// return candidate scoreObj list sorted by score, higher first
		public static List<OrthographicScore> GetCandidateScoreList(string inStr, HashSet<string> candidates, double wf1, double wf2, double wf3) {
			HashSet<OrthographicScore> candScoreSet = GetCandidateScoreSet(inStr, candidates, wf1, wf2, wf3);
			List<OrthographicScore> candScoreList = new List<OrthographicScore>(candScoreSet);
			OrthographicScoreComparator<OrthographicScore> osc = new OrthographicScoreComparator<OrthographicScore>();
			candScoreList.Sort(osc);
			return candScoreList;
		}
		// return candidate set with orthographic score
		// inStr is the srcTxt used to calculate the score between it and cand
		public static HashSet<OrthographicScore> GetCandidateScoreSet(string inStr, HashSet<string> candidates, double wf1, double wf2, double wf3) {
			HashSet<OrthographicScore> candScoreSet = new HashSet<OrthographicScore>();
			foreach (string cand in candidates) {
				OrthographicScore os = new OrthographicScore(inStr, cand, wf1, wf2, wf3);
				candScoreSet.Add(os);
			}
			return candScoreSet;
		}
		// return the best ranked str from candidates using orthographic score
		public static string GetTopRankStr(string inStr, HashSet<string> candidates, double wf1, double wf2, double wf3) {
			string topRankStr = inStr;
			// get the sorted list
			List<OrthographicScore> candScoreList = GetCandidateScoreList(inStr, candidates, wf1, wf2, wf3);
			if (candScoreList.Count > 0) {
				topRankStr = candScoreList[0].GetTarStr();
			}
			return topRankStr;
		}
		// private methods
		// not verified test
		private static int RunTest(bool detailFlag, long limitNo) {
			// init dic
			string configFile = "../data/Config/cSpell.properties";
			CSpellApi cSpellApi = new CSpellApi(configFile);
			double wf1 = cSpellApi.GetOrthoScoreEdDistFac();
			double wf2 = cSpellApi.GetOrthoScorePhoneticFac();
			double wf3 = cSpellApi.GetOrthoScoreOverlapFac();
			// provide cmdLine interface
			int returnValue = 0;
			OrthographicScoreComparator<OrthographicScore> osc = new OrthographicScoreComparator<OrthographicScore>();
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
						string topRankStr = GetTopRankStr(inText, candSet, wf1, wf2, wf3);
						Console.WriteLine("- top tank str: " + topRankStr);
						// print details
						if (detailFlag == true) {
							HashSet<OrthographicScore> candScoreSet = GetCandidateScoreSet(inText, candSet, wf1, wf2, wf3);
							Console.WriteLine("------ Suggestion List ------");
							var list = candScoreSet.OrderBy(x => x, osc).Take((int) limitNo).Select(obj => obj.ToString()).ToList();
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
				Console.WriteLine("Usage: java RankByOrthographic <-d> <limitNo>");
				Environment.Exit(0);
			}
			// test
			int returnValue = RunTest(detailFlag, limitNo);
			Environment.Exit(returnValue);
		}
		// data member
	}

}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Candidates;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///***************************************************************************
	/// This class ranks and finds the best ranked candidates by ContextSocre.
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
	public class RankByContext {
		// private constructor
		private RankByContext() { }
		public static string GetTopRankStr(string inStr, HashSet<string> candidates, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius) {
			bool debugFlag = false; // no debiug print out
			return GetTopRankStr(inStr, candidates, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, debugFlag);
		}
		// return the best ranked str from candidates using word2Vec score
		// inTokenList, includes space token, is not coreTerm.Lc
		// return the orignal inStr if no candidate has score > 0.0d
		public static string GetTopRankStr(string inStr, HashSet<string> candidates, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, bool debugFlag) {
			/// <summary>
			/// ArrayList<ContextScore> candScoreList
			///    = GetCandidateScoreList(candidates, tarPos, tarSize,
			///        nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord,
			///        contextRadius, debugFlag);
			/// 
			/// String topRankStr = candScoreList.get(0).GetTerm();
			/// 
			/// </summary>
			string topRankStr = inStr;
			// find sorted score list for each candidates ...
			List<ContextScore> candScoreList = GetCandidateScoreList(candidates, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, debugFlag);
			// the 0 element has the highest score because it is sorted        
			if (candScoreList.Count == 1) { // only 1 candidate, use it for nonWord
				topRankStr = candScoreList[0].GetTerm();
			} else if (candScoreList.Count > 0) { // multiple candidates
				// 2. Use scroe system
				// Check the score, no updated if the top score is 0.0
				// It works for top score is + or - 
				// if the top is 0.0, no updated because top cand is not in w2v
				// if top score is 0, we don't know if it is better than -
				if (candScoreList[0].GetScore() != 0.0d) {
					topRankStr = candScoreList[0].GetTerm();
				}
				// do nothing if the top is 0.0d
				/*
				for(ContextScore contextScore:candScoreList)
				{
				    if(contextScore.GetScore() != 0.0)
				    {
				        topRankStr = contextScore.GetTerm();
				    }
				}
				*/
			}
			return topRankStr;
		}
		// return candidate str only list sorted by score, higher first
		public static List<string> GetCandidateStrList(HashSet<string> candidates, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, bool debugFlag) {
			// find sorted score list for each candidates ...
			List<ContextScore> candScoreList = GetCandidateScoreList(candidates, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, debugFlag);
			List<string> candStrList = new List<string>();
			foreach (ContextScore cs in candScoreList) {
				candStrList.Add(cs.GetTerm());
			}
			return candStrList;
		}
		// return candidate scoreObj list sorted by score, higher first
		public static List<ContextScore> GetCandidateScoreList(HashSet<string> candidates, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, bool debugFlag) {
			// find score object set for each candidates ...
			HashSet<ContextScore> candScoreSet = GetCandidateScoreSet(candidates, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, debugFlag);
			// sorted by the score, higher go first
			List<ContextScore> candScoreList = new List<ContextScore>(candScoreSet);
			ContextScoreComparator<ContextScore> csc = new ContextScoreComparator<ContextScore>();
			candScoreList.Sort(csc);
			return candScoreList;
		}
		// return candidate set with context score
		// word2Vec is the word|wordVec map to get the wordVec 
		// Not sorted, because it is a set
		// tarPos: starting position of target token
		// tarSize: token size of target token (single word = 1)
		// contextRadius: windown radius
		public static HashSet<ContextScore> GetCandidateScoreSet(HashSet<string> candidates, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, bool debugFlag) {
			// 1. get the context and contextVec, using input matrix
			DoubleVec contextVec = Word2VecContext.GetContextVec(tarPos, tarSize, nonSpaceTokenList, word2VecIm, contextRadius, word2VecSkipWord, debugFlag);

			// 2. get context score for all candidates
			HashSet<ContextScore> candScoreSet = new HashSet<ContextScore>();
			foreach (string cand in candidates) {
				// get ContextSocre for each candidates, use output matrix
				ContextScore cs = new ContextScore(cand, contextVec, word2VecOm);
				candScoreSet.Add(cs);
			}
			return candScoreSet;
		}
		// return the best ranked str from candidates using context score
		// this method is replaced by GetTopRankStr, which sorted by comparator
		public static string GetTopRankStrByScore(string inStr, HashSet<string> candidates, int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, bool debugFlag) {
			// 1. get the context and contextVec
			DoubleVec contextVec = Word2VecContext.GetContextVec(tarPos, tarSize, nonSpaceTokenList, word2VecIm, contextRadius, word2VecSkipWord, debugFlag);
			string topRankStr = inStr;
			double maxScore = 0.0d;
			foreach (string cand in candidates) {
				ContextScore cs = new ContextScore(cand, contextVec, word2VecOm);
				double score = cs.GetScore();
				// update only if the score is > 0.0d
				if (score > maxScore) {
					topRankStr = cand;
					maxScore = score;
				}
			}
			return topRankStr;
		}
		// private methods
		// this test is not verified
		private static int RunTest(bool detailFlag, int tarPos, int tarSize, int contextRadius, long limitNo) {
			// init dic
			string configFile = "../data/Config/cSpell.properties";
			CSpellApi cSpellApi = new CSpellApi(configFile);
			cSpellApi.SetRankMode(CSpellApi.RANK_MODE_CONTEXT);
			Word2Vec word2VecIm = cSpellApi.GetWord2VecIm();
			Word2Vec word2VecOm = cSpellApi.GetWord2VecOm();
			bool word2VecSkipWord = cSpellApi.GetWord2VecSkipWord();
			ContextScoreComparator<ContextScore> csc = new ContextScoreComparator<ContextScore>();
			// provide cmdLine interface
			int returnValue = 0;
			try {
				StreamReader stdInput = new StreamReader(Console.OpenStandardInput());
				try {
					string inText = null;
					Console.WriteLine("- Please input a text, only a spell error allowed (type \"Ctl-d\" to quit) > ");
					while (!string.ReferenceEquals((inText = stdInput.ReadLine()), null)) {
						// ---------------------------------
						// Get spell correction on the input
						// ---------------------------------
						// convert input text to TokenObj
						TextObj textObj = new TextObj(inText);
						List<TokenObj> inTextList = textObj.GetTokenList();
						// *2 because tokenList include space
						string tarWord = inTextList[tarPos * 2].GetTokenStr();
						for (int i = 1; i < tarSize; i++) {
							int ii = (tarPos + 1) * 2;
							tarWord += " " + inTextList[ii].GetTokenStr();
						}
						Console.WriteLine("- input text: [" + inText + "]");
						Console.WriteLine("- target: [" + tarPos + "|" + tarSize + "|" + tarWord + "]");
						Console.WriteLine("- context radius: " + contextRadius);
						// get all possible candidates
						HashSet<string> candSet = NonWord1To1Candidates.GetCandidates(tarWord, cSpellApi);
						candSet.Add(tarWord); // add the original word
						Console.WriteLine("-- canSet.size(): " + candSet.Count);
						// get final suggestion
						// remove space token from the list
						List<TokenObj> nonSpaceTokenList = TextObj.GetNonSpaceTokenObjList(inTextList);
						string topRankStr = GetTopRankStr(tarWord, candSet, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, detailFlag);
						Console.WriteLine("- top rank str: " + topRankStr);
						// print details
						if (detailFlag == true) {
							HashSet<ContextScore> candScoreSet = GetCandidateScoreSet(candSet, tarPos, tarSize, inTextList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, detailFlag);
							Console.WriteLine("------ Suggestion List ------");
							var list = candScoreSet.OrderBy(x => x, csc).Take((int) limitNo).Select(obj => obj.ToString());
							foreach (var item in list)
								Console.WriteLine(item);
						}
						// print the prompt
						Console.WriteLine("- Please input a text, only a spell error allowed (type \"Ctl-d\" to quit) > ");
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
			int tarPos = 2; // the pos of error, not include space
			int tarSize = 1; // the size of error
			int contextRadius = 2; // the context size of either sides (window/2)
			long limitNo = 10; // limit no  of condidates
			if (args.Length == 5) {
				string option = args[0];
				if (option.Equals("-d") == true) {
					detailFlag = true;
				}
				tarPos = int.Parse(args[1]);
				tarSize = int.Parse(args[2]);
				contextRadius = int.Parse(args[3]);
				limitNo = long.Parse(args[4]);
			} else if (args.Length == 4) {
				string option = args[0];
				if (option.Equals("-d") == true) {
					detailFlag = true;
				}
				tarPos = int.Parse(args[1]);
				tarSize = int.Parse(args[2]);
				contextRadius = int.Parse(args[3]);
			} else if (args.Length == 2) {
				string option = args[0];
				if (option.Equals("-d") == true) {
					detailFlag = true;
				}
				tarSize = int.Parse(args[1]);
			} else if (args.Length == 1) {
				string option = args[0];
				if (option.Equals("-d") == true) {
					detailFlag = true;
				}
			} else if (args.Length > 0) {
				Console.WriteLine("Usage: java RankByContext <-d> <tarPos> <tarSize> <contextRadius> <limitNo>");
				Environment.Exit(0);
			}
			// test
			int returnValue = RunTest(detailFlag, tarPos, tarSize, contextRadius, limitNo);
			Environment.Exit(returnValue);
		}
		// data member
	}

}
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
using SpellChecker.Engines.cSpell.Extensions;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///***************************************************************************
	/// This class ranks and finds the best ranked candidates for non-word merge
	/// by ContextSocre.
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
	public class RankNonWordMergeByContext {
		// private constructor
		private RankNonWordMergeByContext() { }
		// return the best ranked str from candidates using word2Vec score
		// inTokenList, includes space token, is not coreTerm.Lc
		// return the orignal inStr if no candidate has score > 0.0d
		public static MergeObj GetTopRankMergeObj(HashSet<MergeObj> candidates, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, bool debugFlag) {
			MergeObj topRankMergeObj = null;
			if (candidates.Count > 0) {
				// 1. find sorted score list for each candidates ...
				List<ContextScore> candScoreList = GetCandidateScoreList(candidates, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, debugFlag);
				// 2. find the hgihest str
				// the 0 element has the highest score because it is sorted        
				string topRankStr = null;
				// only 1 candidate, use it for nonWord
				if (candScoreList.Count == 1) {
					topRankStr = candScoreList[0].GetTerm();
				} else if (candScoreList.Count > 0) { // multiple candidates
					// 1. Check the score, the top rank must be > 0.0
					// This shold use the corrdinated comparator, which +, 0, -
					//if(candScoreList.get(0).GetScore() > 0.0d)
					// 2. Use score system 2
					// Check the score, no updated if the top score is 0.0
					// It works for top score is + or -
					// if the top is 0.0, no updated because top can is not in w2v
					// if top score is 0, we don't know is it better than -
					// top rank rules: score can't be 0.0d
					if (candScoreList[0].GetScore() != 0.0d) {
						topRankStr = candScoreList[0].GetTerm();
					}
				}
				// 3. find the mergeObj from the topRankStr
				if (!string.ReferenceEquals(topRankStr, null)) {
					// 3.1. convert mergeObj set to string set
					// key: coreMergeWord, MergeObj
					Dictionary<string, MergeObj> candStrMergeObjMap = new Dictionary<string, MergeObj>();
					foreach (MergeObj mergeObj in candidates) {
						string mergeWord = mergeObj.GetCoreMergeWord();
						candStrMergeObjMap[mergeWord] = mergeObj;
					}
					HashSet<string> candStrSet = new HashSet<string>(candStrMergeObjMap.Keys);
					// 3.2 convert back from top rank str to MergeObj
					// topRankStr should never be null because candidates is > 0
					topRankMergeObj = candStrMergeObjMap.GetValueOrNull(topRankStr);
				}
			}
			return topRankMergeObj;
		}
		// return candidate scoreObj list sorted by score, higher first
		public static List<ContextScore> GetCandidateScoreList(HashSet<MergeObj> candidates, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, bool debugFlag) {
			// find score object set for each candidates ...
			HashSet<ContextScore> candScoreSet = GetCandidateScoreSet(candidates, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, debugFlag);
			// sorted by the score, higher go first
			List<ContextScore> candScoreList = new List<ContextScore>(candScoreSet);
			ContextScoreComparator<ContextScore> csc = new ContextScoreComparator<ContextScore>();
			candScoreList.Sort(csc);
			// print detail
			foreach (ContextScore contextScore in candScoreList) {
				DebugPrint.PrintCScore(contextScore.ToString(), debugFlag);
			}
			return candScoreList;
		}
		// return candidate set with context score
		// word2Vec is the word|wordVec map to get the wordVec 
		// Not sorted, because it is a set
		// tarPos: starting position of target token
		// tarSize: token size of target token (single word = 1, merge > 1)
		public static HashSet<ContextScore> GetCandidateScoreSet(HashSet<MergeObj> candidates, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, bool debugFlag) {
			HashSet<ContextScore> candScoreSet = new HashSet<ContextScore>();
			// get context score for all candidates
			// go through all merge candidates, all have differetn context  
			foreach (MergeObj mergeObj in candidates) {
				// 1. get the context and contextVec, using input matrix
				int tarPos = mergeObj.GetStartPos();
				int tarSize = mergeObj.GetEndPos() - mergeObj.GetStartPos() + 1;
				DoubleVec contextVec = Word2VecContext.GetContextVec(tarPos, tarSize, nonSpaceTokenList, word2VecIm, contextRadius, word2VecSkipWord, debugFlag);
				// 2. get ContextSocre for each merge, use output matrix
				string mergeWord = mergeObj.GetCoreMergeWord();
				ContextScore cs = new ContextScore(mergeWord, contextVec, word2VecOm);
				candScoreSet.Add(cs);
			}
			return candScoreSet;
		}
		// return the best ranked str from candidates using context score
		// this method is replaced by GetTopRankStr, which sorted by comparator
		public static MergeObj GetTopRankMergeObjByScore(HashSet<MergeObj> candidates, List<TokenObj> nonSpaceTokenList, Word2Vec word2VecIm, Word2Vec word2VecOm, bool word2VecSkipWord, int contextRadius, bool debugFlag) {
			MergeObj topRankMergeObj = null;
			double maxScore = 0.0d;
			foreach (MergeObj mergeObj in candidates) {
				// 1. get the context and contextVec
				int tarPos = mergeObj.GetStartPos();
				int tarSize = mergeObj.GetEndPos() - mergeObj.GetStartPos() + 1;
				DoubleVec contextVec = Word2VecContext.GetContextVec(tarPos, tarSize, nonSpaceTokenList, word2VecIm, contextRadius, word2VecSkipWord, debugFlag);
				// 2. get ContextSocre for each merge, use output matrix
				string mergeWord = mergeObj.GetCoreMergeWord();
				ContextScore cs = new ContextScore(mergeWord, contextVec, word2VecOm);
				double score = cs.GetScore();
				// update only if the score is > 0.0d
				if (score > maxScore) {
					topRankMergeObj = mergeObj;
					maxScore = score;
				}
			}
			return topRankMergeObj;
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
						// remove space token from the list
						List<TokenObj> nonSpaceTokenList = TextObj.GetNonSpaceTokenObjList(inTextList);
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
						HashSet<MergeObj> candSet = NonWordMergeCandidates.GetCandidates(tarPos, nonSpaceTokenList, cSpellApi);
						Console.WriteLine("-- canSet.size(): " + candSet.Count);
						// get final suggestion
						MergeObj topRankMergeObj = GetTopRankMergeObj(candSet, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, detailFlag);
						Console.WriteLine("- top rank merge Obj: " + topRankMergeObj.ToString());
						// print details
						if (detailFlag == true) {
							HashSet<ContextScore> candScoreSet = GetCandidateScoreSet(candSet, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, detailFlag);
							Console.WriteLine("------ Suggestion List ------");
							var list = candScoreSet.OrderBy(x => x, csc).Take((int) limitNo).Select(obj => obj.ToString()).ToList();
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
			int tarSize = 1; // the size of total token of merge words
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
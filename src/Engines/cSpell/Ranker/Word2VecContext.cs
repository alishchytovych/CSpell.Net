using System;
using System.Collections.Generic;
using System.Linq;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///***************************************************************************
	/// This class is the context utility for word2Vec.
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
	public class Word2VecContext {
		// private constructor
		public Word2VecContext() {
			// calculate score
		}
		// public method
		// specify the window radius
		// tarPos: the index of target word in the non-space tokenlist
		// tarSize:
		public static DoubleVec GetContextVec(int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec w2vIm, int radius, bool word2VecSkipWord, bool debugFlag) {
			// 1. get the context
			List<string> contextList = GetContext(tarPos, tarSize, nonSpaceTokenList, w2vIm, radius, word2VecSkipWord, debugFlag);
			// 2. get the wordVec for the context    
			DoubleVec contextVec = Word2VecScore.GetAvgWordVecForList(contextList, w2vIm);
			return contextVec;
		}
		// context from all inTextList, no specify on window radius
		public static DoubleVec GetContextVec(int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec w2vIm, bool word2VecSkipWord, bool debugFlag) {
			// 1. get the context
			List<string> contextList = GetContext(tarPos, tarSize, nonSpaceTokenList, w2vIm, word2VecSkipWord, debugFlag);
			// 2. get the wordVec for the context    
			DoubleVec contextVec = Word2VecScore.GetAvgWordVecForList(contextList, w2vIm);
			return contextVec;
		}
		// specify the radius
		public static List<string> GetContext(int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec w2vIm, bool word2VecSkipWord, bool debugFlag) {
			int radius = 0; // raidus is not needed when Context = true
			bool allContext = true;
			return GetContext(tarPos, tarSize, nonSpaceTokenList, w2vIm, radius, word2VecSkipWord, debugFlag, allContext);
		}
		public static List<string> GetContext(int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec w2vIm, int radius, bool word2VecSkipWord, bool debugFlag) {
			bool allContext = false;
			return GetContext(tarPos, tarSize, nonSpaceTokenList, w2vIm, radius, word2VecSkipWord, debugFlag, allContext);
		}
		private static List<string> GetContext(int tarPos, int tarSize, List<TokenObj> nonSpaceTokenList, Word2Vec w2vIm, int radius, bool word2VecSkipWord, bool debugFlag, bool allContext) {
			// normal TokenObj to string, use coreTerm.lc
			List<string> normTextList = new List<string>();
			if (nonSpaceTokenList != null)
				foreach (TokenObj tokenObj in nonSpaceTokenList) {
					// norm the token, such as [NUM], [URL], [EMAIL]
					// TBD, should be done in pre-correction, preProcess
					string normWord = NormWordForWord2Vec(tokenObj.GetTokenStr());
					normTextList.Add(normWord);
				}
			// get the context list by normStr (becasue normStr is key in w2v)
			List<string> contextList = GetContextForTar(tarPos, tarSize, normTextList, w2vIm, radius, word2VecSkipWord, allContext);
			DebugPrint.PrintContext(contextList, debugFlag);
			return contextList;
		}
		public static string NormWordForWord2Vec(string inWord) {
			// 1. CoreTerm
			int ctType = CoreTermUtil.CT_TYPE_SPACE_PUNC;
			bool lcFlag = true;
			string inWordCtLc = CoreTermUtil.GetCoreTerm(inWord, ctType, lcFlag);
			// 2. find patterns of [NUM], [URL], [EMAIL]
			string inWordPat = inWordCtLc;
			if (InternetTokenUtil.IsUrl(inWordCtLc) == true) {
				inWordPat = PAT_URL;
			} else if (InternetTokenUtil.IsEmail(inWordCtLc) == true) {
				inWordPat = PAT_EMAIL;
			} else if (DigitPuncTokenUtil.IsPunc(inWordCtLc) == true) {
				inWordPat = ""; // remove puctuation
			} else if (DigitPuncTokenUtil.IsDigitPunc(inWordCtLc) == true) {
				inWordPat = PAT_NUM; // add puctuation test to remove
			}
			// Add test set special case
			// TBD: convert the format [CONTACT] to [EMAIL]
			// TBD: not to implemented, because it is better to
			// sync the format in PreProcess: [CONTACT], [NUM], ...
			// TBD: make sure the coreTerm does not take out above pattern
			/*
			else if(inWord.equals("[CONTACT]") == true)
			{
			    inWordPat = PAT_EMAIL;    // could be Telephone number [PAT_NUM]
			}
			*/
			// 3. TBD: take care of xxx's
			return inWordPat;
		}

		// Get context:
		// tarPos: target word position
		// tarSize: no. of tokens for target word (merge should be > 1)
		// inTextList: No empty space token
		// w2vIm: context must use word2Vec input matrix
		// radius: number of tokens before / after the tarPos
		// boolean word2VecSkipWord: skip word if the word does not have wordVec
		private static List<string> GetContextForTar(int tarPos, int tarSize, List<string> nonSpaceTokenList, Word2Vec w2vIm, int radius, bool word2VecSkipWord, bool allContext) {
			// output context
			List<string> outContextList = new List<string>();
			// 2. find context before the tar token
			int tokenNo = 0;
			for (int i = tarPos - 1; i >= 0; i--) {
				string inWord = nonSpaceTokenList[i];
				// check if has wordVec if word2VecSkipWord = true
				if ((word2VecSkipWord == false) || (w2vIm.HasWordVec(inWord) == true)) {
					tokenNo++;
					if ((tokenNo <= radius) || (allContext == true)) {
						outContextList.Insert(0, inWord);
					} else {
						break;
					}
				}
			}
			// 3. find context after the tar token
			int endPos = tarPos + tarSize; // target could be multiwords
			tokenNo = 0;
			for (int i = endPos; i < nonSpaceTokenList.Count; i++) {
				string inWord = nonSpaceTokenList[i];
				if ((word2VecSkipWord == false) || (w2vIm.HasWordVec(inWord) == true)) {
					tokenNo++;
					if ((tokenNo <= radius) || (allContext == true)) {
						outContextList.Add(inWord);
					} else {
						break;
					}
				}
			}

			return outContextList;
		}
		// private method
		private static void Test(Word2Vec w2vIm, Word2Vec w2vOm) { }
		private static void Tests(Word2Vec w2vIm, Word2Vec w2vOm) {
			string inText = "... last 10 years #$% was dianosed test123 yahoo.com early on set deminita 3 year ago.";
			List<TokenObj> inTextList = TextObj.TextToTokenList(inText);
			Console.WriteLine("======= Word2VecContext ======================");
			Console.WriteLine(" - inText: [" + inText + "]");
			string inStr = String.Join("|", inTextList.Select(obj => obj.GetTokenStr()));
			Console.WriteLine(" - inTextList (" + inTextList.Count + "): [" + inStr + "]");

			int tarPos = 0;
			int tarSize = 1;
			int index = 0;
			int radius = 3;
			bool debugFlag = false;
			Console.WriteLine("------ Test GetContext (no skip), radius=3 ...");
			// remove space token from the list
			List<TokenObj> nonSpaceTokenList = TextObj.GetNonSpaceTokenObjList(inTextList);
			foreach (TokenObj tokenObj in inTextList) {
				// not the space token
				if (tokenObj.IsSpaceToken() == false) {
					string tokenStr = tokenObj.GetTokenStr();
					// word2VecSkipWord = false (no skip)
					List<string> contextList = GetContext(tarPos, tarSize, nonSpaceTokenList, w2vIm, radius, false, debugFlag);
					string contextStr = String.Join("|", contextList);
					Console.WriteLine(tarPos + "|" + index + "|" + tokenStr + ": [" + contextStr + "]");
					tarPos++;
				}
				index++;
			}
			Console.WriteLine("------ Test GetContext (skip) , radius=3 ...");
			Console.WriteLine(" - inText: [" + inText + "]");
			tarPos = 0;
			foreach (TokenObj tokenObj in inTextList) {
				// not the space token
				if (tokenObj.IsSpaceToken() == false) {
					string tokenStr = tokenObj.GetTokenStr();
					// word2VecSkipWord = true (skip)
					List<string> contextList2 = GetContext(tarPos, tarSize, nonSpaceTokenList, w2vIm, radius, true, debugFlag);
					string contextStr2 = String.Join("|", contextList2);
					Console.WriteLine(tarPos + "|" + index + "|" + tokenStr + ": [" + contextStr2 + "]");
					tarPos++;
				}
				index++;
			}
			Console.WriteLine("------ Test GetContext (skip) , all ...");
			Console.WriteLine(" - inText: [" + inText + "]");
			tarPos = 0;
			// not the space token
			foreach (TokenObj tokenObj in nonSpaceTokenList) {
				string tokenStr = tokenObj.GetTokenStr();
				// word2VecSkipWord = true (skip)
				List<string> contextList3 = GetContext(tarPos, tarSize, nonSpaceTokenList, w2vIm, true, debugFlag);
				string contextStr3 = String.Join("|", contextList3);
				Console.WriteLine(tarPos + "|" + tokenStr + ": [" + contextStr3 + "]");
				tarPos++;
			}
		}
		// test Driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java Word2VecContext");
				Environment.Exit(0);
			}
			// test
			string inImFile = "../data/Context/syn0.data";
			string inOmFile = "../data/Context/syn1n.data";
			bool verboseFlag = true;
			Word2Vec w2vIm = new Word2Vec(inImFile, verboseFlag);
			Word2Vec w2vOm = new Word2Vec(inOmFile, verboseFlag);
			Tests(w2vIm, w2vOm);
		}
		// data member
		public const string PAT_URL = "[URL]";
		public const string PAT_EMAIL = "[EMAIL]";
		public const string PAT_NUM = "[NUM]";
	}

}
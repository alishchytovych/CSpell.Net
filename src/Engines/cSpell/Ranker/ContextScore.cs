using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Ranker {
	/// <summary>
	///***************************************************************************
	/// This class provides a java object of context score.
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
	public class ContextScore {
		// public constructor
		/// <summary>
		/// Public constructor for ContextScore
		/// </summary>
		/// <param name="inTerm"> target token or candidate (can be multiword) </param>
		/// <param name="contextVec"> wordVec of context from IM </param>
		/// <param name="word2Vec"> word2Vec matrix of OM </param>
		public ContextScore(string inTerm, DoubleVec contextVec, Word2Vec word2Vec) {
			term_ = inTerm;
			// Use Cosine Similarity between IM and OM
			score_ = Word2VecScore.GetScore(inTerm, contextVec, word2Vec);
			// TBD: use 2-3 gram 
			//score_ = NgramScore.GetScore(word, ngram);
		}
		// public method
		public virtual string GetTerm() {
			return term_;
		}
		public virtual double GetScore() {
			return score_;
		}
		public virtual string ToString() {
			string outStr = ToString(GlobalVars.FS_STR);
			return outStr;
		}
		public virtual string ToString(string fieldSepStr) {
			string outStr = term_ + fieldSepStr + string.Format("{0,1:F8}", score_); //match up to the PRECISION
			return outStr;
		}
		// private method
		// Test merge and Split
		private static void Test(string inText, int tarPos, int tarSize, int radius, string mergedWord, string splitWords, Word2Vec w2vIm, Word2Vec w2vOm) {
			// 0. process the inText
			TextObj textObj = new TextObj(inText);
			List<TokenObj> inTextList = textObj.GetTokenList();
			// remove space token from the list
			List<TokenObj> nonSpaceTokenList = TextObj.GetNonSpaceTokenObjList(inTextList);
			Console.WriteLine("==========================================");
			Console.WriteLine("-- inTextList: [" + inText + "]");
			bool word2VecSkipWord = true;
			bool debugFlag = false;
			// 1.a context with window radius
			DoubleVec contextVec = Word2VecContext.GetContextVec(tarPos, tarSize, nonSpaceTokenList, w2vIm, radius, word2VecSkipWord, debugFlag);
			// 1.b context with all inText
			DoubleVec contextVecA = Word2VecContext.GetContextVec(tarPos, tarSize, nonSpaceTokenList, w2vIm, word2VecSkipWord, debugFlag);
			// 1.c get score1
			ContextScore score1 = new ContextScore(mergedWord, contextVec, w2vOm);
			ContextScore score1a = new ContextScore(mergedWord, contextVecA, w2vOm);
			Console.WriteLine(score1.ToString() + "|" + string.Format("{0,1:F8}", score1a.GetScore()));
			// 2. split words
			ContextScore score2 = new ContextScore(splitWords, contextVec, w2vOm);
			ContextScore score2a = new ContextScore(splitWords, contextVecA, w2vOm);
			Console.WriteLine(score2.ToString() + "|" + string.Format("{0,1:F8}", score2a.GetScore()));
			// 3. 3. 3. Use avg. score on single words
			// This method use different context for each single word
			List<string> splitWordList = TermUtil.ToWordList(splitWords);
			int index = 0;
			double scoreSAvg = 0.0d; // radius
			double scoreSAAvg = 0.0d; // all inText
			//debugFlag = false;
			foreach (string splitWord in splitWordList) {
				// window radius
				DoubleVec contextVecS = Word2VecContext.GetContextVec(tarPos + index, 1, nonSpaceTokenList, w2vIm, radius, word2VecSkipWord, debugFlag);
				ContextScore scoreS = new ContextScore(splitWord, contextVecS, w2vOm);
				//System.out.println("-- " + scoreS.ToString());    
				scoreSAvg += scoreS.GetScore();
				// all text
				DoubleVec contextVecSA = Word2VecContext.GetContextVec(tarPos + index, 1, nonSpaceTokenList, w2vIm, word2VecSkipWord, debugFlag);
				ContextScore scoreSA = new ContextScore(splitWord, contextVecSA, w2vOm);
				//System.out.println("-- " + scoreSA.ToString());    
				scoreSAAvg += scoreSA.GetScore();
				index++;
			}
			scoreSAvg = scoreSAvg / index; // window
			scoreSAAvg = scoreSAAvg / index; // all text
			Console.WriteLine("Avg. Single Word|" + string.Format("{0,1:F8}", scoreSAvg) + "|" + string.Format("{0,1:F8}", scoreSAAvg));
		}
		private static void Tests(Word2Vec w2vIm, Word2Vec w2vOm) {
			// test 1: non-word split
			Console.WriteLine("======== non-word split ==========");
			// 14, knowabout -> know about
			//String inTextNs1 = "Not all doctors know about this syndrome.";
			string inTextNs1 = "Not all doctors know about this syndrome.";
			Test(inTextNs1, 3, 2, 2, "knowabout", "know about", w2vIm, w2vOm);
			// 10225, friendShare -> friend Share 
			string inTextNs2 = "Aftercare Email this page to a friend share on facebook.";
			Test(inTextNs2, 6, 2, 2, "friendshare", "friend share", w2vIm, w2vOm);
			// 1-135085045, noone -> no one
			string inTextNs3 = "We have went to doctors, all over the state of NC and still, no one will help us.";
			Test(inTextNs3, 13, 2, 2, "noone", "no one", w2vIm, w2vOm);
			// 1-120029095, hotflashes -> hotflashes
			string inTextNs4 = "Menopause and hot flashes. non-pharmacological ways to relieve symptoms of menopause.";
			Test(inTextNs4, 2, 2, 2, "hotflashes", "hot flashes", w2vIm, w2vOm);
			// test 2: non-word merge 
			Console.WriteLine("======== non-word merge ==========");
			// 12, tricho rhino phalangeal -> trichorhinophalangeal 
			string inTextNm1 = "Hello, looking at your site it said that it had doctors that treated tricho rhino phalangeal syndrome.";
			Test(inTextNm1, 13, 3, 2, "trichorhinophalangeal", "tricho rhino phalangeal", w2vIm, w2vOm);
			// 73, dur ing -> during
			string inTextNm2 = "I have seven live births with no problems dur ing my pregnancies.";
			Test(inTextNm2, 8, 2, 2, "during", "dur ing", w2vIm, w2vOm);
			// 1-119980475, stiff n ess -> stiffness
			string inTextNm3 = "A lot of the pain is joint stiff n ess and my skin feels like it is being burned.";
			Test(inTextNm3, 7, 3, 2, "stiffness", "stiff n ess", w2vIm, w2vOm);
			// 1-136586815, ver y -> very
			string inTextNm4 = "Thank you ver y much.";
			Test(inTextNm4, 2, 2, 2, "very", "ver y", w2vIm, w2vOm);
			// test 3: real-word split
			Console.WriteLine("======== real-word split ==========");
			// 1034, along -> a long
			string inTextRs1 = "sounding in my ear every time for along time.";
			Test(inTextRs1, 4, 2, 2, "everytime", "every time", w2vIm, w2vOm);
			// 13864, apart -> a part
			string inTextRs2 = "I also wanted to know how can I donate myself to be a part of this study?";
			Test(inTextRs2, 12, 2, 2, "apart", "a part", w2vIm, w2vOm);
			// 1-134591345, anyway -> any way
			string inTextRs3 = "Is there any way of knowing or getting an answer to that question after the event has happened?";
			Test(inTextRs3, 2, 2, 2, "anyway", "any way", w2vIm, w2vOm);
			// 1-136441717, everyday -> every day
			string inTextRs4 = "every day I took the pill, my pain increased!";
			Test(inTextRs4, 0, 2, 2, "everyday", "every day", w2vIm, w2vOm);
			// test 4: real-word merge
			Console.WriteLine("======== real-word merge ==========");
			// 1, on set -> onset
			string inTextRm1 = "My mom is 82 years old suffering from anxiety and depression for the last 10 years was diagnosed early on set dementia 3 years ago.";
			Test(inTextRm1, 19, 2, 2, "onset", "on set", w2vIm, w2vOm);
			// 24, some thing -> something
			string inTextRm2 = "can you send me a chart or some thing";
			Test(inTextRm2, 7, 2, 2, "something", "some thing", w2vIm, w2vOm);
			// 1-123152135, ultrasound
			string inTextRm3 = "My sputum report is negative hiv test also negative in ultra sound show some fluid (water) but it shows constant.";
			Test(inTextRm3, 10, 2, 2, "ultrasound", "ultra sound", w2vIm, w2vOm);
			// 1-122785307, up date -> update
			string inTextRm4 = "Need all up date on lupus and send by mail information on this subject.";
			Test(inTextRm4, 2, 2, 2, "update", "up date", w2vIm, w2vOm);
		}
		private static void TestOnSet(Word2Vec w2vIm, Word2Vec w2vOm) {
			string inText = "He was diagnosed early on set dementia 3 years ago.";

			TextObj textObj = new TextObj(inText);
			List<TokenObj> inTextList = textObj.GetTokenList();
			// remove space token from the list
			List<TokenObj> nonSpaceTokenList = TextObj.GetNonSpaceTokenObjList(inTextList);
			Console.WriteLine("==========================================");
			Console.WriteLine("-- inTextList: [" + inText + "]");
			int tarPos = 4;
			int tarSize = 2; // "on set" has 2 tokens
			int radius = 2;
			bool word2VecSkipWord = true;
			bool debugFlag = false;
			// 1 context with window radius
			DoubleVec contextVec = Word2VecContext.GetContextVec(tarPos, tarSize, nonSpaceTokenList, w2vIm, radius, word2VecSkipWord, debugFlag);
			string str1 = "onset";
			ContextScore s1 = new ContextScore(str1, contextVec, w2vOm);
			string str2 = "on set";
			ContextScore s2 = new ContextScore(str2, contextVec, w2vOm);
			Console.WriteLine("- [" + str1 + "]: " + s1.ToString());
			Console.WriteLine("- [" + str2 + "]: " + s2.ToString());
		}
		// test Driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java ContextScore");
				Environment.Exit(0);
			}
			// TBD: test
			// test real word and non word on split and merge case
			//
			// nonword split: knowabout -> know about, hotflahes -> hot flashes 
			// alot -> a lot
			// nonword merge: stiff n ess -> stiffness
			// 
			// realword: whatever vs. what ever 
			// realword: onset vs. on set
			/// <summary>
			/// String configFile = "../data/Config/cSpell.properties";
			/// CSpellApi cSpellApi = new CSpellApi(configFile);
			/// Word2Vec w2vIm = cSpellApi.GetWord2VecIm();
			/// Word2Vec w2vOm = cSpellApi.GetWord2VecOm();
			/// 
			/// </summary>
			string inImFile = "../data/Context/syn0.data";
			string inOmFile = "../data/Context/syn1n.data";
			bool verboseFlag = true;
			Word2Vec w2vIm = new Word2Vec(inImFile, verboseFlag);
			Word2Vec w2vOm = new Word2Vec(inOmFile, verboseFlag);
			//Tests(w2vIm, w2vOm);
			TestOnSet(w2vIm, w2vOm);
		}
		// data member
		// This is language model, each string has a context similarity score
		// currently, only use word2Vec similarity
		// TBD, use bi-gram & tri-grams
		private string term_ = "";
		private double score_ = 0.0d; // context score
	}

}
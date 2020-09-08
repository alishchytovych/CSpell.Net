using System;
using System.Collections.Generic;
using System.Linq;
using SpellChecker.Engines.cSpell.Extensions;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;

namespace SpellChecker.Engines.cSpell.Ranker {
	/// <summary>
	///***************************************************************************
	/// This class is the java object of frequency score by word count.
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
	public class WordCountScore {
		// private constructor
		public WordCountScore() {
			// calculate score
		}
		// public method
		public static double GetScore(string inWord, WordWcMap wordWcMap) {
			//double score = GetScoreByChurch(inWord, wordWcMap);
			//double score = GetScoreByCrowell(inWord, wordWcMap);
			//double score = GetScoreByPeter(inWord, wordWcMap);
			//double score = GetWc(inWord, wordWcMap);
			// Halil 
			//double score = GetUnigramFreqScore(inWord, wordWcMap);
			//double score = GetScoreDev1(inWord, wordWcMap);

			// default - multiword
			double score = GetScoreDev2(inWord, wordWcMap);
			// Baseline Orginal code
			//double score = GetCorpusFreqScore(inWord, wordWcMap);

			// Not used: double score = GetAdjustScoreMin(inWord, wordWcMap);
			// Not used: double score = GetAdjustScoreAvg(inWord, wordWcMap);
			return score;
		}
		// single word score, multiwords is 0.0
		private static double GetScoreDev1(string inWord, WordWcMap wordWcMap) {
			long maxWc = wordWcMap.GetMaxWc();
			double wc = GetWc(inWord, wordWcMap);
			double score = wc / (1.0 * maxWc);
			return score;
		}
		// include multiwords, multiwords = avg. score
		private static double GetScoreDev2(string inWord, WordWcMap wordWcMap) {
			// check multiword case for split
			bool normFlag = false; // don't use punctuation for determiner
			List<string> wordList = TermUtil.ToWordList(inWord, normFlag);
			double score = 0.0;
			double totalScore = 0.0;
			int totalWords = wordList.Count;
			//double maxWc = GetAdjustedWc(wordWcMap.GetMaxWc());
			// use the average score for the multiwords
			foreach (string word in wordList) {
				//double curScore = GetScoreByChurch(word, wordWcMap);
				//double curScore = GetScoreByCrowell(word, wordWcMap);
				//double curScore = GetScoreByPeter(word, wordWcMap);
				//double curScore = GetUnigramFreqScore(word, wordWcMap);
				//double curScore = GetWc(word, wordWcMap);
				double curScore = GetScoreDev1(word, wordWcMap);
				totalScore += curScore;
			}
			if (totalScore > 0.0) {
				score = totalScore / totalWords;
			}
			return score;
		}
		// 1990, 1191 paper from Church
		private static double GetScoreByCrowell(string inWord, WordWcMap wordWcMap) {
			double wc = GetWc(inWord, wordWcMap);
			double score = 0.5; // assign to 0.5 if word is not in the corpus
			if (wc != 0.0) {
				score = (1.0 + Math.Log(wc));
			}
			return score;
		}
		private static double GetScoreByChurch(string inWord, WordWcMap wordWcMap) {
			long totalWc = wordWcMap.GetTotalWc();
			double wc = GetWc(inWord, wordWcMap);
			double score = (1.0 + wc) / (1.0 * totalWc);
			return score;
		}
		private static double GetScoreByPeter(string inWord, WordWcMap wordWcMap) {
			long totalWc = wordWcMap.GetTotalWc();
			double wc = GetWc(inWord, wordWcMap);
			double score = wc / (1.0 * totalWc);
			return score;
		}
		public static double GetAdjustScoreMin(string inWord, WordWcMap wordWcMap) {
			// check multiword case for split
			bool normFlag = false; // don't use punctuation for determiner
			List<string> wordList = TermUtil.ToWordList(inWord, normFlag);
			double score = 0.0;
			double totalScore = 0.0;
			int totalWords = wordList.Count;
			double maxWc = GetAdjustedWc(wordWcMap.GetMaxWc());
			// use the average score for the multiwords
			double minScore = int.MaxValue;
			foreach (string word in wordList) {
				double curScore = GetWordScore(word, maxWc, wordWcMap);
				minScore = (curScore < minScore ? curScore : minScore);
			}
			if (minScore < int.MaxValue) {
				score = minScore;
			}
			return score;
		}
		// get socre for single word and multiwords (for split cases)
		// 1). multiword: score = avg. score of allwords
		// 2). single word: score =  log(adjust WC) / log (adjust Max. WC).
		public static double GetAdjustScoreAvg(string inWord, WordWcMap wordWcMap) {
			// check multiword case for split
			bool normFlag = false; // don't use punctuation for determiner
			List<string> wordList = TermUtil.ToWordList(inWord, normFlag);
			double score = 0.0;
			double totalScore = 0.0;
			long totalWords = wordList.Count;
			double maxWc = GetAdjustedWc(wordWcMap.GetMaxWc());
			// use the average score for the multiwords
			foreach (string word in wordList) {
				totalScore += GetWordScore(word, maxWc, wordWcMap);
			}
			if (totalWords > 0) {
				score = totalScore / totalWords;
			}
			return score;
		}
		// score = WC(word)/total_word_cout
		// score range is between 0.0 ~ 1.0
		// should be the same as GetCorpusFreqScore
		// Get score for a singel word
		private static double GetWordScore(string inWord, double maxWc, WordWcMap wordWcMap) {
			double wc = GetAdjustedWc(inWord, wordWcMap);
			double score = (Math.Log(wc) / Math.Log(maxWc));
			return score;
		}
		// should be the same as GetUnigramFreqScore
		// score range is between 0.0 ~ 1.0
		// not used because it is no good
		private static double GetWordScore2(string inWord, double maxWc, WordWcMap wordWcMap) {
			double wc = 1.0d * GetWc(inWord, wordWcMap);
			double totalWc = 1.0d * wordWcMap.GetTotalWc();
			double score = (Math.Log(wc / totalWc) / Math.Log(maxWc / totalWc));
			return score;
		}
		// add Adjust WC = CONS + COEF*WC 
		// This is used to calcualted the score
		// Set 1.0 so log will not return Nan for words not exist 
		// Set 10.0 * WC so that word in the corpus has higher rank 
		public static double GetAdjustedWcOld(long wc) {
			double ajustWc = CONS + COEF * Math.Log(wc);
			return ajustWc;
		}
		public static double GetAdjustedWc(long wc) {
			double ajustWc = CONS / 2.0;
			if (wc != 0.0) {
				ajustWc = CONS + COEF * (Math.Log(wc) / Math.Log(10));
			}
			return ajustWc;
		}
		public static double GetAdjustedWc(string inWord, WordWcMap wordWcMap) {
			int wc = GetWc(inWord, wordWcMap);
			return GetAdjustedWc(wc);
		}
		public static double GetWordPossOverTotalWc(string inWord, WordWcMap wordWcMap) {
			bool caseFlag = false;
			return GetWordPossOverTotalWc(inWord, wordWcMap, caseFlag);
		}
		public static double GetWordPossOverMaxWc(string inWord, WordWcMap wordWcMap) {
			bool caseFlag = false;
			return GetWordPossOverMaxWc(inWord, wordWcMap, caseFlag);
		}
		// org code from baseline, TBM
		public static double GetCorpusFreqScore(string inWord, WordWcMap wordWcMap) {
			// get the wc
			Dictionary<string, int> wWcMap = wordWcMap.GetWordWcMap();
			int freq = (wWcMap.ContainsKey(inWord) ? wWcMap.GetValueOrNull(inWord) : 0);
			// check if inWord is a multiword
			IList<string> spls = inWord.Split("[ ]").ToList();
			bool isSplit = spls.Count >= 2;
			if (isSplit == false) {
				// check possessive, this is not right:
				// all XXX's will result in same scsore is XXX is bigger than 's
				if (inWord.EndsWith("'s", StringComparison.Ordinal)) {
					spls = new List<string>();
					spls.Add(inWord.Substring(0, inWord.Length - 2));
					spls.Add("'s");
					isSplit = true;
				}
			} else {
				//System.out.println("---- split: [" + inWord + "]"); 
			}
			// use the min. wc of split word in the multiword's case
			if (freq == 0 && isSplit) {
				int min = int.MaxValue;
				foreach (string spl in spls) {
					//System.out.println("- split: rpStr: [" + spl + "|" + rpStr + "]");
					if (String.IsNullOrEmpty(spl)) {
						continue;
					}
					int splFreq = (wWcMap.ContainsKey(spl) ? wWcMap.GetValueOrNull(spl) : 0);
					//System.out.println("Corpus count:" + spl + "|" + wWcMap.get(spl) + "|" + splFreq);
					// use the min. freq of each word as the freq of the multiwords
					if (splFreq >= 0 && splFreq < min) {
						min = splFreq;
					}
				}
				// use the min. freq of the split words as whole word?
				freq = min;
			}
			if (freq == 0) {
				return 0.0;
			}
			long maxWc = wordWcMap.GetMaxWc();
			double score = (Math.Log(freq) / Math.Log(maxWc));
			return score;
		}
		// org code from baseline, TBM, From Ensemble
		public static double GetUnigramFreqScore(string inWord, WordWcMap wordWcMap) {
			Dictionary<string, int> wWcMap = wordWcMap.GetWordWcMap();
			int freq = (wWcMap.ContainsKey(inWord) ? wWcMap.GetValueOrNull(inWord) : 0);
			IList<string> spls = inWord.Split("[ ]", true).ToList();
			bool isSplit = spls.Count >= 2;
			if (isSplit == false) {
				if (inWord.EndsWith("'s", StringComparison.Ordinal)) {
					spls = new List<string>();
					spls.Add(inWord.Substring(0, inWord.Length - 2));
					spls.Add("'s");
					isSplit = true;
				}
			}
			// use the min. wc of split word in the multiword's case
			if (freq == 0 && isSplit) {
				int min = int.MaxValue;
				foreach (string spl in spls) {
					if (String.IsNullOrEmpty(spl)) {
						continue;
					}
					int splFreq = (wWcMap.ContainsKey(spl) ? wWcMap.GetValueOrNull(spl) : 0);
					//System.out.println("Corpus count:" + spl + "|" + wWcMap.get(spl) + "|" + splFreq);
					if (splFreq >= 0 && splFreq < min) {
						min = splFreq;
					}
				}
				freq = min;
			}
			if (freq == 0) {
				return 0.0; // to avoid infinity
			}
			long maxWc = wordWcMap.GetMaxWc();
			long totalWc = wordWcMap.GetTotalWc();
			double score = (Math.Log(1.0 * freq / totalWc) / Math.Log(1.0 * maxWc / totalWc));
			return score;
		}
		// private method
		// possibility = WC(w0rd)/max_word_cout
		private static double GetWordPossOverMaxWc(string inWord, WordWcMap wordWcMap, bool caseFlag) {
			double wc = 1.0d * GetWc(inWord, wordWcMap, caseFlag);
			double maxWc = 1.0d * wordWcMap.GetMaxWc();
			double score = wc / maxWc;
			return score;
		}
		// possibility = WC(w0rd)/total_word_cout
		private static double GetWordPossOverTotalWc(string inWord, WordWcMap wordWcMap, bool caseFlag) {
			double wc = 1.0d * GetWc(inWord, wordWcMap, caseFlag);
			double totalWc = 1.0d * wordWcMap.GetTotalWc();
			double score = wc / totalWc;
			return score;
		}
		// convert inWord to lowercase, key in word Frequency are all lowercase
		public static int GetWc(string inWord, WordWcMap wordWcMap) {
			bool caseFlag = false; // lowercase, case insensitive
			return GetWc(inWord, wordWcMap, caseFlag);
		}
		private static int GetWc(string inWord, WordWcMap wordWcMap, bool caseFlag) {
			string inWordLc = inWord;
			// ignore case
			if (caseFlag == false) {
				inWordLc = inWord.ToLower();
			}
			// the key of wWcMap are lowercased in the Beta version
			Dictionary<string, int> wWcMap = wordWcMap.GetWordWcMap();
			int wc = 0;
			if (wWcMap.GetValueOrNull(inWordLc) != null) {
				wc = wWcMap.GetValueOrNull(inWordLc);
			}
			return wc;
		}
		private static void Test(string inWord, WordWcMap wordWcMap) {
			Console.WriteLine(inWord + "|" + string.Format("{0,1:F4}", GetScore(inWord, wordWcMap)) + "|" + string.Format("{0,1:F4}", GetAdjustedWc(inWord, wordWcMap)) + "|" + GetWc(inWord, wordWcMap) + "|" + Math.Log(GetWc(inWord, wordWcMap)) + "|" + (Math.Log(GetWc(inWord, wordWcMap) / Math.Log(10))) + "|" + string.Format("{0,1:F4}", GetWordPossOverMaxWc(inWord, wordWcMap)));
		}
		private static void Tests(WordWcMap wordWcMap) {
			List<string> testStrList = new List<string>();
			testStrList.Add("the"); // first one in the corpus
			testStrList.Add("&eacute;vy"); // last one in the corpus
			testStrList.Add("xxxx"); // not in the corpus
			testStrList.Add("spondylitis"); // first one in the corpus
			testStrList.Add("spondyl"); // first one in the corpus
			testStrList.Add("its"); // first one in the corpus
			testStrList.Add("if"); // first one in the corpus
			testStrList.Add("you"); // first one in the corpus
			testStrList.Add("doctor");
			testStrList.Add("Doctor"); // Test Case
			testStrList.Add("doctor[123]");
			testStrList.Add("'s");
			testStrList.Add("container");
			testStrList.Add("diagnose");
			testStrList.Add("deionized");
			testStrList.Add("diabetic");
			testStrList.Add("diabetics");
			testStrList.Add("doctor's"); // posssive
			testStrList.Add("heart's");
			testStrList.Add("if you"); // multiwords
			testStrList.Add("the doctor"); // multiwords
			testStrList.Add("Not exist");
			testStrList.Add("brokenribscantsleepatnight");
			testStrList.Add("broken");
			testStrList.Add("rib");
			testStrList.Add("ribs");
			testStrList.Add("cant");
			testStrList.Add("cants");
			testStrList.Add("scant");
			testStrList.Add("scants");
			testStrList.Add("sleep");
			testStrList.Add("leep");
			testStrList.Add("lee");
			testStrList.Add("pat");
			testStrList.Add("at");
			testStrList.Add("night");
			testStrList.Add("broken ribs cants leep at night");
			testStrList.Add("broken ribs cant sleep at night");
			testStrList.Add("broken rib scants leep at night");
			testStrList.Add("broken rib scants lee pat night");
			testStrList.Add("broken rib scant sleep at night");
			testStrList.Add("friend share");
			testStrList.Add("assistance");
			testStrList.Add("baraclude and");
			testStrList.Add("xifaxan as");
			testStrList.Add("pamphlets");
			testStrList.Add("damage");
			testStrList.Add("withdrawal");
			testStrList.Add("tachycardia");
			testStrList.Add("always");
			testStrList.Add("itching");
			testStrList.Add("philtrum");
			testStrList.Add("achalasia");
			testStrList.Add("swollen");
			testStrList.Add("of course");
			testStrList.Add("antenatal");
			testStrList.Add("microsomia");
			testStrList.Add("migraine");
			testStrList.Add("hemorrhage");
			Console.WriteLine("=================================================");
			Console.WriteLine("Word|Score|Adjust|Wc|Wc/max");
			Console.WriteLine("=================================================");
			foreach (string testStr in testStrList) {
				Test(testStr, wordWcMap);
			}
		}
		// test Driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java WordCountScore");
				Environment.Exit(0);
			}
			// test
			string inFile = "../data/Frequency/wcWord.data";
			bool verboseFlag = true;
			WordWcMap wordWcMap = new WordWcMap(inFile, verboseFlag);
			Tests(wordWcMap);
		}
		// data member
		private const double CONS = 0.0d;
		private const double COEF = 1.0d;
	}

}
using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;

namespace SpellChecker.Engines.cSpell.Ranker {
	/// <summary>
	///***************************************************************************
	/// This class provides a java object of frequency score.
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
	public class FrequencyScore {
		// public constructor
		public FrequencyScore(string word, WordWcMap wordWcMap) {
			word_ = word;
			// calculate score
			score_ = WordCountScore.GetScore(word, wordWcMap);
		}
		// public method
		public virtual string GetWord() {
			return word_;
		}
		public virtual double GetScore() {
			return score_;
		}
		public virtual string ToString() {
			string outStr = ToString(GlobalVars.FS_STR);
			return outStr;
		}
		public virtual string ToString(string fieldSepStr) {
			string outStr = word_ + fieldSepStr + string.Format("{0,1:F8}", score_); //match up to the PRECISION
			return outStr;
		}

		// private method
		private static void Test(string word, WordWcMap wordWcMap) {
			FrequencyScore fs = new FrequencyScore(word, wordWcMap);
			Console.WriteLine(fs.ToString());
		}
		private static void Tests(WordWcMap wordWcMap) {
			List<string> testStrList = new List<string>();
			testStrList.Add("the"); // first one in the corpus
			testStrList.Add("if"); // first one in the corpus
			testStrList.Add("you"); // first one in the corpus
			testStrList.Add("doctor");
			testStrList.Add("Doctor"); // Test Case
			testStrList.Add("doctor[123]");
			testStrList.Add("'s");
			testStrList.Add("container");
			testStrList.Add("diagnose");
			testStrList.Add("deionized");
			testStrList.Add("&eacute;vy"); // last one in the corpus
			testStrList.Add("xxxx"); // last one in the corpus
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
			Console.WriteLine("=================================================");
			Console.WriteLine("Word|Score");
			Console.WriteLine("=================================================");
			foreach (string testStr in testStrList) {
				Test(testStr, wordWcMap);
			}
		}
		// test Driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java FrequencyScore");
				Environment.Exit(0);
			}
			// test
			string inFile = "../data/Frequency/wcWord.data";
			bool verboseFlag = true;
			WordWcMap wordWcMap = new WordWcMap(inFile, verboseFlag);
			Tests(wordWcMap);
		}
		// data member
		// This is language model, each string has a WC (frequency score)
		private string word_ = "";
		private double score_ = 0.0;
	}

}
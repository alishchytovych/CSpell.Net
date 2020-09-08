using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SpellChecker.Engines.cSpell.Lib;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///***************************************************************************
	/// This class converts and stores word (key) and frequency (value) in Map.
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
	public class WordWcMap {
		// public constructor
		public WordWcMap(string inFile) {
			InitMap(inFile, false);
		}
		public WordWcMap(string inFile, bool verboseFlag) {
			InitMap(inFile, verboseFlag);
		}
		// public method
		public virtual long GetMaxWc() {
			return maxWc_;
		}
		public virtual long GetTotalWc() {
			return totalWc_;
		}
		public virtual long GetTotalWordNo() {
			return totalWordNo_;
		}
		public virtual Dictionary<string, int> GetWordWcMap() {
			return wordWcMap_;
		}
		// private method
		private void InitMap(string inFile, bool verboseFlag) {
			if (verboseFlag == true) {
				Console.WriteLine("- Get WordWcMap from: " + inFile);
			}
			int lineNo = 0;
			string line = null;
			try {
				StreamReader @in = new StreamReader(new FileStream(inFile, FileMode.Open, FileAccess.Read), Encoding.UTF8);
				// read in line by line from a file
				while (!string.ReferenceEquals((line = @in.ReadLine()), null)) {
					lineNo++;
					var buf = line.Split(GlobalVars.FS_STR);
					int wc = Int32.Parse(buf[0]);
					string word = buf[1];
					if ((word.StartsWith("[", StringComparison.Ordinal) && word.EndsWith("]", StringComparison.Ordinal)) == false) {
						wordWcMap_[word] = wc;
						totalWc_ += wc;
						totalWordNo_++;
						maxWc_ = (wc > maxWc_?wc : maxWc_);
					}
				}
				// close
				@in.Close();
				// print out all word set
				if (verboseFlag == true) {
					Console.WriteLine("-- Total line no: " + lineNo);
					Console.WriteLine("-- Total word no: " + totalWordNo_ + " (" + wordWcMap_.Keys.Count + ")");
					Console.WriteLine("-- Total word count: " + totalWc_);
					Console.WriteLine("-- Max. word count: " + maxWc_);
				}
			} catch (Exception e) {
				Console.Error.WriteLine("Line: " + lineNo + " - " + line);
				Console.Error.WriteLine("** ERR@WordWcMap( ), problem of reading file (" + inFile + ") @ line [" + lineNo + "]: " + line);
				Console.Error.WriteLine("Exception: " + e.ToString());
			}
		}
		// test Driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java WordWcMap");
				Environment.Exit(0);
			}
			// test
			string inFile = "../data/Freq/baselineWordFreq.data";
			bool verboseFlag = true;
			WordWcMap wordWcMap = new WordWcMap(inFile, verboseFlag);
		}
		// data member
		private long maxWc_ = 0; // max word count in the map
		private long totalWc_ = 0; // total word count in the Map
		private long totalWordNo_ = 0; // total word no
		private Dictionary<string, int> wordWcMap_ = new Dictionary<string, int>(); // key: word, value:WC
	}

}
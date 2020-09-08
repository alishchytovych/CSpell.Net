using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SpellChecker.Engines.cSpell.Extensions;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Dictionary {
	/// <summary>
	///***************************************************************************
	/// This class is the java object for a full dictionary.
	/// Full dictionary uses 8 field to load words from Lexicon dictionary.
	/// Full dictionary is not used in the 2018 release.
	/// 
	/// <para><b>History:</b>
	/// <ul>
	/// <li>2018 baseline
	/// </ul>
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ****************************************************************************
	/// </para>
	/// </summary>
	public class FullDictionary : RootDictionary {
		// public constructor
		// TBD .. a dictinary with case for abb/acr and proper noun
		/// <summary>
		/// Public constructor to initiate the dictionary.
		/// </summary>
		/// <param name="caseFlag"> flag for case sensitive </param>
		public FullDictionary(bool caseFlag) {
			caseFlag_ = caseFlag;
		}
		/// <summary>
		/// Public constructor to initiate the dictionary.
		/// </summary>
		/// <param name="inFile"> source flat file for the dictionary </param>
		public FullDictionary(string inFile) {
			LoadWords(inFile);
		}
		// public methods
		public virtual void AddWord(string word) {
			// add to dictionary
			FullDicVarObj fullDicVarObj = new FullDicVarObj(word);
			if (dictionary_.ContainsKey(word) == true) {
				dictionary_.GetValueOrNull(word).Add(fullDicVarObj);
			} else {
				HashSet<FullDicVarObj> valueSet = new HashSet<FullDicVarObj>();
				valueSet.Add(fullDicVarObj);
				dictionary_[word] = valueSet;
			}
			// add to file: TBD
		}
		// default the caseFlag is false, not case sensitive
		public virtual bool IsDicWord(string word) {
			bool caseFlag = false;
			return IsDicWord(word, caseFlag);
		}
		public virtual bool IsDicWord(string word, bool caseFlag) {
			string inWord = (caseFlag?word : word.ToLower());
			bool inDicFlag = dictionary_.ContainsKey(inWord);
			return inDicFlag;
		}
		public virtual bool IsValidWord(string word) {
			// TBD ...
			return IsDicWord(word);
		}
		public virtual int GetSize() {
			return dictionary_.Count;
		}
		public virtual HashSet<string> GetDictionarySet() {
			HashSet<string> dicSet = new HashSet<string>(dictionary_.Keys);
			return dicSet;
		}
		public virtual Dictionary<string, HashSet<FullDicVarObj>> GetDictionary() {
			return dictionary_;
		}
		// the whole line is a word to be added to dictionary
		public virtual void AddDictionaries(string inFiles, string rootPath) {
			bool debugFlag = false;
			AddDictionaries(inFiles, rootPath, debugFlag);
		}
		public virtual void AddDictionaries(string inFiles, string rootPath, bool debugFlag) {
			DebugPrint.Println("- Dictionary Files: [" + inFiles + "].", debugFlag);
			string[] inFileStrArray = inFiles.Split(":", true);
			foreach (string inFileStr in inFileStrArray) {
				string inDicFile = rootPath + inFileStr;
				DebugPrint.Println("--- Add Dictionary: [" + inDicFile + "].", debugFlag);
				AddDictionary(inDicFile);
			}
		}

		public virtual void AddDictionaries2(string inFiles, bool debugFlag) {
			DebugPrint.Println("- Dictionary Files: [" + inFiles + "].", debugFlag);
			string[] inFileStrArray = inFiles.Split("|", true);
			foreach (var item in inFileStrArray) {
				DebugPrint.Println("--- Add Dictionary: [" + item + "].", debugFlag);
				AddDictionary(item);
			}
		}


		public virtual void AddDictionaries(string inFilePaths, bool debugFlag) {
			DebugPrint.Println("- Dictionary Files: [" + inFilePaths + "].", debugFlag);
			string[] inFileStrArray = inFilePaths.Split(":", true);
			foreach (string inFileStr in inFileStrArray) {
				DebugPrint.Println("--- Add Dictionary: [" + inFileStr + "].", debugFlag);
				AddDictionary(inFileStr);
			}
		}
		public virtual void AddDictionary(string inFile) {
			LoadWords(inFile);
		}
		public virtual void AddDictionary(string inFile, bool lowerCaseFlag) {
			LoadWords(inFile);
		}
		// private methods
		// TBD ... too many key ...
		private void LoadWords(string inFile) {
			int lineNo = 0;
			try {
				String[] lines = File.ReadAllLines(inFile, Encoding.UTF8);
				// go through all lines
				foreach (var line in lines) {
					if (line.StartsWith("#", StringComparison.Ordinal) == false) {
						var buf = line.Split("|");
						string word = buf[0];
						long pos = long.Parse(buf[1]);
						long infl = long.Parse(buf[2]);
						string src = buf[3];
						bool acrAbb = bool.Parse(buf[4]);
						bool properNoun = bool.Parse(buf[5]);
						string key = word.ToLower();
						FullDicVarObj fullDicVarObj = new FullDicVarObj(word, pos, infl, src, acrAbb, properNoun);
						if (dictionary_.ContainsKey(key) == true) {
							dictionary_.GetValueOrNull(key).Add(fullDicVarObj);
						} else {
							HashSet<FullDicVarObj> valueSet = new HashSet<FullDicVarObj>();
							valueSet.Add(fullDicVarObj);
							dictionary_[key] = valueSet;
						}
						lineNo++;
					}
				}
				Console.WriteLine("- total LineNo: " + lineNo);
			} catch (Exception x1) {
				Console.Error.WriteLine("** Err@FullDictionary.LoadWords( ): " + x1.ToString());
			}
		}
		private static void Test() {
			Console.WriteLine("===== Unit Test of BasicDictionary =====");
			string lexDicStr = "../data/Dictionary/lexiconDic.data";
			RootDictionary dic = new FullDictionary(lexDicStr);
			Console.WriteLine("----------------------");
			Console.WriteLine("- Dic File: " + lexDicStr);
			Console.WriteLine("- Dic size: " + dic.GetSize());
			Console.WriteLine("----------------------");
			// test words
			List<string> wordList = new List<string>();
			wordList.Add("test");
			wordList.Add("Test");
			wordList.Add("TEST");
			wordList.Add("liter");
			wordList.Add("litre");
			wordList.Add("odor");
			wordList.Add("odour");
			wordList.Add("iodise");
			wordList.Add("iodize");
			wordList.Add("beveled");
			wordList.Add("bevelled");
			wordList.Add("hemolyse");
			wordList.Add("hemolyze");
			wordList.Add("test321");
			foreach (string w in wordList) {
				Console.WriteLine("- IsDicWord(" + w + "): " + dic.IsDicWord(w));
			}
			string word = "test321";
			Console.WriteLine("------ Add [" + word + "] to dictionary ------");
			dic.AddWord(word);
			Console.WriteLine("- Dic size: " + dic.GetSize());
			Console.WriteLine("- IsDicWord(" + word + "): " + dic.IsDicWord(word));
			Console.WriteLine("===== End of Unit Test =====");
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java FullDictionary");
				Environment.Exit(0);
			}

			// test case and print out 
			Test();
		}
		// data member
		private const int DIC_SIZE = 1000000; // initial size for dic: 1M
		private Dictionary<string, HashSet<FullDicVarObj>> dictionary_ = new Dictionary<string, HashSet<FullDicVarObj>>(DIC_SIZE);
		private bool caseFlag_ = false; // cas esnesitive flag
	}

}
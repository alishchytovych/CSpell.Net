using System;
using System.Collections.Generic;
using System.Linq;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Extensions;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Dictionary {
	/// <summary>
	///***************************************************************************
	/// This class is the java object for a basic dictionary.
	/// Basic dictionary uses 1 field to load words from file(s).
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
	public class BasicDictionary : RootDictionary {
		// public constructor
		/// <summary>
		/// Public constructor to initiate the dictionary.
		/// </summary>
		public BasicDictionary() {
			// default: case insensitive
			caseFlag_ = false;
		}
		public BasicDictionary(bool caseFlag) {
			caseFlag_ = caseFlag;
		}
		public BasicDictionary(string inFile) {
			LoadWords(inFile);
		}
		public BasicDictionary(string inFile, int fieldNo) {
			LoadWords(inFile, fieldNo);
		}
		// public methods
		public virtual void AddWord(string inWord) {
			// add to dictionary
			string word = (caseFlag_?inWord : inWord.ToLower());
			dictionary_.Add(word);
			// add to file: TBD
		}
		// check if the input word is a valid word expression, 
		// including possessive, slash or, parenthetic plural form 
		public virtual bool IsValidWord(string inWord) {
			string word = (caseFlag_?inWord : inWord.ToLower());
			bool wordFlag = IsDicWord(word);
			// check possessive
			if (wordFlag == false) {
				string orgWord = PossessiveTokenUtil.GetOrgWord(word);
				wordFlag = IsDicWord(orgWord);
			}
			// check or slash: case/test
			if (wordFlag == false) {
				if (word.IndexOf("/", StringComparison.Ordinal) > -1) {
					string[] orWords = word.Split("/", true);
					bool orFlag = true;
					foreach (string orWord in orWords) {
						if (IsValidWord(orWord) == false) {
							orFlag = false;
							break;
						}
					}
					wordFlag = orFlag;
				}
			}

			// check parenthic plural forms (s), (es),(ies)
			if (wordFlag == false) {
				string orgWord = ParentheticPluralTokenUtil.GetOrgWord(word);
				wordFlag = IsDicWord(orgWord);
			}

			// check "-", not sure it is a good idea, so did not implement
			return wordFlag;
		}
		// caseFlag: case sensitive flag, if flase, all uses lowerCase
		// If caseFlag is true, words in dictionary must be have different case.
		// Also, if caseFlag is false, words in dictionary must be lowercased. 
		public virtual bool IsDicWord(string inWord) {
			string word = (caseFlag_?inWord : inWord.ToLower());
			bool inDicFlag = dictionary_.Contains(word);
			return inDicFlag;
		}
		public virtual bool GetCaseFlag() {
			return caseFlag_;
		}
		public virtual int GetSize() {
			return dictionary_.Count;
		}
		// get dictionary information
		public virtual string ToString() {
			string outStr = "-- size: " + GetSize() + GlobalVars.LS_STR;
			outStr += "-- caseFlag: " + GetCaseFlag();
			return outStr;
		}
		public virtual HashSet<string> GetDictionarySet() {
			return dictionary_;
		}
		// the whole line is a word to be added to dictionary
		public virtual void AddDictionary(string inFile) {
			LoadWords(inFile);
		}
		// add multiple dictinoaries from multiple files
		public virtual void AddDictionaries(string inFiles, string rootPath) {
			bool debugFlag = false;
			AddDictionaries(inFiles, rootPath, debugFlag);
		}

		public virtual void AddDictionaries2(string inFiles, bool debugFlag) {
			DebugPrint.Println("- Dictionary Files: [" + inFiles + "].", debugFlag);
			string[] inFileStrArray = inFiles.Split("|");
			foreach (var item in inFileStrArray) {
				DebugPrint.Println("--- Add Dictionary: [" + item + "].", debugFlag);
				AddDictionary(item);
			}
		}
		
		public virtual void AddDictionaries(string inFiles, string rootPath, bool debugFlag) {
			DebugPrint.Println("- Dictionary Files: [" + inFiles + "].", debugFlag);
			// split the dictinoary by :
			string[] inFileStrArray = inFiles.Split(":", true);
			foreach (string inFileStr in inFileStrArray) {
				string inDicFile = rootPath + inFileStr;
				DebugPrint.Println("--- Add Dictionary: [" + inDicFile + "].", debugFlag);
				AddDictionary(inDicFile);
			}
		}
		public virtual void AddDictionaries(string inFilePaths, bool debugFlag) {
			DebugPrint.Println("- Dictionary Files: [" + inFilePaths + "].", debugFlag);
			// split the dictinoary by :
			string[] inFileStrArray = inFilePaths.Split(":", true);
			foreach (string inFileStr in inFileStrArray) {
				DebugPrint.Println("--- Add Dictionary: [" + inFileStr + "].", debugFlag);
				AddDictionary(inFileStr);
			}
		}
		// the specified field is a word to be added to dictionary
		public virtual void AddDictionary(string inFile, int fieldNo) {
			LoadWords(inFile, fieldNo);
		}
		// private methods
		private void LoadWords(string inFile) {
			// basic dictionary, if caseFlag is false => case-insensitive
			// => lowerCase for the input
			bool lowerCaseFlag = !caseFlag_;
			dictionary_.addAll(FileInToSet.GetSetByLine(inFile, lowerCaseFlag).ToHashSet());
		}
		private void LoadWords(string inFile, int fieldNo) {
			// basic dictionary, if caseFlag is false => case-insensitive
			// => lowerCase for the input
			bool lowerCaseFlag = !caseFlag_;
			dictionary_.addAll(FileInToSet.GetSetByField(inFile, fieldNo, lowerCaseFlag).ToHashSet());
		}
		private static void Test() {
			Console.WriteLine("===== Unit Test of BasicDictionary =====");
			bool caseFlag = false;
			RootDictionary dic0 = DictionaryFactory.GetDictionary(DictionaryFactory.DIC_BASIC, caseFlag);
			// dic0 baselin dictionary
			Console.WriteLine("------- Words from Baseline 11 dicitoaries -------");
			string dicStrs = "../data/Dictionary/eng_medical.dic:../data/Dictionary/center.dic:../data/Dictionary/centre.dic:../data/Dictionary/color.dic:../data/Dictionary/colour.dic:../data/Dictionary/ise.dic:../data/Dictionary/ize.dic:../data/Dictionary/labeled.dic:../data/Dictionary/labelled.dic:../data/Dictionary/yse.dic:../data/Dictionary/yze.dic";
			string[] dicStrArray = dicStrs.Split(":", true);
			List<string> dicStrList = dicStrArray.ToList();
			foreach (string dicStr in dicStrList) {
				dic0.AddDictionary(dicStr);
				Console.WriteLine("- Dic0 File: " + dicStr);
				Console.WriteLine("- Dic0 size: " + dic0.GetSize());
			}
			Console.WriteLine("------- Lexicon element words -------");
			string lexDicEwStr = "../data/Dictionary/lexiconDic.data.ewLc";
			RootDictionary dic1 = new BasicDictionary(lexDicEwStr);
			Console.WriteLine("- Dic1 File: " + lexDicEwStr);
			Console.WriteLine("- Dic1 size: " + dic1.GetSize());
			Console.WriteLine("------- Lexicon words --------");
			string lexDicStr = "../data/Dictionary/lexiconDic.data";
			int fieldNo = 1;
			RootDictionary dic2 = new BasicDictionary(lexDicStr, fieldNo);
			Console.WriteLine("- Dic2 File: " + lexDicStr);
			Console.WriteLine("- Dic2 size: " + dic2.GetSize());
			string numDicStr = "../data/Dictionary/NRVAR.1.uSort.data";
			dic2.AddDictionary(numDicStr);
			Console.WriteLine("- Dic2 File: " + numDicStr);
			Console.WriteLine("- Dic2 size: " + dic2.GetSize());
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
			wordList.Add("ella");
			wordList.Add("centillionths");
			wordList.Add("Down's");
			wordList.Add("Downs'");
			wordList.Add("spot(s)");
			wordList.Add("fetus(es)");
			wordList.Add("box(es)");
			wordList.Add("waltz(es)");
			wordList.Add("mtach(es)");
			wordList.Add("splash(es)");
			wordList.Add("fly(ies)");
			wordList.Add("extremity(ies)");
			wordList.Add("CASE/TEST");
			wordList.Add("John's/Chris's");
			wordList.Add("50mg/100mg");
			wordList.Add("case/test");
			wordList.Add("neck-lesion");
			wordList.Add("day-night");
			wordList.Add("pneumonoultramicroscopicsilicovolcanoconiosis");
			wordList.Add("Walmart");
			wordList.Add("test321");
			Console.WriteLine("input|baseline|L-element|Lexicon|L-RealWord");
			foreach (string w in wordList) {
				Console.WriteLine("- IsDicWord(" + w + "): " + dic0.IsDicWord(w) + ", " + dic1.IsDicWord(w) + ", " + dic2.IsDicWord(w) + ", " + dic2.IsValidWord(w));
			}
			string word = "test321";
			Console.WriteLine("------ Add [" + word + "] to dictionary ------");
			dic0.AddWord(word);
			dic1.AddWord(word);
			dic2.AddWord(word);
			Console.WriteLine("- Dic0 size: " + dic0.GetSize());
			Console.WriteLine("- Dic1 size: " + dic1.GetSize());
			Console.WriteLine("- Dic2 size: " + dic2.GetSize());
			Console.WriteLine("- IsInDic(" + word + "): " + dic0.IsDicWord(word) + ", " + dic1.IsDicWord(word) + ", " + dic2.IsDicWord(word));
			Console.WriteLine("===== End of Unit Test =====");
		}
		private static void TestSplitDic(CSpellApi cSpellApi) {
			// test split dictionary
			RootDictionary splitWordDic = cSpellApi.GetSplitWordDic();

			// test words
			List<string> wordList = new List<string>();
			wordList.Add("do");
			wordList.Add("i");
			wordList.Add("ng");
			wordList.Add("ilove");
			foreach (string word in wordList) {
				Console.WriteLine("-- SplitDic(" + word + "): " + splitWordDic.IsDicWord(word));
			}
		}
		private static void TestPnDic(CSpellApi cSpellApi) {
			// test split dictionary
			RootDictionary pnDic = cSpellApi.GetPnDic();

			// test words
			List<string> wordList = new List<string>();
			wordList.Add("hu");
			wordList.Add("Hu");
			foreach (string word in wordList) {
				Console.WriteLine("-- pnDic(" + word + "): " + pnDic.IsDicWord(word));
			}
		}
		// test driver
		public static void MainTest(string[] args) {
			string configFile = "../data/Config/cSpell.properties";
			if (args.Length > 0) {
				Console.WriteLine("Usage: java BasicDictionary");
				Environment.Exit(0);
			}

			// init
			CSpellApi cSpellApi = new CSpellApi(configFile);
			// test case and print out 
			//Test();
			TestSplitDic(cSpellApi);
			TestPnDic(cSpellApi);
		}
		// data member
		private const int DIC_SIZE = 1000000; // initial size for dic: 1M
		private HashSet<string> dictionary_ = new HashSet<string>(DIC_SIZE);
		private bool caseFlag_ = false; // case sensitive flag
	}

}
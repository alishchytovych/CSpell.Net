using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SpellChecker.Engines.cSpell.Util;
using SpellChecker.Engines.cSpell.Extensions;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///***************************************************************************
	/// This is a java object class for Word2Vec object.
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
	public class Word2Vec {
		// public constructor
		public Word2Vec(string inFile) {
			bool verboseFlag = false;
			Init(inFile, verboseFlag);
		}
		public Word2Vec(string inFile, bool verboseFlag) {
			Init(inFile, verboseFlag);
		}

		// WordVectors: key: word, value: DoubleVec
		public virtual IDictionary<string, DoubleVec> GetWordVecMap() {
			return wordVecMap_;
		}

		// the dimension of DoubleVec
		public virtual int GetDimension() {
			return dimension_;
		}

		// total word no (vocabulary count) with DoubleVec
		public virtual int GetWordNo() {
			return wordNo_;
		}
		// check if a word has a wordVec
		public virtual bool HasWordVec(string word) {
			string inWord = GetKeyWord(word);
			bool hasWordVec = wordVecMap_.ContainsKey(inWord);
			return hasWordVec;
		}
		// get the wordVec
		// lowercase the word
		public virtual DoubleVec GetWordVec(string word) {
			string inWord = GetKeyWord(word);
			DoubleVec @out = wordVecMap_.GetValueOrNull(inWord);
			return @out;
		}
		// the key in word2Vec are all lowercased except for URL, NUM, EMAIL
		private static string GetKeyWord(string inWord) {
			if ((inWord.Equals(Word2VecContext.PAT_URL) == true) || (inWord.Equals(Word2VecContext.PAT_EMAIL) == true) || (inWord.Equals(Word2VecContext.PAT_NUM) == true)) {
				return inWord;
			}
			return inWord.ToLower();
		}

		// private methods
		// TBD: convert the format [CONTACT] to [EMAIL]
		// TBD: sync the format in PreProcess: [CONTACT], [NUM], ...
		// TBD: make sure the coreTerm does not take out above pattern
		private static string GetSyncWord(string inWord) {
			string syncWord = inWord;
			if (inWord.Equals("CONTACT", StringComparison.OrdinalIgnoreCase) == true) {
				syncWord = "EMAIL";
			}
			return syncWord;
		}
		/// <summary>
		/// Instantiates a Word2Vec object from a file that was generated 
		/// by the original word2Vec program.
		/// </summary>
		/// <param name="inFile">  the file containing the Word2Vec model. </param>
		private void Init(string inFile, bool verboseFlag) {
			if (wordVecMap_ == null) {
				ReadWordVectors(inFile, verboseFlag);
			}
		}

		// read word vectors from a input file
		private void ReadWordVectors(string inFile) {
			bool verboseFlag = false;
			ReadWordVectors(inFile, verboseFlag);
		}
		private void ReadWordVectors(string inFile, bool verboseFlag) {
			// init 
			wordVecMap_ = new Dictionary<string, DoubleVec>();
			int lineNo = 0;
			string line = null;
			try {
				StreamReader @in = new StreamReader(new FileStream(inFile, FileMode.Open, FileAccess.Read), Encoding.UTF8);
				// read in line by line from a file
				while (!string.ReferenceEquals((line = @in.ReadLine()), null)) {
					var buf = line.Split(" ");
					// first line is the stats wordNo|dimension
					if (lineNo == 0) {
						wordNo_ = int.Parse(buf[0]);
						dimension_ = int.Parse(buf[1]);
						//dimension_ = line0.split(delimiter).length -1;
					} else { // word|vector
						string word = buf[0];
						double[] array = new double[dimension_];
						for (int i = 0; i < dimension_; i++) {
							array[i] = double.Parse(buf[i + 1]);
						}
						// update wordVec
						wordVecMap_[word] = new DoubleVec(array);
					}
					lineNo++;
				}
				// close
				@in.Close();
			} catch (IOException e) {
				Console.Error.WriteLine("** ERR@Word2Vec.ReadWordVectors( ), problem of reading file (" + inFile);
				Console.Error.WriteLine("Line: " + lineNo + " - " + line);
				Console.Error.WriteLine("Exception: " + e.ToString());
			}
			// print out
			if (verboseFlag == true) {
				Console.WriteLine("====== Word2Vec.ReadWordVectors( ) ======");
				Console.WriteLine("- inFile: " + inFile);
				Console.WriteLine("- Word no: " + wordNo_);
				Console.WriteLine("- dimension: " + dimension_);
			}
		}
		// unit test driver
		public static void MainTest(string[] args) {
			//String inFile = "../data/Context/word2Vec.data";
			string inFile = "../data/Context/syn1n.data";
			if (args.Length == 1) {
				inFile = args[0];
			} else if (args.Length > 0) {
				Console.Error.WriteLine("Usage: java Word2Vec <inFile>");
				Environment.Exit(1);
			}
			// test
			try {
				Word2Vec word2Vec = new Word2Vec(inFile);
				Console.WriteLine("Dimension: " + word2Vec.GetDimension());
				Console.WriteLine("Word No: " + word2Vec.GetWordNo());
				Console.WriteLine("Word size in WrodVec: " + word2Vec.GetWordVecMap().Keys.Count);
				Console.WriteLine("HasWordVec(man): " + word2Vec.HasWordVec("man"));
				Console.WriteLine("HasWordVec(king): " + word2Vec.HasWordVec("king"));
				Console.WriteLine("HasWordVec(ago): " + word2Vec.HasWordVec("ago"));
				Console.WriteLine("HasWordVec(a): " + word2Vec.HasWordVec("a"));
				Console.WriteLine("HasWordVec(ia): " + word2Vec.HasWordVec("ia"));
				Console.WriteLine("HasWordVec(m): " + word2Vec.HasWordVec("m"));
				Console.WriteLine("HasWordVec(xyxy): " + word2Vec.HasWordVec("xyxy"));
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		// data members
		// key: word, value: matrix
		private IDictionary<string, DoubleVec> wordVecMap_ = null;
		private int dimension_; // 200
		private int wordNo_; // 20021. same as the key Word no.
	}

}
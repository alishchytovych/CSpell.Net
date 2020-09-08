using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Extensions;

namespace SpellChecker.Engines.cSpell.NlpUtil {
	/// <summary>
	///***************************************************************************
	/// This is the utility class for terms.
	/// This class contains all basic operations for a term (multiwords).
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
	public class TermUtil {
		// private constructor
		private TermUtil() { }
		// public methods
		// remove extra space from a term, such as [A   B] to [A B]
		public static string StringTrim(string @in) {
			var buf = @in.Split(new char[] { ' ', '\t' });
			string @out = "";
			foreach (var item in buf) {
				if(!String.IsNullOrEmpty(item))
					@out += item + " ";
			}
			return @out.Trim(new char[] { ' ', '\x00' });
		}
		public static string RemovePuncSpace(string inTerm) {
			return RemoveChars(inTerm, puncSpace_);
		}
		public static string RemoveChars(string inTerm, string removeCharList) {
			if ((string.ReferenceEquals(inTerm, null)) || (string.ReferenceEquals(removeCharList, null))) {
				return inTerm;
			}
			string outStr = "";
			var buf = inTerm.SplitByAny(removeCharList);
			foreach (var item in buf)
				if (!String.IsNullOrEmpty(item))
					outStr += item + " ";

			return outStr;
		}
		// convert a term (multiwords) into lowercase single words 
		// (separated by space & punctuation)
		public static List<string> ToWordList(string inTerm, bool normFlag) {
			List<string> wordList = new List<string>();
			// lowercase, tokenize words by all puntuations
			if (normFlag == true) {
				var buf = inTerm.ToLower().SplitByAny(puncSpace_);
				foreach (var item in buf) {
					wordList.Add(item);
				}
			} else { // lowercase, use space and tab as delim
				// last space is U+00A0, NO-BREAK SPACE, LATIN_1_SUPPLEMENT
				var buf2 = inTerm.ToLower().SplitByAny(SPACE_STR);
				//" \t ");
				foreach (var item in buf2) {
					if (!String.IsNullOrEmpty(item))
						wordList.Add(item);
				}
			}
			return wordList;
		}
		// split on all unicode space, tab, and new line
		public static int GetWordNo(string inTerm) {
			// include unicode space
			string[] wordArray = inTerm.Split(patternStrSpace_, true);
			int wordNo = wordArray.Length;
			return wordNo;
		}
		// convert a term into words (separated by SPACE_STR: space and tab only)
		// no include spaces
		// SPACE_STR defined at the end of this class, including Unidocde spaces
		public static List<string> ToWordList(string inTerm) {
			// tokenize words
			//StringTokenizer buf = new StringTokenizer(inTerm.trim(), " \t");
			var buf = inTerm.Trim(new char[] { ' ', '\x00' }).SplitByAny(SPACE_STR);
			List<string> wordList = new List<string>();
			foreach (var item in buf) {
				wordList.Add(item);
			}
			return wordList;
		}
		// convert a term into words (separated by ASCII space only)
		public static List<string> ToWordListBySpace(string inTerm) {
			// tokenize words
			var buf = inTerm.Trim(new char[] { ' ', '\x00' }).SplitByAny(" ");
			List<string> wordList = new List<string>();
			foreach (var item in buf) {
				wordList.Add(item);
			}
			return wordList;
		}
		public static List<string> ToWordListBySpaceHyphen(string inTerm) {
			// tokenize words
			var buf = inTerm.Trim(new char[] { ' ', '\x00' }).SplitByAny(" -");
			List<string> wordList = new List<string>();
			foreach (var item in buf) {
				wordList.Add(item);
			}
			return wordList;
		}
		// predicate to check if a multiword term
		public static bool IsMultiword(string inTerm) {
			bool multiwordFlag = false;
			string inTermTrim = inTerm.Trim();
			if (inTermTrim.IndexOf(" ", StringComparison.Ordinal) != -1) {
				multiwordFlag = true;
			}

			return multiwordFlag;
		}
		// strip punctuaction, then trim
		public static string StripPunctuation(string inTerm) {
			int length = inTerm.Length;
			char[] temp = new char[length];
			int index = 0;
			for (int i = 0; i < length; i++) {
				char tempChar = inTerm[i];
				if (CharUtil.IsPunctuation(tempChar) == false) {
					temp[index] = tempChar;
					index++;
				}
			}
			string @out = new string(temp);
			return @out.Trim(new char[] { ' ', '\x00' }); // must be trimmed
		}
		// strip space
		public static string StripSpace(string inTerm) {
			int length = inTerm.Length;
			char[] temp = new char[length];
			int index = 0;
			for (int i = 0; i < length; i++) {
				char tempChar = inTerm[i];
				if (tempChar != ' ') {
					temp[index] = tempChar;
					index++;
				}
			}
			string @out = new string(temp);
			return @out.Trim(new char[] { ' ', '\x00' }); // must be trimmed
		}
		// replace punctuation with space in a string
		public static string ReplacePuncWithSpaceThenTrim(string inTerm) {
			char[] temp = inTerm.ToCharArray();
			for (int i = 0; i < temp.Length; i++) {
				if (CharUtil.IsPunctuation(temp[i]) == true) {
					temp[i] = ' ';
				}
			}
			string @out = Trim(new string(temp));
			return @out;
		}
		// strip leading chars if it is a punctuation
		public static string StripLeadChars(string inTerm, string specChars) {
			string outStrLead = inTerm;
			int index = 0;
			while ((index < outStrLead.Length) && (CharUtil.IsSpecifiedChar(outStrLead[index], specChars) == true)) {
				index++;
			}
			if ((index > 0) && (index < outStrLead.Length)) {
				outStrLead = outStrLead.Substring(index);
			}
			return outStrLead;
		}
		// strip ending chars if it is a punctuation
		public static string StripEndChars(string inTerm, string specChars) {
			string outStrEnd = inTerm;
			int length = outStrEnd.Length;
			int index = length - 1;
			while ((index > -1) && (CharUtil.IsSpecifiedChar(outStrEnd[index], specChars) == true)) {
				index--;
			}
			if ((index < length - 1) && (index > -1)) {
				outStrEnd = outStrEnd.Substring(0, index + 1);
			}
			return outStrEnd;
		}
		// recursively remove legal punctuatin at the end of a term
		// used in merge case
		public static string StripEndPunc(string inTerm) {
			string curTerm = inTerm;
			string outTerm = "";
			// recursively strip .?!,s:;'"s)]}
			while (true) {
				outTerm = StripEndChars(inTerm, endPunc_);
				if (outTerm.Equals(curTerm) == true) {
					break;
				} else if (outTerm.Length < 1) {
					break;
				}
				curTerm = outTerm;
			}
			outTerm.Trim(new char[] { ' ', '\x00' });
			if (outTerm.Length == 0) {
				outTerm = inTerm;
			}
			return outTerm;
		}
		// strip leading chars if it is a punctuation
		public static string StripLeadPuncSpace(string inTerm) {
			string outStrLead = inTerm;
			int index = 0;
			while ((index < outStrLead.Length) && ((CharUtil.IsPunctuation(outStrLead[index]) == true) || (outStrLead[index] == ' '))) {
				index++;
			}
			if ((index > 0) && (index < outStrLead.Length)) {
				outStrLead = outStrLead.Substring(index);
			}
			return outStrLead;
		}
		// strip ending chars if it is a punctuation
		public static string StripEndPuncSpace(string inTerm) {
			string outStrEnd = inTerm;
			int length = outStrEnd.Length;
			int index = length - 1;
			while ((index > -1) && ((CharUtil.IsPunctuation(outStrEnd[index]) == true) || (outStrEnd[index] == ' '))) {
				index--;
			}
			if ((index < length - 1) && (index > -1)) {
				outStrEnd = outStrEnd.Substring(0, index + 1);
			}
			return outStrEnd;
		}
		public static string GetLeadWordFromTerm(string inTerm) {
			string inTerm1 = inTerm.Trim(new char[] { ' ', '\x00' });
			string leadWord = inTerm1.Split(" ", true) [0];
			return leadWord;
		}
		public static string GetEndWordFromTerm(string inTerm) {
			string inTerm1 = inTerm.Trim(new char[] { ' ', '\x00' });
			int index = inTerm1.LastIndexOf(" ", StringComparison.Ordinal);
			string endWord = inTerm1.Substring(index + 1);
			return endWord;
		}
		public static string GetNField(string inTerm, int nField) {
			string outStr = inTerm;
			try {
				outStr = inTerm.Split("\\|", true) [nField];
			}
			//catch(PatternSyntaxException pse)
			catch (Exception) { }
			return outStr;
		}
		// remove all extra space and tab
		// [space  tab] -> [space tab]
		public static string Trim(string inTerm) {
			var buf = inTerm.Split(new char[] { ' ', '\t' });
			string outTerm = "";
			foreach (var item in buf) {
				if (!String.IsNullOrEmpty(item))
					outTerm += item + " ";
			}
			return outTerm.Trim(new char[] { ' ', '\x00' });
		}
		// private methods
		private static void Test() {
			Console.WriteLine("===== Unit Test of TermUtil =====");
			string inStr = ":(This is a A-1--2 test),;  test Unicode [ ] link: http:";
			Console.WriteLine("--inStr: [" + inStr + "]");
			Console.WriteLine("-- wordNo: [" + GetWordNo(inStr) + "]");
			string inStr2 = "Test­unicode space\t[ ­] end.\nmore";
			Console.WriteLine("--unicode space Str: [" + inStr2 + "]");
			Console.WriteLine("-- unicode space wordNo: [" + GetWordNo(inStr2) + "]");
			Console.WriteLine("--StripPunctuation: [" + StripPunctuation(inStr) + "]");
			Console.WriteLine("--ReplacePuncWithSpaceThenTrim: [" + ReplacePuncWithSpaceThenTrim(inStr) + "]");
			Console.WriteLine("------- Strip Punc, leadning Punc, ending punc");
			Console.WriteLine("-- SP: [" + StripPunctuation(inStr) + "]");
			Console.WriteLine("-- SLPS: [" + StripLeadPuncSpace(inStr) + "]");
			Console.WriteLine("-- SEPS: [" + StripEndPuncSpace(inStr) + "]");
			Console.WriteLine("-------");
			string testStr = "  - of - ";
			Console.WriteLine("-- testStr: [" + testStr + "]");
			Console.WriteLine("--SP: [" + StripPunctuation(testStr) + "]");
			Console.WriteLine("--SLPS: [" + StripLeadPuncSpace(testStr) + "]");
			Console.WriteLine("--SEPS: [" + StripEndPuncSpace(testStr) + "]");
			Console.WriteLine("===== End of Unit Test =====");
			Console.WriteLine("----- StripEndPuncSpace -----");
			string str1 = "tests?.";
			Console.WriteLine("-- StripEndPuncSpace(" + str1 + "): [" + StripEndPuncSpace(str1) + "]");
			str1 = "test..";
			Console.WriteLine("-- StripEndPuncSpace(" + str1 + "): [" + StripEndPuncSpace(str1) + "]");
			str1 = "test..1";
			Console.WriteLine("-- StripEndPuncSpace(" + str1 + "): [" + StripEndPuncSpace(str1) + "]");
		}
		// Unit test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("** Usage: java TermUtil");
			}
			// test
			Test();
		}
		// data members
		// U+00A0, NO-BREAK SPACE, LATIN_1_SUPPLEMENT is icnlude as the last space
		private const string puncSpace_ = " \t-({[)}]_!@#%&*\\:;\"',.?/~+=|<>$`^\xA0";
		// also include unicode space:
		// \s includes: [ \t\n\x0B\f\r]
		// U+00A0, NO-BREAK SPACE
		// U+00AD, SOFT HYPHEN
		private const string SPACE_STR = " \t\xA0";
		private const string patternStrSpace_ = "[\\s ­]+";
		// legal punctuation at the end of a term
		private const string endPunc_ = ".?!,:;'\")}]";
	}

}
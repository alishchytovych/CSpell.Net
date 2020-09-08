using System;
using System.Collections.Generic;

namespace SpellChecker.Engines.cSpell.NlpUtil {
	/// <summary>
	///***************************************************************************
	/// This is the utility class for core term operation.
	/// This class is used to converts a token to core-term.
	/// A core-term is to remove all punctuation at the leading and ending of the 
	/// token except for closed brackets, such as (), [], {}, and &lt;&gt;
	/// 
	/// Algorithm:
	/// <ul>
	/// <li>strip leading chars if they are punctuation, except for left closed brackets
	/// <li>strip ending chars if they are punctuation, except for right closed brackets
	/// <li>recursively strip closed brackets of (), [], {}, &lt;&gt; at both ends.
	///     <ul>
	///     <li>strip lead end bracket if netBracketNo = 0
	///     <li>strip lead bracket if netBracketNo &gt; 0
	///     <li>strip end bracket if netBracketNo &lt; 0
	///     </ul>
	/// </ul>
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
	public class CoreTermUtil {
		// private constructor
		private CoreTermUtil() {
		}
		// public methods
		// the input term can be a word or a term
		public static string GetCoreTerm(string inTerm, int ctType) {
			bool lcFlag = false;
			return GetCoreTerm(inTerm, ctType, lcFlag);
		}
		public static string GetCoreTerm(string inTerm, int ctType, bool lcFlag) {
			string curTerm = inTerm;
			string outTerm = "";
			// recursively strip (), [], {}, <>
			while (true) {
				outTerm = GetCoreTermBySteps(curTerm, ctType);
				if (outTerm.Equals(curTerm) == true) {
					break;
				}
				else if (outTerm.Length < 1) {
					break;
				}
				curTerm = outTerm;
			}
			// reset it back to input term if no coreTerm, such as "()"
			outTerm.Trim();
			if (outTerm.Length == 0) {
				outTerm = inTerm;
			}
			if (lcFlag == true) {
				outTerm = outTerm.ToLower();
			}
			return outTerm;
		}
		public static string GetCoreTermBySteps(string inTerm, int ctType) {
			string leadStripChars = leadSpacePunc_;
			string endStripChars = endSpacePunc_;
			if (ctType == CT_TYPE_SPACE_PUNC_DIGIT) {
				leadStripChars = leadSpacePuncDigit_;
				endStripChars = endSpacePuncDigit_;
			}
			// 1. Strip lead end chars that is a punct
			string outStrLead = TermUtil.StripLeadChars(inTerm, leadStripChars);
			string outStrEnd = TermUtil.StripEndChars(outStrLead, endStripChars);
			// 2. strip lead end brackets
			string outStrBracket = StripLeadEndBrackets(outStrEnd);
			string outStrLeadBracket = StripLeadBrackets(outStrBracket);
			string outStrEndBracket = StripEndBrackets(outStrLeadBracket);
			// 3. trim
			string outTerm = TermUtil.Trim(outStrEndBracket);
			return outTerm;
		}
		// private methods
		// inTerm can be a term or a word
		private static string StripLeadEndBrackets(string inTerm) {
			if (inTerm.Length == 0) {
				return inTerm;
			}
			string outTerm = inTerm;
			int lastIndex = inTerm.Length - 1;
			char leadChar = inTerm[0];
			char endChar = inTerm[lastIndex];
			// even bracket
			if (((leadChar == '(') && (endChar == ')') && (GetNetCharNo(inTerm, '(', ')') == 0)) || ((leadChar == '[') && (endChar == ']') && (GetNetCharNo(inTerm, '[', ']') == 0)) || ((leadChar == '{') && (endChar == '}') && (GetNetCharNo(inTerm, '{', '}') == 0)) || ((leadChar == '<') && (endChar == '>') && (GetNetCharNo(inTerm, '<', '>') == 0))) {
				outTerm = inTerm.Substring(1, lastIndex - 1);
			}
			return outTerm;
		}
		// Strip symmetric brackets at lead end chars
		private static string StripLeadBrackets(string inTerm) {
			if (inTerm.Length == 0) {
				return inTerm;
			}
			string outTerm = inTerm;
			int lastIndex = inTerm.Length - 1;
			char leadChar = inTerm[0];
			char endChar = inTerm[lastIndex];
			// uneven brackets
			if (((leadChar == '(') && (GetNetCharNo(inTerm, '(', ')') > 0)) || ((leadChar == '[') && (GetNetCharNo(inTerm, '[', ']') > 0)) || ((leadChar == '{') && (GetNetCharNo(inTerm, '{', '}') > 0)) || ((leadChar == '<') && (GetNetCharNo(inTerm, '<', '>') > 0))) {
				outTerm = inTerm.Substring(1);
			}
			return outTerm;
		}
		private static string StripEndBrackets(string inTerm) {
			if (inTerm.Length == 0) {
				return inTerm;
			}
			string outTerm = inTerm;
			int lastIndex = inTerm.Length - 1;
			char leadChar = inTerm[0];
			char endChar = inTerm[lastIndex];
			if (((endChar == ')') && (GetNetCharNo(inTerm, '(', ')') < 0)) || ((endChar == ']') && (GetNetCharNo(inTerm, '[', ']') < 0)) || ((endChar == '}') && (GetNetCharNo(inTerm, '{', '}') < 0)) || ((endChar == '>') && (GetNetCharNo(inTerm, '<', '>') < 0))) {
				outTerm = inTerm.Substring(0, lastIndex);
			}
			return outTerm;
		}
		// find the total number of a char (must be a single character string
		// netCharNo = leftCharNo - rightCharNo
		private static int GetNetCharNo(string inTerm, char leftChar, char rightChar) {
			int netCharNo = 0;
			for (int i = 0; i < inTerm.Length; i++) {
				char curChar = inTerm[i];
				if (curChar == leftChar) {
					netCharNo++;
				}
				else if (curChar == rightChar) {
					netCharNo--;
				}
			}
			return netCharNo;
		}
		private static string StripLeadEndBracket(string inTerm) {
			string outTerm = inTerm;
			int lastIndex = inTerm.Length - 1;
			char leadChar = inTerm[0];
			char endChar = inTerm[lastIndex];
			if (((leadChar == '(') && (endChar == ')')) || ((leadChar == '[') && (endChar == ']')) || ((leadChar == '{') && (endChar == '}')) || ((leadChar == '<') && (endChar == '>'))) {
				outTerm = inTerm.Substring(1, lastIndex - 1);
			}
			return outTerm;
		}
		private static void Test(string inWord) {
			Console.WriteLine("===== Unit Test of CoreTermUtil =====");
			List<string> inTermList = new List<string>();
			inTermList.Add("- in details");
			inTermList.Add("- In details:");
			inTermList.Add("#$%IN DETAILS:%^(");
			inTermList.Add("");
			inTermList.Add(" ");
			inTermList.Add("(");
			inTermList.Add("()");
			inTermList.Add("[()]");
			inTermList.Add("$%5^&");
			inTermList.Add("$%%^&");
			inTermList.Add("{$%%^&}");
			inTermList.Add("{in (5) details}");
			inTermList.Add("{{in (5) details}");
			inTermList.Add("{in (5) details}}");
			inTermList.Add("{in (5)} details}}");
			inTermList.Add("(in details:)");
			inTermList.Add("(in details:))");
			inTermList.Add("(-(in details)%^)");
			inTermList.Add("{in (5) days},");
			inTermList.Add("in (5 days),");
			inTermList.Add("in ((5) days),");
			inTermList.Add("((clean room(s)))");
			inTermList.Add("((inch(es)))");
			inTermList.Add("(%) decreased");
			inTermList.Add(" space ");
			inTermList.Add("-punc,");
			inTermList.Add(" spacePunc: ");
			inTermList.Add("-digit21:");
			inTermList.Add("12digit21");
			inTermList.Add(" spacePuncDigit: 12");
			// new data with unicode
			inTermList.Add(" which");
			inTermList.Add("“eye”");
			inTermList.Add("•radical");
			// from input
			inTermList.Add("i.e.,");
			inTermList.Add("[ORGANIZATION][LOCATION].");
			inTermList.Add("(dob-[DATE]),");
			inTermList.Add("46XY,dup(16)(q13q23).");
			inTermList.Add("girl(5'8\"),");
			inTermList.Add("c8899A");
			inTermList.Add("Hirayama's");
			inTermList.Add("U.");
			inTermList.Add("U.2");
			inTermList.Add("U2.");
			inTermList.Add(inWord);
			int ctType1 = CT_TYPE_SPACE_PUNC; // include digit
			int ctType2 = CT_TYPE_SPACE_PUNC_DIGIT; // no digit
			foreach (string inTerm in inTermList) {
				Console.WriteLine("- GetCoreTerm(" + inTerm + "): [" + GetCoreTerm(inTerm, ctType1) + "], [" + GetCoreTerm(inTerm, ctType2) + "]");
			}
			Console.WriteLine("===== End of Unit Test =====");
		}
		// test driver
		public static void MainTest(string[] args) {
			string inWord = "i.e.,";
			if (args.Length == 1) {
				inWord = args[0];
			}
			else if (args.Length > 0) {
				Console.WriteLine("** Usage: java CoreTermUtil <inWord>");
			}
			// test
			Test(inWord);
		}
		// data members
		public const int CT_TYPE_SPACE_PUNC = 1; // no spacePunc in ct
		public const int CT_TYPE_SPACE_PUNC_DIGIT = 2; // no sapcePuncDigit
		private const string leadSpacePunc_ = " \t-)}]_!@#%&*\\:;\"',.?/~+=|>$`^ “•”";
		private const string endSpacePunc_ = " \t-({[_!@#%&*\\:;\"',.?/~+=|<$`^ “•”";
		private const string leadSpacePuncDigit_ = " \t-)}]_!@#%&*\\:;\"',.?/~+=|>$`^ “•”0123456789";
		private const string endSpacePuncDigit_ = " \t-({[_!@#%&*\\:;\"',.?/~+=|<$`^ “•”0123456789";
	}

}
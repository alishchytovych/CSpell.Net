using System;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.NlpUtil {
	/// <summary>
	///***************************************************************************
	/// This class is the java object for split.
	/// This class is used in non-dictionary splitter.
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
	public class SplitObj {
		// public constructor
		/// <summary>
		/// Public constructor 
		/// </summary>
		public SplitObj() { }
		// no suffix is present
		public SplitObj(string inStr) {
			prefix_ = inStr;
		}
		public SplitObj(string prefix, string suffix) {
			prefix_ = prefix;
			suffix_ = suffix;
		}
		public virtual void SetPrefix(string prefix) {
			prefix_ = prefix;
		}
		public virtual void SetSuffix(string suffix) {
			suffix_ = suffix;
		}
		public virtual string GetPrefix() {
			return prefix_;
		}
		public virtual string GetSuffix() {
			return suffix_;
		}
		public virtual string ToString() {
			// there is not split if the suffix is empty
			string outStr = prefix_;
			if (suffix_.Length != 0) {
				outStr = ToString(prefix_, suffix_);
			}
			return outStr;
		}
		public static string ToString(string prefix, string suffix) {
			string outStr = prefix + GlobalVars.SPACE_STR + suffix;
			return outStr;
		}
		// public methods
		// splitStr is the delimiter str where a space is added before it
		// split at the lastIndex of splitStr, after the splitStr
		public static string GetSplitStrAfterPunc(string inStr, string splitStr) {
			string outStr = inStr;
			int lastIndex = inStr.LastIndexOf(splitStr, StringComparison.Ordinal);
			int length = splitStr.Length;
			if ((lastIndex != -1) && (lastIndex != inStr.Length - length)) { // not at the end
				string prefix = inStr.Substring(0, lastIndex + length);
				string suffix = inStr.Substring(lastIndex + length);
				outStr = prefix + GlobalVars.SPACE_STR + suffix;
			}
			return outStr;
		}
		// split at the first index of splitStr, before the splitStr
		public static string GetSplitStrBeforePunc(string inStr, string splitStr) {
			string outStr = inStr;
			int index = inStr.IndexOf(splitStr, StringComparison.Ordinal);
			int length = splitStr.Length;
			if ((index != -1) && (index != 0)) { // not at the beginning
				string prefix = inStr.Substring(0, index);
				string suffix = inStr.Substring(index);
				outStr = prefix + GlobalVars.SPACE_STR + suffix;
			}
			return outStr;
		}
		// splitStr is the delimiter str where a space is added before it
		// split at the lastIndex of splitStr
		public static SplitObj GetSplitObj(string inStr, string splitStr) {
			SplitObj splitObj = new SplitObj(inStr);
			int index = inStr.IndexOf(splitStr, StringComparison.Ordinal);
			int lastIndex = inStr.LastIndexOf(splitStr, StringComparison.Ordinal);
			int length = splitStr.Length;
			if ((index != -1) && (lastIndex != inStr.Length - length)) { // not at the end
				string prefix = inStr.Substring(0, lastIndex + 1);
				string suffix = inStr.Substring(lastIndex + 1);
				splitObj = new SplitObj(prefix, suffix);
			}
			return splitObj;
		}
		// private methods
		private static void Test() {
			Console.WriteLine("===== Unit Test of SplitObj =====");
			/*
			ArrayList<String> inTermList = new ArrayList<String>();
			inTermList.add("- in details");
			inTermList.add("#$%IN DETAILS:%^(");
			inTermList.add("");
			inTermList.add(" ");
			inTermList.add("()");
			inTermList.add("-http://www.nih.gov");
			for(String inTerm:inTermList)
			{
			    CoreTermObj cto = new CoreTermObj(inTerm);
			    String outStr = cto.ToString();
			    System.out.println(inTerm + "|" + cto.GetPrefix() + "|"
			        + cto.GetCoreTerm() + "|" + cto.GetSuffix() + "|"
			        + cto.ToString() + "|" + inTerm.equals(outStr));
			}
			*/
			Console.WriteLine("===== End of Unit Test =====");
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java SplitObj");
				Environment.Exit(0);
			}

			// test case and print out 
			Test();
		}
		// data member
		private string prefix_ = ""; // leading Str before the split
		private string suffix_ = ""; // ending str after the split
	}

}
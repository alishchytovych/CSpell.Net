using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.NlpUtil {
	/// <summary>
	///***************************************************************************
	/// This class is the java object for coreTerm.
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
	public class CoreTermObj {
		// public constructor
		/// <summary>
		/// Public constructor 
		/// </summary>
		/// <param name="inStr"> input string </param>
		/// <param name="ctType"> core term type, defined in CoreTermUtil.java </param>
		public CoreTermObj(string inStr, int ctType) {
			Process(inStr, ctType);
		}
		public virtual void SetPrefix(string prefix) {
			prefix_ = prefix;
		}
		public virtual void SetCoreTerm(string coreTerm) {
			coreTerm_ = coreTerm;
		}
		public virtual void SetSuffix(string suffix) {
			suffix_ = suffix;
		}
		public virtual string GetCoreTerm() {
			return coreTerm_;
		}
		public virtual string GetPrefix() {
			return prefix_;
		}
		public virtual string GetSuffix() {
			return suffix_;
		}
		// compose the object and converts backto String format
		public virtual string ToString() {
			string ourStr = prefix_ + coreTerm_ + suffix_;
			return ourStr;
		}
		public virtual string ToDetailString() {
			string ourStr = prefix_ + GlobalVars.FS_STR + coreTerm_ + GlobalVars.FS_STR + suffix_;
			return ourStr;
		}
		// public methods
		public virtual void Process(string inTerm, int ctType) {
			// 1. get coreterm from the input Term
			coreTerm_ = CoreTermUtil.GetCoreTerm(inTerm, ctType);
			// 2. get prefix and suffix
			int inLength = inTerm.Length;
			int coreLength = coreTerm_.Length;
			if ((coreLength > 0) && (coreLength < inLength)) { // coreTerm = strip punc
				int index = inTerm.IndexOf(coreTerm_, StringComparison.Ordinal);
				int indexS = index + coreLength;
				// Check error: should not happen
				if (index == -1) {
					Console.Error.WriteLine("** Err@CoreTermObj.Process(" + inTerm + "): prefix too small");
				} else if (indexS > inLength) {
					Console.Error.WriteLine("** Err@CoreTermObj.Process(" + inTerm + "): suffix too big");
				}
				// get prefix
				if (index > 0) {
					prefix_ = inTerm.Substring(0, index);
				}
				// get suffix
				if (indexS < inLength) {
					suffix_ = inTerm.Substring(indexS);
				}
			}
		}
		// private methods
		private static void Test() {
			Console.WriteLine("===== Unit Test of TokenUtil =====");
			List<string> inTermList = new List<string>();
			inTermList.Add("- in details");
			inTermList.Add("#$%IN DETAILS:%^(");
			inTermList.Add("");
			inTermList.Add(" ");
			inTermList.Add("()");
			inTermList.Add("-http://www.nih.gov");
			inTermList.Add("U.");
			inTermList.Add("U.2");
			int ctType = CoreTermUtil.CT_TYPE_SPACE_PUNC_DIGIT;
			foreach (string inTerm in inTermList) {
				CoreTermObj cto = new CoreTermObj(inTerm, ctType);
				string outStr = cto.ToString();
				Console.WriteLine(inTerm + "|" + cto.GetPrefix() + "|" + cto.GetCoreTerm() + "|" + cto.GetSuffix() + "|" + cto.ToString() + "|" + inTerm.Equals(outStr));
			}
			Console.WriteLine("===== End of Unit Test =====");
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java CoreTermObj");
				Environment.Exit(0);
			}

			// test case and print out 
			Test();
		}
		// data member
		private string coreTerm_ = "";
		private string prefix_ = ""; // leading punc chars
		private string suffix_ = ""; // ending punc chars
	}

}
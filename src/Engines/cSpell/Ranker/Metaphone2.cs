using System;
using System.Collections.Generic;
using Phonix;
using SpellChecker.Engines.cSpell.Lib;

namespace SpellChecker.Engines.cSpell.Ranker {
	/// <summary>
	///***************************************************************************
	/// This class provides a java class to get the doubleMetaphone code of a string
	/// using org.apache.commons.codec.language.DoubleMetaphone.
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
	public class Metaphone2 {
		// private constructor
		private Metaphone2() { }
		// public method
		public static string GetCode(string inStr) {
			string outStr = dm_.BuildKey(inStr);
			return outStr;
		}
		public static string GetCode(string inStr, int maxCodeLength) {
			//dm_.MaxCodeLen = maxCodeLength;
			string outStr = dm_.BuildKey(inStr);
			return outStr;
		}
		public static int GetDistance(string str1, string str2) {
			int maxCodeLength = 10;
			return GetDistance(str1, str2, maxCodeLength);
		}
		public static int GetDistance(string str1, string str2, int maxCodeLength) {
			string str1Lc = str1.ToLower();
			string str2Lc = str2.ToLower();
			string str1Code = GetCode(str1Lc, maxCodeLength);
			string str2Code = GetCode(str2Lc, maxCodeLength);
			int dist = EditDistance.GetEditDistance(str1Code, str2Code);
			return dist;
		}
		public static string GetDistanceDetailStr(string str1, string str2, int maxCodeLength) {
			string str1Lc = str1.ToLower();
			string str2Lc = str2.ToLower();
			string str1Code = GetCode(str1Lc, maxCodeLength);
			string str2Code = GetCode(str2Lc, maxCodeLength);
			int dist = EditDistance.GetEditDistance(str1Code, str2Code);
			string detailStr = str1Code + GlobalVars.FS_STR + str2Code + GlobalVars.FS_STR + dist;
			return detailStr;
		}
		// alternate: use alternate encode
		public static string GetCode(string inStr, int maxCodeLength, bool alternate) {
			//dm_.MaxCodeLen = maxCodeLength;
			string outStr = dm_.BuildKey(inStr);
			return outStr;
		}
		public static void SetMaxCodeLen(int maxCodeLength) {
			//dm_.MaxCodeLen = maxCodeLength;
		}
		public static bool IsEqualCode(string str1, string str2) {
			bool flag = dm_.IsSimilar(new String[] { str1, str2 });
			return flag;
		}
		public static bool IsEqualCode(string str1, string str2, bool alternate) {
			bool flag = dm_.IsSimilar(new String[] { str1, str2 });
			return flag;
		}
		// private methods
		private static void Test(string str) {
			List<string> inStrList = new List<string>();
			inStrList.Add("zinc trisulphonatophthalocyanine");
			inStrList.Add("anemia");
			inStrList.Add("anaemia");
			inStrList.Add("yuppie flu");
			inStrList.Add("yuppy flu");
			inStrList.Add("toxic edema");
			inStrList.Add("toxic oedema");
			inStrList.Add("careful");
			inStrList.Add("carefully");
			inStrList.Add("zyxorin");
			inStrList.Add("zyxoryn");
			inStrList.Add("dianosed");
			inStrList.Add("diagnosed");
			inStrList.Add("diagnose");
			inStrList.Add("diagnost");
			inStrList.Add("diagnosed");
			inStrList.Add("truely");
			inStrList.Add("truly");
			inStrList.Add("knoledge");
			inStrList.Add("knowledge");
			inStrList.Add(str);
			int maxCodeLength = 10;
			// print out
			Console.WriteLine("-- maxCodeLength: [" + maxCodeLength + "]");
			foreach (string inStr in inStrList) {
				string metaphone = GetCode(inStr, maxCodeLength);
				Console.WriteLine("- [" + inStr + "] => [" + metaphone + "]");
			}
		}

		// test drive
		public static void MainTest(string[] args) {
			string inStr = "";
			if (args.Length == 1) {
				inStr = args[0];
			} else if (args.Length > 0) {
				Console.Error.WriteLine("*** Usage: java Metaphone2 <inStr>");
				Environment.Exit(1);
			}
			// test
			Test(inStr);
		}
		//private methods, set the length to 10.
		private static DoubleMetaphone dm_ = new DoubleMetaphone(10);
	}

}
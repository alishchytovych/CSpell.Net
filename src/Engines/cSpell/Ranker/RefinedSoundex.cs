using System;
using System.Collections.Generic;
using Phonix;
using SpellChecker.Engines.cSpell.Lib;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///***************************************************************************
	/// This class provides a java class to get the refined soundex code form 
	/// of a string using org.apache.commons.codec.language.RefinedSoundex. 
	/// A refined soundex code is optimized for spelling checking words. 
	/// Soundex originally developed by Margaret Odell and Robert Russell.
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ***************************************************************************
	/// </summary>
	public class RefinedSoundex {
		// private constructor
		private RefinedSoundex() { }
		// Compute Edit (Levenshtein) distance
		public static bool IsEqualCode(string str1, string str2) {
			bool flag = GetCode(str1).Equals(GetCode(str2));
			return flag;
		}
		public static bool IsEqualSoundex(string str1, string str2) {
			bool flag = GetSoundex(str1).Equals(GetSoundex(str2));
			return flag;
		}
		public static int GetDistance(string str1, string str2) {
			string str1Lc = str1.ToLower();
			string str2Lc = str2.ToLower();
			string str1Code = GetCode(str1Lc);
			string str2Code = GetCode(str2Lc);
			int dist = EditDistance.GetEditDistance(str1Code, str2Code);
			return dist;
		}
		public static string GetDistanceDetailStr(string str1, string str2) {
			string str1Lc = str1.ToLower();
			string str2Lc = str2.ToLower();
			string str1Code = GetCode(str1Lc);
			string str2Code = GetCode(str2Lc);
			int dist = EditDistance.GetEditDistance(str1Code, str2Code);
			string detailStr = str1Code + GlobalVars.FS_STR + str2Code + GlobalVars.FS_STR + dist;
			return detailStr;
		}
		public static string GetCode(string inStr) {
			string outStr = rs_.BuildKey(inStr);
			return outStr;
		}
		public static string GetSoundex(string inStr) {
			string outStr = rs_.BuildKey(inStr);
			return outStr;
		}
		// public method
		public static void MainTest(string[] args) {
			List<string> inStrList = new List<string>();
			if (args.Length == 1) {
				inStrList.Add(args[0]);
			} else if (args.Length == 0) {
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
				inStrList.Add("zymographical");
				inStrList.Add("zymographically");
				inStrList.Add("absorption test");
				inStrList.Add("absorption tests");
				inStrList.Add("effect");
				inStrList.Add("affect");
				inStrList.Add("now");
				inStrList.Add("know");
				inStrList.Add("there");
				inStrList.Add("their");
			} else if (args.Length > 0) {
				Console.Error.WriteLine("*** Usage: java RefinedSoundex <inStr>");
				Environment.Exit(1);
			}
			// print out
			foreach (string inStr in inStrList) {
				string code = GetCode(inStr);
				string soundex = GetSoundex(inStr);
				Console.WriteLine("- [" + inStr + "] => [" + code + "|" + soundex + "]");
			}
			Console.WriteLine("-- effect|affect: " + IsEqualCode("effect", "affect") + "|" + IsEqualSoundex("effect", "affect"));
			Console.WriteLine("-- now|know: " + IsEqualCode("now", "know") + "|" + IsEqualSoundex("now", "know"));
		}
		//private methods
		private static Soundex rs_ = new Soundex(10);
	}

}
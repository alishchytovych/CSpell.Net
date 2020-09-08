using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.NlpUtil {
	/// <summary>
	///***************************************************************************
	/// This NLP utility class handles internet related operations. Such as check
	/// if a token is an eMail or URL.
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
	public class InternetTokenUtil {
		// public constructor
		/// <summary>
		/// private constructor 
		/// </summary>
		private InternetTokenUtil() { }
		// public methods
		/// <summary>
		/// A method to validate a token is an URL.
		/// </summary>
		/// <param name="inToken">    the input token (single word)
		/// </param>
		/// <returns>    true if the inToken is a valid URL, false if otherwise. </returns>
		public static bool IsUrl(string inToken) {
			bool checkEmptyTokenFlag = true;
			return TokenUtil.IsMatch(inToken, patternU_, checkEmptyTokenFlag);
		}
		/// <summary>
		/// A method to validate a token is an eMail address
		/// </summary>
		/// <param name="inToken">    the input token (single word)
		/// </param>
		/// <returns>    true if the inToken is a valid eMail address, 
		///             false if otherwise. </returns>
		public static bool IsEmail(string inToken) {
			bool checkEmptyTokenFlag = true;
			return TokenUtil.IsMatch(inToken, patternE_, checkEmptyTokenFlag);
		}
		// private methods
		private static void TestEmail() {
			List<string> inWordList = new List<string>();
			inWordList.Add("ab#=$%c@mail.nih.gov");
			inWordList.Add("abc@mail.nih.gov");
			inWordList.Add("abc@gmail.com");
			inWordList.Add("abc@com.gmail");
			inWordList.Add("abc@mail.google");
			inWordList.Add("abc@mail");
			inWordList.Add("abc@com");
			inWordList.Add("abc@123.net");
			inWordList.Add("123@gmail.com");
			inWordList.Add("12ab%^@gamil.com");
			inWordList.Add("_+-@gamil.com");
			inWordList.Add("!!@gamil.com");
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsEmail(" + inWord + "): " + IsEmail(inWord));
			}
		}
		private static void TestUrl() {
			List<string> inWordList = new List<string>();
			inWordList.Add("https://yahoo.com");
			inWordList.Add("http://yahoo.com");
			inWordList.Add("http://www.yahoo.com");
			inWordList.Add("www.yahoo.com");
			inWordList.Add("yahoo.com");
			inWordList.Add("com");
			inWordList.Add("http://www.yahoo.com?test=1%20try%20abc");
			inWordList.Add("clinicaltrials.gov");
			inWordList.Add("male.read");
			inWordList.Add("co.uk");
			inWordList.Add("co.ch");
			inWordList.Add("https://ehlers-danlos.com/eds-types/");
			inWordList.Add("http://emedicine.medscape.com/article/1143167-treatment");
			inWordList.Add("http://emedicine.medscape.com/article/1143167-treatment.");
			inWordList.Add("https://ehlers-danlos.com/eds-types/");
			inWordList.Add("http://www.dupuytren-online.info/ledderhose_therapies.html");
			inWordList.Add("http://www.newbornscreening.info/parents/organicaciddisorders/mma_hcu.html#4");
			inWordList.Add("http://live-naa.pantheon.io/wp-content/uploads/2014/12/managing-ppa.pdf.");
			inWordList.Add("http://goo.gl/c4rm4p");
			inWordList.Add("good.bad");
			foreach (string inWord in inWordList) {
				Console.WriteLine("- IsUrl(" + inWord + "): " + IsUrl(inWord));
			}
		}
		private static void Test() {
			Console.WriteLine("===== Unit Test of InternetTokenUtil =====");
			TestEmail();
			Console.WriteLine("-------");
			TestUrl();
			Console.WriteLine("===== End of Unit Test =====");
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java InternetTokenUtil");
				Environment.Exit(0);
			}

			// test case and print out 
			Test();
		}
		// data member
		// eMail
		private const string patternStrE_ = "^[\\w!#$%&'*+-/=?^_`{|}~]+@(\\w+(\\.\\w+)*(\\.(gov|com|org|edu|mil|net)))$";
		private static readonly Regex patternE_ = new JRegex(patternStrE_, RegexOptions.Compiled);
		// URL
		private const string patternStrU_ = "^((ftp|http|https|file)://)?((\\w|\\-)+(\\.(\\w|\\-)+)*(\\.(gov|com|org|edu|mil|net|uk|info|io|gl)).*)$";
		private static readonly Regex patternU_ = new JRegex(patternStrU_, RegexOptions.Compiled);
	}

}
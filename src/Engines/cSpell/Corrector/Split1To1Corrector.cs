using System;
using System.Collections.Generic;
using System.Linq;
using SpellChecker.Engines.cSpell.Extensions;
using SpellChecker.Engines.cSpell.NlpUtil;

namespace SpellChecker.Engines.cSpell.Corrector {
	/// <summary>
	///***************************************************************************
	/// This class is the java class to correct split and 1To1.
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
	public class Split1To1Corrector {
		// public constructor
		/// <summary>
		/// Private constructor
		/// </summary>
		private Split1To1Corrector() { }
		// 3 operations:
		// convert a tokenObj to a arrayList of tokenObjs:
		// 1. merge (delete) a tokenObj if the str is empty (length = 0)
		// 2. keep the same tokenObj if str is a single word
		// 3. split a tokenObj if the str contains space
		public static void AddSplit1To1Correction(List<TokenObj> inList, TokenObj inToken) {
			string tokenStr = inToken.GetTokenStr();
			// 1. do not add to the list if the token is empty
			if ((string.ReferenceEquals(tokenStr, null)) || (tokenStr.Length == 0)) {
				// do nothing
			}
			// 2. keep the same tokenObj if there is no change
			// 1-to-1 correction
			else if (TermUtil.IsMultiword(tokenStr) == false) {
				Add1To1Correction(inList, inToken);
				// TB Deleted
				//inList.add(inToken);
			}
			// 3. split a tokenObj to an arrayList if the str has space
			else {
				AddSplitCorrection(inList, inToken);
				/* TB deleted
				ArrayList<TokenObj> tempTokenList = new ArrayList<TokenObj>(); 
				// keep token and delimiters
				String[] tokenArray = tokenStr.split(TextObj.patternStrSpace_);
				tempTokenList = new ArrayList<TokenObj>(Arrays.stream(tokenArray)
				    .map(token -> new TokenObj(inToken, token))
				    .collect(Collectors.toList()));
				inList.addAll(tempTokenList);
				*/
			}
		}
		// private method
		private static void Add1To1Correction(List<TokenObj> inList, TokenObj inToken) {
			inList.Add(inToken);
		}
		// use flat map to add split words to the list
		private static void AddSplitCorrection(List<TokenObj> inList, TokenObj inToken) {
			List<TokenObj> tempTokenList = new List<TokenObj>();
			// keep token and delimiters
			string tokenStr = inToken.GetTokenStr();
			string[] tokenArray = tokenStr.Split(TextObj.patternStrSpace_, true);
			// flat Map
			tempTokenList = new List<TokenObj>(tokenArray.Select(token => new TokenObj(inToken, token)).ToList());
			inList.AddRange(tempTokenList);
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java Split1To1Corrector <inFile>");
				Environment.Exit(0);
			}

			// init
		}
		// data member
	}

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.Extensions;

namespace SpellChecker.Engines.cSpell.NlpUtil {
	/// <summary>
	///***************************************************************************
	/// This class is the java object for cSpell text.
	/// A text can be composed of sentences, phrases, words.
	/// A TextObj is composed of a list of TokenObjs.
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
	public class TextObj {
		// public constructor
		/// <summary>
		/// Public constructor to initiate the Text object.
		/// </summary>
		public TextObj() { }
		/// <summary>
		/// Public constructor to initiate the text Obj.
		/// </summary>
		/// <param name="text">    the text </param>
		public TextObj(string text) {
			Init(text);
		}
		/// <summary>
		/// Public constructor to initiate the text Obj.
		/// </summary>
		/// <param name="tokenList">    the tokenList, cotains single word token
		/// inlcuding pucntuation, space, linebreak, etc. </param>
		public TextObj(List<TokenObj> tokenList) {
			Init(tokenList);
		}
		// public methods
		public virtual void SetText(string text) {
			text_ = text;
		}
		public virtual void SetTokenList(List<TokenObj> tokenList) {
			// further make sure all token is a single word
			tokenList_ = GetAllTokens(tokenList);
		}
		public virtual string GetText() {
			return text_;
		}
		public virtual List<TokenObj> GetTokenList() {
			return tokenList_;
		}
		// convert token to string, put all token to a string by joining
		public virtual string ToString() {
			// put all token + delimiters together
			string outText = String.Join("", tokenList_.Select(token => token.GetTokenStr()));
			return outText;
		}
		// used after split,
		// after split operation, a token might include space(s),
		// use this to convert the token to string and then initiate a new TextObj
		public static string TokenListToText(List<TokenObj> tokenList) {
			// put all token + delimiters together
			string outText = String.Join("", tokenList.Select(token => token.GetTokenStr()));
			return outText;
		}
		// gt the operation details for each token
		public static string TokenListToOperationDetailStr(List<TokenObj> tokenList) {
			/// <summary>
			/// To Be deleted, older implementation
			/// // for loop implementation for Java 7-
			/// String outStr = new String();
			/// int index = 0;
			/// for(TokenObj tokenObj:tokenList)
			/// {
			///    String dStr = index + GlobalVars.FS_STR
			///        + tokenObj.ToString();
			///    outStr += dStr + GlobalVars.LS_STR;        
			///    index++;
			/// }
			/// // java 8 implementation, not good for pararell processing
			/// int[] index = {0};    // use array for increasing index,
			/// String outStr = tokenList.stream()
			///    .map(token -> token.GetOpString(index[0]++))
			///    .collect(Collectors.joining());
			/// 
			/// </summary>
			int aInt = -1; // To mimic Java behaviour of the interlocked increment
			string outStr = String.Join(GlobalVars.LS_STR, tokenList.Select(token => token.GetOpString(Interlocked.Increment(ref aInt))));
			return outStr;
		}
		// include empty space as a token
		public static List<TokenObj> TextToTokenList(string inText) {
			List<TokenObj> tokenList = new List<TokenObj>();
			if ((!string.ReferenceEquals(inText, null)) && (inText.Length > 0)) {
				// keep token and delimiters
				string[] tokenArray = inText.Split(patternStrSpace_, true);
				tokenList = new List<TokenObj>(tokenArray.Select(token => new TokenObj(token)).ToList());
			}
			// update index and posistion
			// this could added to above to speed up if needed
			UpdateIndexPos(tokenList);
			return tokenList;
		}
		// update index and position for each TokenObj in the list
		// index: is the index of the tokenList
		// pos: is the index (ignoring empty string)
		public static void UpdateIndexPos(List<TokenObj> tokenList) {
			if ((tokenList != null) && (tokenList.Count > 0)) {
				int pos = 0;
				for (int index = 0; index < tokenList.Count; index++) {
					TokenObj tokenObj = tokenList[index];
					tokenObj.SetIndex(index);
					if (tokenObj.IsSpaceToken() == false) {
						tokenObj.SetPos(pos);
						pos++;
					}
				}
			}
		}
		// get the non-spaceToken list
		public static List<TokenObj> GetNonSpaceTokenObjList(List<TokenObj> inTokenList) {
			List<TokenObj> outTokenList = new List<TokenObj>();
			foreach (TokenObj tokenObj in inTokenList) {
				if (tokenObj.IsSpaceToken() == false) { // skip space tokens
					outTokenList.Add(tokenObj);
				}
			}
			return outTokenList;
		}
		// init
		// further decompose tokenList to token without space
		// Similar to use flatmap for toeknObj has space in the toeknStr
		// The tokenObj should only have string without space,
		// This method is needed when split happen
		private static List<TokenObj> GetAllTokens(List<TokenObj> inTokenList) {
			List<TokenObj> outTokenList = new List<TokenObj>();
			foreach (TokenObj tokenObj in inTokenList) {
				// further decompose token by string
				string tokenStr = tokenObj.GetTokenStr();
				string[] tokenArray = tokenStr.Split(patternStrSpace_, true);
				if (tokenArray.Length == 1) { // the original token
					outTokenList.Add(tokenObj);
				} else { // further decompose
					List<TokenObj> tokenList2 = new List<TokenObj>(tokenArray.Select(token => new TokenObj(token)).ToList());
					outTokenList.AddRange(tokenList2);
				}
			}
			return outTokenList;
		}
		private void Init(List<TokenObj> tokenList) {
			// update tokenList_
			tokenList_ = GetAllTokens(tokenList);
			// update text
			if (tokenList_ != null) {
				text_ = ToString();
			}
		}
		// get the tokenList from the input text by tokenizer
		private void Init(string text) {
			// update text
			text_ = text;
			// update tokenList_ from the text
			// this tokenize by spaces, tabs, and line return
			// Use look-arounds to split on empty String just 
			// before or after the delimiters (spaces) to keep the delimiters
			// Need a throught test tested
			string[] tokenArray = text_.Split(patternStrSpace_, true);
			tokenList_ = new List<TokenObj>(tokenArray.Select(token => new TokenObj(token)).ToList());
		}
		// to be deleted
		public static string[] GetTokenArray(string text) {
			string[] tokenArray = text.Split(patternStrSpace_, true);
			return tokenArray;
		}
		// to be deleted
		public static List<string> GetTokenArrayList(string text) {
			List<string> tokenArrayList = text.Split(patternStrSpace_).ToList();
			return tokenArrayList;
		}
		public static List<TokenObj> FlatTokenToArrayList(TokenObj inTokenObj) {
			string[] tokenArray = inTokenObj.GetTokenStr().Split(patternStrSpace_, true);
			List<TokenObj> tokenArrayList = new List<TokenObj>();
			foreach (string tokenStr in tokenArray) {
				TokenObj tokenObj = new TokenObj(inTokenObj);
				tokenObj.SetTokenStr(tokenStr);
				tokenArrayList.Add(tokenObj);
			}
			return tokenArrayList;
		}
		private static void Test() {
			Console.WriteLine("===== Unit Test of TextObj =====");
			// init: test double tabs and double spaces
			string inText = "Contraction:        We cant  theredve 123.45 hell.\nPlz u r good. ";
			// test case, go through each token
			TextObj textObj = new TextObj(inText);
			List<TokenObj> tokenList = textObj.GetTokenList();
			string outText = textObj.ToString();
			// print out
			Console.WriteLine("--------- TextObj( ) -----------");
			Console.WriteLine("In: [" + inText + "]");
			Console.WriteLine("Out: [" + outText + "]");
			Console.WriteLine("------ Detail -------------------");
			List<TokenObj> tokenObjList = textObj.GetTokenList();
			foreach (TokenObj tokenObj in tokenObjList) {
				Console.WriteLine("[" + tokenObj.GetTokenStr() + "]");
			}
			Console.WriteLine("===== End of Unit Test =====");
			// test more
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java TextObj");
				Environment.Exit(0);
			}

			// Unit Test
			Test();
		}
		// data member
		// tokenize including unicode spaces, keep spcae as token , use look around
		//private static String patternStrSpace_ = "(?=\\s)|(?<=\\s)";
		//private static String patternStrSpace_ = "(?=[ \\t\\n ­])|(?<=[ \\t\\n ­])";
		//public static string patternStrSpace_ = "(?=[\\s -­])|(?<=[\\s -­])";
		//public static string patternStrSpace_ = @"(?=[\s -])|(?<=[\s -])";
		public static string patternStrSpace_ = @"(?=[\s\u00A0\u00AD­])|(?<=[\s\u00A0\u00AD­])";
		private string text_ = ""; // the whole string for the text
		// the list of tokens for the text, including space, punctuation, etc.
		private List<TokenObj> tokenList_ = new List<TokenObj>();
	}

}
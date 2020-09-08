using System;
using System.Collections.Generic;
using System.Linq;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NdCorrector;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.NlpUtil {
	/// <summary>
	///***************************************************************************
	/// This class is the java object for cSpell token.
	/// A token is a single word in a text. 
	/// Spaces and tabs are used as word boundary.
	/// 
	/// <para><b>History:</b>
	/// <ul>
	/// <li>2018 baseline
	/// <li>SCR-3, chlu, 03-05-19, add word length for legit token
	/// </ul>
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ****************************************************************************
	/// </para>
	/// </summary>
	public class TokenObj {
		// public constructor
		/// <summary>
		/// Public constructor to initiate the Token Object.
		/// </summary>
		public TokenObj() { }
		public TokenObj(string tokenStr) {
			orgTokenStr_ = tokenStr;
			tokenStr_ = tokenStr;
		}
		// used in merge
		public TokenObj(string orgTokenStr, string tokenStr) {
			orgTokenStr_ = orgTokenStr;
			tokenStr_ = tokenStr;
		}
		public TokenObj(TokenObj inTokenObj) {
			index_ = inTokenObj.GetIndex(); // index of this token in the TextObj
			pos_ = inTokenObj.GetPos(); // position
			tag_ = inTokenObj.GetTag();
			orgTokenStr_ = inTokenObj.GetOrgTokenStr(); // the org str, never change
			tokenStr_ = inTokenObj.GetTokenStr(); // the current str
			// history of processes
			procHist_ = new List<string>(inTokenObj.GetProcHist());
		}
		public TokenObj(TokenObj inTokenObj, string tokenStr) {
			index_ = inTokenObj.GetIndex(); // index of this token in the TextObj
			pos_ = inTokenObj.GetPos(); // position
			tag_ = inTokenObj.GetTag();
			orgTokenStr_ = inTokenObj.GetOrgTokenStr(); // the org str, never change
			// history of processes
			procHist_ = new List<string>(inTokenObj.GetProcHist());
			tokenStr_ = tokenStr;
		}
		// public methods
		public virtual void SetIndex(int index) {
			index_ = index;
		}
		public virtual void SetPos(int pos) {
			pos_ = pos;
		}
		public virtual void SetTag(int tag) {
			tag_ = tag;
		}
		public virtual void SetTokenStr(string tokenStr) {
			tokenStr_ = tokenStr;
		}
		public virtual int GetIndex() {
			return index_;
		}
		public virtual int GetPos() {
			return pos_;
		}
		public virtual int GetTag() {
			return tag_;
		}
		public virtual string GetOrgTokenStr() {
			return orgTokenStr_;
		}
		public virtual string GetTokenStr() {
			return tokenStr_;
		}
		public virtual List<string> GetProcHist() {
			return procHist_;
		}
		public virtual void AddProcToHist(string proc) {
			procHist_.Add(proc);
		}
		// a legit token will be processed for spelling correction
		// a lgit token must not be space, or too long
		public virtual bool IsLegitToken(int maxLegitTokenLength) {
			bool flag = true;
			if ((this.IsSpaceToken() == true) || (tokenStr_.Length > maxLegitTokenLength)) {
				flag = false;
			}
			return flag;
		}

		// to be improved to all unicode space
		// check if the token is one of space str (space token)
		// TBD, change to use data member
		public virtual bool IsSpaceToken() {
			bool spaceFlag = TokenUtil.IsSpaceToken(tokenStr_);
			return spaceFlag;
		}
		// all process history
		public virtual string GetProcHistStr() {
			string outStr = PROC_START_STR + String.Join(PROC_SP_STR, procHist_) + PROC_END_STR;
			return outStr;
		}
		// orgToken|curToken|History
		public virtual string ToHistString() {
			string outStr = orgTokenStr_ + GlobalVars.FS_STR + tokenStr_ + GlobalVars.FS_STR + GetProcHistStr();
			return outStr;
		}
		// all data members
		public virtual string ToString() {
			string outStr = index_ + GlobalVars.FS_STR + pos_ + GlobalVars.FS_STR + tag_ + GlobalVars.FS_STR + orgTokenStr_ + GlobalVars.FS_STR + tokenStr_ + GlobalVars.FS_STR + GlobalVars.FS_STR + GetProcHistStr();
			return outStr;
		}
		// operation string with index
		public virtual string GetOpString(int index) {
			string outStr = index + GlobalVars.FS_STR + ToHistString();
			return outStr;
		}
		// Test Driver
		private static void Test(Dictionary<string, string> informalExpMap) {
			Console.WriteLine("===== Unit Test of TokenObj =====");
			// init
			string inText = "Contraction: We cant theredve hell. Plz u r  good.";
			List<TokenObj> inTokenList = TextObj.TextToTokenList(inText);
			// construct the outstr from tokens by joining
			List<TokenObj> outTokenList = new List<TokenObj>(inTokenList.Select(tokenObj => InformalExpHandler.Process(tokenObj, informalExpMap)).ToList());
			// result   
			string outText = TextObj.TokenListToText(outTokenList);

			// print out
			Console.WriteLine("--------- ProcInformalExpression( ) -----------");
			Console.WriteLine("In: [" + inText + "]");
			Console.WriteLine("Out: [" + outText + "]");
			Console.WriteLine("--------- detail -----------");
			foreach (TokenObj tokenObj in inTokenList) {
				Console.WriteLine(tokenObj.ToString());
			}
			Console.WriteLine("===== End of Unit Test =====");
		}
		// test driver
		public static void MainTest(string[] args) {
			string inFile = "../data/informalExpression.txt";
			if (args.Length == 1) {
				inFile = args[0];
			} else if (args.Length > 0) {
				Console.WriteLine("Usage: java TokenObj <inFile>");
				Environment.Exit(0);
			}

			// init
			Dictionary<string, string> informalExpMap = InformalExpHandler.GetInformalExpMapFromFile(inFile);
			// Unit Test
			Test(informalExpMap);
		}
		// data member
		// for History: This should be file driven
		public const string PROC_START_STR = "[";
		public const string PROC_END_STR = "]";
		public const string PROC_SP_STR = ",";
		public const int TAG_NONE = 0;
		public const int TAG_SEN_END = 1; // end of a sentence, such as . or ?
		public const int NO_INDEX = -1;
		public const int NO_POS = -1;
		public const string HIST_ND_XML_HTML = "ND_XML_HTML";
		public const string HIST_ND_INFORMAL_EXP = "ND_INFORMAL_EXP";
		public const string HIST_ND_S_E_D = "ND_SPLIT_END_DIGIT";
		public const string HIST_ND_S_E_P = "ND_SPLIT_END_PUNC";
		public const string HIST_ND_S_L_D = "ND_SPLIT_LEAD_DIGIT";
		public const string HIST_ND_S_L_P = "ND_SPLIT_LEAD_PUNC";
		public const string HIST_NW_1 = "NW_1_To_1";
		public const string HIST_NW_S = "NW_SPLIT";
		public const string HIST_NW_M = "NW_MERGE";
		public const string HIST_RW_1 = "RW_1_To_1";
		public const string HIST_RW_S = "RW_SPLIT";
		public const string HIST_RW_M = "RW_MERGE";
		public const string MERGE_START_STR = "(";
		public const string MERGE_END_STR = ")";
		// data member
		private int index_ = NO_INDEX; // index of the token in the TextObj
		private int pos_ = NO_POS; // position, index, skip space tokens
		private int tag_ = TAG_NONE; // not used yet
		private string orgTokenStr_ = ""; // the org str, never change
		private string tokenStr_ = ""; // the current str
		// history of processes
		private List<string> procHist_ = new List<string>();
	}

}
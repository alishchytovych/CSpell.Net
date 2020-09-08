using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Candidates {

	/// <summary>
	///***************************************************************************
	/// This class is the merge collection object.
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
	public class MergeObj {
		// public constructor
		/// <summary>
		/// Public constructor for MergeObj. 
		/// </summary>
		/// <param name="tarWord"> target word for merge </param>
		/// <param name="mergeWord"> merged word </param>
		/// <param name="coreMergeWord"> core term of the merged word </param>
		/// <param name="mergeNo"> total no of merge tokens </param>
		public MergeObj(string tarWord, string mergeWord, string coreMergeWord, int mergeNo) {
			tarWord_ = tarWord;
			mergeWord_ = mergeWord;
			coreMergeWord_ = coreMergeWord;
			mergeNo_ = mergeNo;
		}
		/// <summary>
		/// Public constructor for MergeObj. 
		/// </summary>
		/// <param name="tarWord"> target word for merge </param>
		/// <param name="orgMergeWord"> original word before the merge </param>
		/// <param name="mergeWord"> merged word </param>
		/// <param name="coreMergeWord"> core term of the merged word </param>
		/// <param name="mergeNo"> total no of merge tokens </param>
		/// <param name="startIndex"> index of the starting token of the merge </param>
		/// <param name="tarIndex"> index of the target token for merge </param>
		/// <param name="endIndex"> index of the ending token of the merge </param>
		/// <param name="startPos"> position (index in the no space token list) of the 
		///             starting token for the merge </param>
		/// <param name="tarPos"> positin of thetarget token for merge </param>
		/// <param name="endPos"> position of the ending token for the merge </param>
		public MergeObj(string tarWord, string orgMergeWord, string mergeWord, string coreMergeWord, int mergeNo, int startIndex, int tarIndex, int endIndex, int startPos, int tarPos, int endPos) {
			tarWord_ = tarWord;
			orgMergeWord_ = orgMergeWord;
			mergeWord_ = mergeWord;
			coreMergeWord_ = coreMergeWord;
			mergeNo_ = mergeNo;
			startIndex_ = startIndex;
			tarIndex_ = tarIndex;
			endIndex_ = endIndex;
			startPos_ = startPos;
			tarPos_ = tarPos;
			endPos_ = endPos;
		}
		// set target term
		public virtual void SetTarWord(string tarWord) {
			tarWord_ = tarWord;
		}
		public virtual void SetOrgMergeWord(string orgMergeWord) {
			orgMergeWord_ = orgMergeWord;
		}
		public virtual void SetMergeWord(string mergeWord) {
			mergeWord_ = mergeWord;
		}
		public virtual void SetCoreMergeWord(string coreMergeWord) {
			coreMergeWord_ = coreMergeWord;
		}
		public virtual void SetStartIndex(int startIndex) {
			startIndex_ = startIndex;
		}
		public virtual void SetEndIndex(int endIndex) {
			endIndex_ = endIndex;
		}
		public virtual void SetTarIndex(int tarIndex) {
			tarIndex_ = tarIndex;
		}
		public virtual void SetStartPos(int startPos) {
			startPos_ = startPos;
		}
		public virtual void SetEndPos(int endPos) {
			endPos_ = endPos;
		}
		public virtual void SetTarPos(int tarPos) {
			tarPos_ = tarPos;
		}
		public virtual string GetTarWord() {
			return tarWord_;
		}
		public virtual string GetOrgMergeWord() {
			return orgMergeWord_;
		}
		public virtual string GetMergeWord() {
			return mergeWord_;
		}
		public virtual string GetCoreMergeWord() {
			return coreMergeWord_;
		}
		public virtual int GetMergeNo() {
			return mergeNo_;
		}
		public virtual int GetStartIndex() {
			return startIndex_;
		}
		public virtual int GetEndIndex() {
			return endIndex_;
		}
		public virtual int GetTarIndex() {
			return tarIndex_;
		}
		public virtual int GetStartPos() {
			return startPos_;
		}
		public virtual int GetEndPos() {
			return endPos_;
		}
		public virtual int GetTarPos() {
			return tarPos_;
		}
		// get the simulated original term by add space tokens
		public static string GetNonMergeTerm(MergeObj mergeObj, List<TokenObj> nonSpaceTextList) {
			string nonMergeTerm = "";
			if ((mergeObj != null) && (nonSpaceTextList != null)) {
				int startPos = mergeObj.GetStartPos();
				int endPos = mergeObj.GetEndPos();
				nonMergeTerm = nonSpaceTextList[startPos].GetTokenStr();
				for (int i = startPos + 1; i <= endPos; i++) {
					if ((i >= 0) && (i < nonSpaceTextList.Count)) {
						nonMergeTerm += GlobalVars.SPACE_STR + nonSpaceTextList[i].GetTokenStr();
					} else { // illegal index
						break;
					}
				}
			}
			return nonMergeTerm;
		}
		/// <summary>
		/// This override method checks the objects sequentiqlly if hascode are the 
		/// same.
		/// </summary>
		public override bool Equals(object anObject) {
			bool flag = false;
			if ((anObject != null) && (anObject is MergeObj)) {
				if (this.ToString().Equals(((MergeObj) anObject).ToString())) {
					flag = true;
				}
			}
			return flag;
		}
		/// <summary>
		/// This override method is used in hashTable to store data as key.
		/// </summary>
		public override int GetHashCode() {
			int hashCode = this.ToString().GetHashCode();
			return hashCode;
		}
		// compose the object and converts backto String format
		// format: tarWord|orgMergeWord|mergeWord|coreMergeWord|mergeNo|startIndex|tarIndex|endIndex|startPos|tarPos|endPos
		public virtual string ToString() {
			string ourStr = tarWord_ + GlobalVars.FS_STR + orgMergeWord_ + GlobalVars.FS_STR + mergeWord_ + GlobalVars.FS_STR + coreMergeWord_ + GlobalVars.FS_STR + mergeNo_ + GlobalVars.FS_STR + startIndex_ + GlobalVars.FS_STR + tarIndex_ + GlobalVars.FS_STR + endIndex_ + GlobalVars.FS_STR + startPos_ + GlobalVars.FS_STR + tarPos_ + GlobalVars.FS_STR + endPos_;
			return ourStr;
		}
		// public methods
		// private methods
		private static void Test() {
			int tarIndex = 6; // target index
			int startIndex = 4; // start index of merge
			int endIndex = 6; // end index of merge
			int tarPos = 3; // target pos
			int startPos = 2; // start pos of merge
			int endPos = 3; // end pos of merge
			int mergeNo = 1; // total no of merged tokens
			string tarWord = "gnosed"; // target term
			string mergeWord = "diagnosed."; // suggested merged terms
			string coreMergeWord = "diagnosed"; // core suggested merged terms
			string orgMergeWord = "dia gnosed"; // org word b4 merge
			MergeObj mergeObj = new MergeObj(tarWord, orgMergeWord, mergeWord, coreMergeWord, mergeNo, startIndex, tarIndex, endIndex, startPos, tarPos, endPos);
			string inText = "He is dia gnosed last week.";
			List<TokenObj> inTextList = TextObj.TextToTokenList(inText);
			List<TokenObj> nonSpaceTextList = TextObj.GetNonSpaceTokenObjList(inTextList);
			Console.WriteLine("------ Merge Obj -------");
			Console.WriteLine(mergeObj.ToString());
			Console.WriteLine("------ Non Merge Term -------");
			string nonMergeTerm = GetNonMergeTerm(mergeObj, nonSpaceTextList);
			Console.WriteLine("- inText: [" + inText + "]");
			Console.WriteLine("- nonMergeTerm: [" + nonMergeTerm + "]");
		}
		// test driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java MergeObj");
				Environment.Exit(0);
			}

			// test case and print out 
			Test();
		}
		// data member
		// index is the index in the original text, inluding space token
		// this is needed during the merge operation when correct the original text
		private int tarIndex_ = -1; // target index
		private int startIndex_ = -1; // start index of merge
		private int endIndex_ = -1; // end index of merge
		// position is the index in the non-space token list
		// this is needed for the context scoring bz it only uses nonSpaceTokens
		private int tarPos_ = -1; // target pos
		private int startPos_ = -1; // start pos of merge
		private int endPos_ = -1; // end pos of merge
		private int mergeNo_ = 0; // total no of merged tokens
		private string tarWord_ = ""; // target word
		private string orgMergeWord_ = ""; // original b4 merged word
		private string mergeWord_ = ""; // suggested merged word
		private string coreMergeWord_ = ""; //coreTerm of sug merged word
	}

}
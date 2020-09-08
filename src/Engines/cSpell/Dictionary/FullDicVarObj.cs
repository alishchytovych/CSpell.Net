using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.Util;
namespace SpellChecker.Engines.cSpell.Dictionary {
	/// <summary>
	///***************************************************************************
	/// This class is the java object for a word (entry) in the full dictionary.
	/// Full dictioary is not used in 2018 release.
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
	public class FullDicVarObj {
		// private constructor
		public FullDicVarObj(string word) {
			word_ = word;
		}
		public FullDicVarObj(string word, long pos, long infl, string src, bool acrAbb, bool properNoun) {
			word_ = word;
			pos_ = pos;
			infl_ = infl;
			src_ = src;
			acrAbb_ = acrAbb;
			properNoun_ = properNoun;
		}

		// public methods
		public virtual string GetWord() {
			return word_;
		}
		public virtual string GetSrc() {
			return src_;
		}
		public virtual long GetPos() {
			return pos_;
		}
		public virtual long GetInfl() {
			return infl_;
		}
		public virtual bool GetAcyAbb() {
			return acrAbb_;
		}
		public virtual bool GetProperNoun() {
			return properNoun_;
		}
		/// <summary>
		/// This method returns a string of all data members of the current LexItem. 
		/// The format is:
		/// word|POS|Infl|Src|AcrAbb|ProperNoun
		/// </summary>
		/// <returns>  a string representation of full dictionary variable object </returns>
		public virtual string ToString() {
			string fs = GlobalVars.FS_STR;
			string outStr = word_ + fs + pos_ + fs + infl_ + fs + src_ + fs + acrAbb_ + fs + properNoun_ + fs;
			return outStr;
		}
		/// <summary>
		/// This override method checks the objects sequentiqlly if hascode are the 
		/// same. It is used to remove duplicate FullDicVarObj in a set. 
		/// Two FullDicVarObj are considered as the same if the String format are 
		/// the same:
		/// word|pos|infl|src|acrAbb|properNoun
		/// </summary>
		public override bool Equals(object anObject) {
			bool flag = false;
			if ((anObject != null) && (anObject is FullDicVarObj)) {
				if (this.ToString().Equals(((FullDicVarObj) anObject).ToString())) {
					flag = true;
				}
			}
			return flag;
		}
		/// <summary>
		/// This override method is used in hashTable to store data as key. It is
		/// used to removed duplicate LexItems in a set. The hasdcode of String
		/// format is used.
		/// </summary>
		/// <returns>  hash code of the detail string of LexItem </returns>
		public override int GetHashCode() {
			int hashCode = this.ToString().GetHashCode();
			return hashCode;
		}
		// data member
		public const string SRC_NONE_STR = "E0000000";
		private string word_ = ""; // word
		private string src_ = SRC_NONE_STR; // EUI
		private long pos_ = 0; // POS, category in long format
		private long infl_ = 0; // inflection in long format
		private bool acrAbb_ = false;
		private bool properNoun_ = false;
	}

}
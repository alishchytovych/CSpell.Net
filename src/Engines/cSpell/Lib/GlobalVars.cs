using System;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Lib {
	/// <summary>
	///***************************************************************************
	/// This class provides global variables used in CSpell.
	/// 
	/// <para><b>History:</b>
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ***************************************************************************
	/// </para>
	/// </summary>
	public class GlobalVars {
		// private constructor, no one can create it
		private GlobalVars() { }
		// public method
		public static GlobalVars GetInstance() {
			lock(typeof(GlobalVars)) {
				if (instance_ == null) {
					instance_ = new GlobalVars();
				}
				return instance_;
			}
		}
		public virtual void SetFieldSeparator(string value) {
			fieldSeparator_ = value;
		}
		public virtual string GetFieldSeparator() {
			return fieldSeparator_;
		}

		// data member
		/// <summary>
		/// CSPELL default separator: "|" </summary>
		public static readonly string LS_STR = System.Environment.NewLine; // line sep string
		public const string FS_STR = "|"; // field seperator string
		public const string CT_STR = "#"; // Comment string
		public const string SPACE_STR = " "; // space string
		public const string HYPHEN_STR = "-"; // hyphen string
		public const char SPACE_CHAR = ' '; // space char
		/// <summary>
		/// CSPELL version </summary>
		public const string YEAR = "2020"; // year of release
		public const string RELEASE = "2020.0.0.0"; // release version
		/// <summary>
		/// CSPELL jar string </summary>
		public static readonly string CSPELL = "cSpell" + YEAR + "dist.jar";
		private string fieldSeparator_ = FS_STR;
		// singleton instance
		private static GlobalVars instance_;
	}

}
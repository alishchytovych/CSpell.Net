	using SpellChecker.Engines.cSpell.Util;
	namespace SpellChecker.Engines.cSpell.Dictionary {

		/// <summary>
		///***************************************************************************
		/// This class is the factory of dictionary.
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
		public class DictionaryFactory {
			// private constructor
			private DictionaryFactory() { }

			// public methods
			// caseFlag is true for case sensitive dictinoary 
			// or false for case-insensitive
			public static RootDictionary GetDictionary(int dicType) {
				bool caseFlag = false; // default
				return GetDictionary(dicType, caseFlag);
			}
			public static RootDictionary GetDictionary(int dicType, bool caseFlag) {
				if (dicType == DIC_BASIC) {
					return new BasicDictionary(caseFlag);
				} else if (dicType == DIC_FULL) {
					return new FullDictionary(caseFlag);
				}
				return null;
			}
			// data member
			public const int DIC_BASIC = 0;
			public const int DIC_FULL = 1;
		}

	}
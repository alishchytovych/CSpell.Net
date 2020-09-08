using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Dictionary {

	/// <summary>
	///***************************************************************************
	/// This class is the interface for dictionary implementation.
	/// It provides most basic methods for a dictioanry.
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
	public interface RootDictionary {
		// public methods
		void AddWord(string word);
		void AddDictionary(string inFile);
		void AddDictionaries(string inFile, string rootPath);
		void AddDictionaries(string inFile, string rootPath, bool debugFlag);
		void AddDictionaries(string inFilePath, bool debugFlag);
		void AddDictionaries2(string inFile, bool debugFlag);
		bool IsDicWord(string word); // exact match
		bool IsValidWord(string word); // check X', X/Y
		int GetSize();
		HashSet<string> GetDictionarySet();
		// data member
	}

}
using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///************************************************************************** 
	/// This class ranks and finds the best candidate for real-word split by
	/// specifying different ranking method.
	/// 
	/// <para><b>History:</b>
	/// <ul>
	/// <li>2018 baseline
	/// </ul>
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ***************************************************************************
	/// </para>
	/// </summary>
	public class RankRealWordSplitByMode {
		// private constructor
		private RankRealWordSplitByMode() { }
		// public method
		// real-word ranking use context
		// tarPos: start from 0, not include empty space token
		public static string GetTopRankStr(string inStr, HashSet<string> candidates, CSpellApi cSpellApi, bool debugFlag, int tarPos, List<TokenObj> nonSpaceTokenList) {
			string topRankStr = GetTopRankStrByContext(inStr, candidates, cSpellApi, debugFlag, tarPos, nonSpaceTokenList);
			return topRankStr;
		}
		// private
		// TBD ...
		private static string GetTopRankStrByCSpell(string inStr, HashSet<string> candidates, CSpellApi cSpellApi, bool debugFlag, int tarPos, List<TokenObj> nonSpaceTokenList) {
			string topRankStr = inStr;
			return topRankStr;
		}
		private static string GetTopRankStrByContext(string inStr, HashSet<string> candidates, CSpellApi cSpellApi, bool debugFlag, int tarPos, List<TokenObj> nonSpaceTokenList) {
			// init
			Word2Vec word2VecIm = cSpellApi.GetWord2VecIm();
			Word2Vec word2VecOm = cSpellApi.GetWord2VecOm();
			//WordWcMap wordWcMap = cSpellApi.GetWordWcMap();
			int contextRadius = cSpellApi.GetRwSplitContextRadius();
			bool word2VecSkipWord = cSpellApi.GetWord2VecSkipWord();
			int maxCandNo = cSpellApi.GetCanMaxCandNo();
			int tarSize = 1; // only for split, the target size = 1
			double rwSplitCFactor = cSpellApi.GetRankRwSplitCFac();
			int shortSplitWordLength = cSpellApi.GetCanRwShortSplitWordLength();
			int maxShortSplitWordNo = cSpellApi.GetCanRwMaxShortSplitWordNo();
			// include detail print
			string topRankStr = RankRealWordSplitByContext.GetTopRankStr(inStr, candidates, tarPos, tarSize, nonSpaceTokenList, word2VecIm, word2VecOm, word2VecSkipWord, contextRadius, shortSplitWordLength, maxShortSplitWordNo, rwSplitCFactor, maxCandNo, debugFlag);
			return topRankStr;
		}

		// test Driver is implmeneted in real-word corrector
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java RankRealWordByMode");
				Environment.Exit(1);
			}
		}
		// data member
	}

}
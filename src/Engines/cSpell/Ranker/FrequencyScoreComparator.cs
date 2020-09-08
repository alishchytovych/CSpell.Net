using System.Collections.Generic;
using SpellChecker.Engines.cSpell.NlpUtil;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///***************************************************************************
	/// This class is a comparator to compare FrequencyScores.
	/// Compare the frequency score from WC, see WordCountScore.java
	/// 
	/// <para><b>History:</b>
	/// <ul>
	/// <li>2018 baseline
	/// <li>SCR-2, chlu, 03-04-19, fix comparator general contract violation error.
	/// </ul>
	/// 
	/// @author NLM NLS Development Team
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=       FrequencyScore
	/// 
	/// @version    V-2018
	/// *************************************************************************** </seealso>
	public class FrequencyScoreComparator<T> : IComparer<FrequencyScore> {
		/// <summary>
		/// Compare two object o1 and o2.  Both objects o1 and o2 are 
		/// FrequencyScore.  The compare algorithm: 
		/// </summary>
		/// <param name="o1">  first object to be compared </param>
		/// <param name="o2">  second object to be compared
		/// </param>
		/// <returns>  a negative integer, 0, or positive integer to represent the
		///          object o1 is less, equals, or greater than object 02. </returns>
		public virtual int Compare(FrequencyScore o1, FrequencyScore o2) {
			// 1. compare how many words
			// for now, we assume less word is better,
			// i.e. whatever is better than "what ever"
			int @out = 0;
			string word1 = ((FrequencyScore) o1).GetWord();
			string word2 = ((FrequencyScore) o2).GetWord();
			int wordNo1 = TermUtil.GetWordNo(word1);
			int wordNo2 = TermUtil.GetWordNo(word2);
			if (wordNo1 != wordNo2) {
				@out = wordNo1 - wordNo2; // less wordNo has higher rank
			} else { // same word no
				// 2. compare total score first
				double score1 = ((FrequencyScore) o1).GetScore();
				double score2 = ((FrequencyScore) o2).GetScore();
				// SCR-2: use a fixed number to ensure result is not 0.
				if (score2 > score1) {
					// from high to low
					@out = 1;
				} else if (score2 < score1) {
					@out = -1;
				} else { // 3. alphabetic order of word
					@out = word2.CompareTo(word1);
				}
			}
			return @out;
		}
		// data member
	}

}
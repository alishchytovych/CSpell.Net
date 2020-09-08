using System.Collections.Generic;
using SpellChecker.Engines.cSpell.NlpUtil;
namespace SpellChecker.Engines.cSpell.Ranker {
	/// <summary>
	///***************************************************************************
	/// This class is a comparator to compare ContextScores.
	/// 
	/// <para><b>History:</b>
	/// <ul>
	/// <li>2018 baseline
	/// <li>SCR-2, chlu, 03-04-19, fix comparator general contract violation error.
	/// </ul>
	/// 
	/// @author NLM NLS Development Team
	/// 
	/// @version    V-2018
	/// ***************************************************************************
	/// </para>
	/// </summary>
	public class ContextScoreComparator<T> : IComparer<ContextScore> {
		/// <summary>
		/// Compare two object o1 and o2.  Both objects o1 and o2 are 
		/// FrequencyScore.  The compare algorithm: 
		/// </summary>
		/// <param name="o1">  first object to be compared </param>
		/// <param name="o2">  second object to be compared
		/// </param>
		/// <returns>  a negative integer, 0, or positive integer to represent the
		///          object o1 is less, equals, or greater than object 02. </returns>
		public virtual int Compare(ContextScore o1, ContextScore o2) {
			int @out = 0;

			// 1. compare total score first
			double score1 = ((ContextScore) o1).GetScore();
			double score2 = ((ContextScore) o2).GetScore();
			// SCR-2: use a fixed number to ensure result is not 0.
			if (score2 > score1) {
				// from high to low
				@out = 1;
			} else if (score2 < score1) {
				@out = -1;
			} else { // 2. alphabetic order of word
				string term1 = ((ContextScore) o1).GetTerm();
				string term2 = ((ContextScore) o2).GetTerm();
				@out = term2.CompareTo(term1);
			}
			return @out;
		}
		// data member
	}

}
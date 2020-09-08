using System.Collections.Generic;

namespace SpellChecker.Engines.cSpell.Ranker {
	/// <summary>
	///***************************************************************************
	/// This class is a comparator to compare OrthographicScore records.
	/// Compare the total score, then edit distance, then phonetic, then overlap.
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
	/// <seealso cref=        OrthographicScore
	/// 
	/// @version    V-2018
	/// *************************************************************************** </seealso>
	public class OrthographicScoreComparator<T> : IComparer<OrthographicScore> {
		/// <summary>
		/// Compare two object o1 and o2.  Both objects o1 and o2 are 
		/// OrthographicScore.  The compare algorithm: 
		/// </summary>
		/// <param name="o1">  first object to be compared </param>
		/// <param name="o2">  second object to be compared
		/// </param>
		/// <returns>  a negative integer, 0, or positive integer to represent the
		///          object o1 is less, equals, or greater than object 02. </returns>
		public virtual int Compare(OrthographicScore o1, OrthographicScore o2) {
			// 1. compare total score first
			double score1 = ((OrthographicScore) o1).GetScore();
			double score2 = ((OrthographicScore) o2).GetScore();
			int @out = 0;
			// SCR-2: use a fixed number to ensure result is not 0.
			if (score2 > score1) {
				// from high to low
				//out = (int) (1000*(score2-score1)); might result 0
				@out = 1;
			} else if (score2 < score1) {
				@out = -1;
			} else {
				// 2. compare edit score
				double tokenScore1 = ((OrthographicScore) o1).GetTokenScore();
				double tokenScore2 = ((OrthographicScore) o2).GetTokenScore();
				// SCR-2: use a fixed number to ensure result is not 0.
				if (tokenScore2 > tokenScore1) {
					@out = 1;
				}
				if (tokenScore2 < tokenScore1) {
					@out = -1;
				} else {
					// 3. compare phoneticScore
					double pScore1 = ((OrthographicScore) o1).GetPhoneticScore();
					double pScore2 = ((OrthographicScore) o2).GetPhoneticScore();
					// SCR-2: use a fixed number to ensure result is not 0.
					if (pScore2 > pScore1) {
						@out = 1;
					} else if (pScore2 < pScore1) {
						@out = -1;
					} else {
						double oScore1 = ((OrthographicScore) o1).GetOverlapScore();
						double oScore2 = ((OrthographicScore) o2).GetOverlapScore();
						// 4. compare overlap Score
						// SCR-2: use a fixed number to ensure result is not 0.
						if (oScore2 > oScore1) {
							@out = 1;
						} else if (oScore2 < oScore1) {
							@out = -1;
						} else { // 5. Alphabetic order
							string str1 = ((OrthographicScore) o1).GetTarStr();
							string str2 = ((OrthographicScore) o2).GetTarStr();
							@out = str2.CompareTo(str1);
						}
					}
				}
			}
			return @out;
		}
	}

}
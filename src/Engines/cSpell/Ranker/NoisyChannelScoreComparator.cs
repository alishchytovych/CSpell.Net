using System.Collections.Generic;
using SpellChecker.Engines.cSpell.NlpUtil;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///***************************************************************************
	/// This class is a comparator to compare NoisyChannelScores.
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
	public class NoisyChannelScoreComparator<T> : IComparer<NoisyChannelScore> {
		/// <summary>
		/// Compare two object o1 and o2.  Both objects o1 and o2 are 
		/// NoisyChannelScore.  The compare algorithm: 
		/// </summary>
		/// <param name="o1">  first object to be compared </param>
		/// <param name="o2">  second object to be compared
		/// </param>
		/// <returns>  a negative integer, 0, or positive integer to represent the
		///          object o1 is less, equals, or greater than object 02. </returns>
		public virtual int Compare(NoisyChannelScore o1, NoisyChannelScore o2) {
			// 1. compare how many words for the candidates
			// for now, we assume less word is better,
			// i.e. whatever is better than "what ever"
			int @out = 0;
			string cand1 = ((NoisyChannelScore) o1).GetCandStr();
			string cand2 = ((NoisyChannelScore) o2).GetCandStr();
			int wordNo1 = TermUtil.GetWordNo(cand1);
			int wordNo2 = TermUtil.GetWordNo(cand2);
			if (wordNo1 != wordNo2) {
				@out = wordNo1 - wordNo2; // less wordNo has higher rank
			} else {
				// 2. compare noisy Channel score 
				double score1 = ((NoisyChannelScore) o1).GetScore();
				double score2 = ((NoisyChannelScore) o2).GetScore();
				// SCR-2: use a fixed number to ensure result is not 0.
				if (score2 > score1) {
					// from high to low
					@out = 1;
				} else if (score2 < score1) {
					@out = -1;
				} else {
					// 3. compare by orthographic score
					OrthographicScore oScore1 = ((NoisyChannelScore) o1).GetOScore();
					OrthographicScore oScore2 = ((NoisyChannelScore) o2).GetOScore();
					if (oScore1.GetScore() != oScore2.GetScore()) {
						OrthographicScoreComparator<OrthographicScore> osc = new OrthographicScoreComparator<OrthographicScore>();
						@out = osc.Compare(oScore1, oScore2);
					} else { // 4. hannelScore
						FrequencyScore fScore1 = ((NoisyChannelScore) o1).GetFScore();
						FrequencyScore fScore2 = ((NoisyChannelScore) o2).GetFScore();
						if (fScore1.GetScore() != fScore2.GetScore()) {
							FrequencyScoreComparator<FrequencyScore> fsc = new FrequencyScoreComparator<FrequencyScore>();
							@out = fsc.Compare(fScore1, fScore2);
						} else { // 4. alphabetic order
							@out = cand2.CompareTo(cand1);
						}
					}
				}
			}
			return @out;
		}
		// data member
	}

}
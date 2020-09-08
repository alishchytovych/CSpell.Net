using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Api;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Ranker;
using SpellChecker.Engines.cSpell.Extensions;

namespace SpellChecker.Engines.cSpell.Candidates {

	/// <summary>
	///***************************************************************************
	/// This class generates real-word 1To1 candidates. 
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
	public class RealWord1To1Candidates {
		// private constructor
		private RealWord1To1Candidates() { }
		// TBD... this is the bottle neck because so many real-words call this
		// needs to speed up
		//
		// public method
		// Get candidates from dictionary by Edit-distance:
		// 1. get all possible combinations from insert, remove, replace, switch
		//    chars. However, it does not include space (so no split).
		// 2. check if the combination is in dictionary
		public static HashSet<string> GetCandidates(string inWord, CSpellApi cSpellApi) {
			int maxLength = cSpellApi.GetCanRw1To1WordMaxLength();
			string inWordLc = inWord.ToLower();
			// 1. get it from the memoery to speed up running time
			HashSet<string> candidates = candMap_.GetValueOrNull(inWordLc);
			// 2. generate candidates on the fly, find all possibile candidates
			if (candidates == null) {
				// 2.1. get all possible candidates 
				// bottle neck for real-word: 7 min.
				HashSet<string> candidatesByEd = CandidatesUtil1To1.GetCandidatesByEd(inWord, maxLength);
				// filter out those are not valid words
				candidates = new HashSet<string>();
				// 2.2. bottle neck for real-word: 2 min.
				foreach (string candByEd in candidatesByEd) {
					// check if valid one-to-one candidate word
					if (IsValid1To1Cand(inWordLc, candByEd, cSpellApi) == true) {
						candidates.Add(candByEd);
					}
				}
				// update candMap_ and save to memory to speed up runing time 
				// TBD, need to set the maxKeyNo for candMap_ to prevent 
				// max. key size need to be <= 2**31-1 = 2,147,483,647
				// slow performance and crash could happen if too many keys 
				if (candMap_.ContainsKey(inWordLc) == false) {
					candMap_[inWordLc] = candidates;
					// warning msg< suggest value: < 1,500,000,000 for performance
					int maxHashKeySize = cSpellApi.GetCanRw1To1CandMaxKeySize();
					int hashKeySize = candMap_.Keys.Count;
					if (hashKeySize > maxHashKeySize) {
						if ((hashKeySize % 100) == 0) {
							Console.Error.WriteLine("** WARNING@RealWord1To1Candidates.GetCandidates: the size of key in RW-1To1-Cand-HashMap is too big (" + hashKeySize + " > " + maxHashKeySize + "). Please rerun the cSpell and increase the max. hash key size in the cSpell config (must < 2,147,483,647).");
						}
					}
				}
			}
			return candidates;
		}
		// real-word candidate has more restriction than non-word
		// TBD, need to organize the code ...
		// the check should be done in the ranking
		// Core process for real-word candidates
		private static bool IsValid1To1Cand(string inWord, string cand, CSpellApi cSpellApi) {
			RootDictionary suggestDic = cSpellApi.GetSuggestDic();
			Word2Vec word2VecOm = cSpellApi.GetWord2VecOm();
			WordWcMap wordWcMap = cSpellApi.GetWordWcMap();
			// real-word, check phonetic and suggDic
			// 1. check suggDic
			// 1.1 edDist <= 1
			// 1.2 edDist <= 2 && phonetic dist <= 1
			// 2. check if inflections, not a candidate real-word, not correct
			bool flag = false;
			int rw1To1CandMinWc = cSpellApi.GetCanRw1To1CandMinWc();
			int rw1To1CandMinLength = cSpellApi.GetCanRw1To1CandMinLength();
			string inWordLc = inWord.ToLower();
			int inWordLen = inWordLc.Length;
			int candLen = cand.Length;
			int lenDiff = inWordLen - candLen;
			// 1. check suggDic and inflVars
			if ((suggestDic.IsDicWord(cand) == true) && (word2VecOm.HasWordVec(cand) == true) && (candLen >= rw1To1CandMinLength) && (WordCountScore.GetWc(cand, wordWcMap) >= rw1To1CandMinWc) && (InflVarsUtil.IsInflectionVar(inWordLc, cand) == false)) // not inflVars
			{
				//&& ((lenDiff <= 1) && (lenDiff >= -1))) // length diff <= 1
				// more restriction for real-word candidates
				int pmDist = Metaphone2.GetDistance(inWordLc, cand);
				int prDist = RefinedSoundex.GetDistance(inWordLc, cand);
				int leadDist = GetLeadCharDist(inWordLc, cand);
				int endDist = GetEndCharDist(inWordLc, cand);
				int lengthDist = GetLengthDist(inWordLc, cand);
				int totalDist1 = leadDist + endDist + lengthDist + pmDist + prDist;
				int editDist = EditDistance.GetDistanceForRealWord(inWordLc, cand);
				int totalDist2 = editDist + pmDist + prDist;
				// if they sound the same
				if ((pmDist == 0) && (prDist == 0)) {
					flag = true;
				}
				// if they sound similar and orthographic is also similar
				// fixed from empierical test, not configuable
				else if ((totalDist1 < 3) && (totalDist2 < 4) && (pmDist * prDist == 0)) {
					flag = true;
				}
			}
			return flag;
		}

		private static int GetLengthDist(string str1, string str2) {
			int len1 = str1.Length;
			int len2 = str2.Length;
			int lengthDist = ((len1 >= len2) ? (len1 - len2) : (len2 - len1));
			return lengthDist;
		}
		private static int GetEndCharDist(string str1, string str2) {
			int index1 = str1.Length - 1;
			int index2 = str2.Length - 1;
			int dist = ((str1[index1] == str2[index2]) ? 0 : 1);
			return dist;
		}
		private static int GetLeadCharDist(string str1, string str2) {
			int dist = ((str1[0] == str2[0]) ? 0 : 1);
			return dist;
		}
		// 90% of real-word error have same begin characters
		private static bool HasSameBeginChar(string str1, string str2) {
			bool flag = (str1[0] == str2[0]);
			return flag;
		}
		// simplified way to check if two string are inlfectional variants
		// this method is simplied, assuming one of the input is base
		private static bool IsInflectionVarTbd(string str1, string str2) {
			bool flag = false;
			// 1. to assign base string by comparing length
			int len1 = str1.Length;
			int len2 = str2.Length;
			string baseStr = str1;
			string inflStr = str2;
			// same length, not inflectional vars, exclude irreg, such as see|saw
			if (len1 == len2) {
				return false;
			} else if (len1 > len2) { // assume the short string is the base
				baseStr = str2;
				inflStr = str1;
			}
			// check the inflections
			HashSet<string> inflSet = InflVarsUtil.GetInflVars(baseStr);
			flag = inflSet.Contains(inflStr);
			return flag;
		}
		private static void TestTpStr(string str1, string str2, CSpellApi cSpellApi) {
			HashSet<string> candSet = GetCandidates(str1.ToLower(), cSpellApi);
			bool flag = candSet.Contains(str2);
			if (flag == true) {
				totalTpNo_++;
			}
			totalTpStrNo_++;
			Console.WriteLine(flag + "|" + totalTpNo_ + "|" + totalTpStrNo_ + "|" + str1 + "|" + str2 + "|" + EditDistance.GetDistanceForRealWord(str1, str2) + "|" + RefinedSoundex.GetDistanceDetailStr(str1, str2) + "|" + Metaphone2.GetDistanceDetailStr(str1, str2, 10));
		}
		private static void TestFpStr(string str1, string str2, CSpellApi cSpellApi) {
			HashSet<string> candSet = GetCandidates(str1.ToLower(), cSpellApi);
			bool flag = candSet.Contains(str2);
			if (flag == true) {
				totalFpNo_++;
			}
			totalFpStrNo_++;
			Console.WriteLine(flag + "|" + totalFpNo_ + "|" + totalFpStrNo_ + "|" + str1 + "|" + str2 + "|" + EditDistance.GetDistanceForRealWord(str1, str2) + "|" + RefinedSoundex.GetDistanceDetailStr(str1, str2) + "|" + Metaphone2.GetDistanceDetailStr(str1, str2, 10));
		}
		private static void TestTestSet(CSpellApi cSpellApi) {
			// get candidates with dictionary
			// TP
			Console.WriteLine("====== TP examples ======");
			TestTpStr("then", "than", cSpellApi);
			TestTpStr("bowl", "bowel", cSpellApi);
			TestTpStr("effect", "affect", cSpellApi);
			TestTpStr("their", "there", cSpellApi);
			TestTpStr("weather", "whether", cSpellApi);
			TestTpStr("small", "smell", cSpellApi);
			TestTpStr("undisguised", "undiagnosed", cSpellApi);
			TestTpStr("stereotypy", "stereotypic", cSpellApi);
			TestTpStr("specially", "especially", cSpellApi);
			TestTpStr("haberman", "habermann", cSpellApi);
			TestTpStr("therefor", "therefore", cSpellApi);
			TestTpStr("pregnancy", "pregnant", cSpellApi);
			TestTpStr("anderson", "andersen", cSpellApi);
			TestTpStr("domestic", "damaged", cSpellApi);
			TestTpStr("medical", "medicine", cSpellApi);
			TestTpStr("devises", "devices", cSpellApi);
			TestTpStr("loosing", "losing", cSpellApi);
			TestTpStr("access", "excess", cSpellApi);
			TestTpStr("tiered", "tired", cSpellApi);
			TestTpStr("sever", "severe", cSpellApi);
			TestTpStr("repot", "report", cSpellApi);
			TestTpStr("thing", "think", cSpellApi);
			TestTpStr("tried", "tired", cSpellApi);
			TestTpStr("adema", "edema", cSpellApi);
			TestTpStr("lease", "least", cSpellApi);
			TestTpStr("doner", "donor", cSpellApi);
			TestTpStr("leave", "live", cSpellApi);
			TestTpStr("hank", "thank", cSpellApi);
			TestTpStr("well", "swell", cSpellApi);
			TestTpStr("dose", "does", cSpellApi);
			TestTpStr("tent", "tend", cSpellApi);
			TestTpStr("fine", "find", cSpellApi);
			TestTpStr("spot", "stop", cSpellApi);
			TestTpStr("bond", "bone", cSpellApi);
			TestTpStr("know", "now", cSpellApi);
			TestTpStr("bed", "bad", cSpellApi);
			TestTpStr("our", "are", cSpellApi);
			TestTpStr("are", "arm", cSpellApi);
			TestTpStr("gey", "get", cSpellApi);
			TestTpStr("law", "lat", cSpellApi);
			TestTpStr("off", "of", cSpellApi);
			TestTpStr("too", "to", cSpellApi);
			// possessive does not have neough data in the word2Vec, not handled 
			//TestTpStr("month's", "months", cSpellApi);
			//TestTpStr("quantity's", "quantities", cSpellApi);
			//TestTpStr("guys", "guy's", cSpellApi);
			//TestTpStr("Noonan's", "Noonan", cSpellApi);
			//TestTpStr("sisters", "sisters'", cSpellApi);
			//TestTpStr("Williams'", "Williams", cSpellApi);
			// FP
			Console.WriteLine("====== FP examples ======");
			TestFpStr("swelling", "stealing", cSpellApi);
			TestFpStr("mother", "bother", cSpellApi);
			TestFpStr("affected", "unaffected", cSpellApi);
			TestFpStr("accidental", "accident", cSpellApi);
			TestFpStr("developed", "develops", cSpellApi);
			TestFpStr("currently", "correctly", cSpellApi);
			TestFpStr("medication", "education", cSpellApi);
			TestFpStr("irritating", "irritation", cSpellApi);
			TestFpStr("generally", "general", cSpellApi);
			TestFpStr("exercises", "exercised", cSpellApi);
			TestFpStr("professionals", "professions", cSpellApi);
			TestFpStr("prediction", "reduction", cSpellApi);
			TestFpStr("publications", "duplications", cSpellApi);
			TestFpStr("show", "how", cSpellApi);
			TestFpStr("many", "any", cSpellApi);
			TestFpStr("heat", "eat", cSpellApi);
			Console.WriteLine("====== FP examples: diff.6531.txt ======");
			TestFpStr("live", "love", cSpellApi);
			TestFpStr("your", "our", cSpellApi);
			TestFpStr("from", "form", cSpellApi);
			TestFpStr("please", "place", cSpellApi);
			TestFpStr("life", "live", cSpellApi);
			TestFpStr("going", "growing", cSpellApi);
			TestFpStr("which", "watch", cSpellApi);
			TestFpStr("every", "ever", cSpellApi);
			TestFpStr("thrush", "through", cSpellApi);
			TestFpStr("please", "place", cSpellApi);
			TestFpStr("which", "watch", cSpellApi);
			TestFpStr("while", "awhile", cSpellApi);
			TestFpStr("main", "man", cSpellApi);
			TestFpStr("fear", "far", cSpellApi);
			TestFpStr("what", "wheat", cSpellApi);
			TestFpStr("order", "older", cSpellApi);
			TestFpStr("advance", "advice", cSpellApi);
			TestFpStr("thuss", "thus", cSpellApi);
			TestFpStr("fold", "food", cSpellApi);
			TestFpStr("legs", "less", cSpellApi);
			TestFpStr("contact", "contract", cSpellApi);
			TestFpStr("last", "least", cSpellApi);
			TestFpStr("donor", "done", cSpellApi);
			TestFpStr("hypertension", "hypotension", cSpellApi);
			TestFpStr("hour", "her", cSpellApi);
			TestFpStr("anyone", "alone", cSpellApi);
			TestFpStr("contact", "contract", cSpellApi);
			TestFpStr("itch", "itchy", cSpellApi);
			TestFpStr("pressure", "pleasure", cSpellApi);
			TestFpStr("consult", "consist", cSpellApi);
			TestFpStr("genetic", "generic", cSpellApi);
			TestFpStr("with", "width", cSpellApi);
			TestFpStr("make", "maze", cSpellApi);
			TestFpStr("regions", "reasons", cSpellApi);
			TestFpStr("where", "were", cSpellApi);
			TestFpStr("currently", "correctly", cSpellApi);
			TestFpStr("case", "cause", cSpellApi);
			TestFpStr("going", "growing", cSpellApi);
			TestFpStr("working", "worrying", cSpellApi);
			TestFpStr("happen", "happy", cSpellApi);
			TestFpStr("caused", "cause", cSpellApi);
			TestFpStr("support", "sport", cSpellApi);
			TestFpStr("left", "lift", cSpellApi);
			TestFpStr("joint", "join", cSpellApi);
			TestFpStr("taking", "talking", cSpellApi);
			TestFpStr("would", "world", cSpellApi);
			TestFpStr("know", "now", cSpellApi);
			TestFpStr("also", "als", cSpellApi);
			TestFpStr("advised", "advises", cSpellApi);
			TestFpStr("sides", "sites", cSpellApi);
			TestFpStr("spot", "spout", cSpellApi);
			TestFpStr("into", "onto", cSpellApi);
			TestFpStr("advance", "advice", cSpellApi);
			TestFpStr("bent", "bend", cSpellApi);
			TestFpStr("head", "had", cSpellApi);
			TestFpStr("finding", "funding", cSpellApi);
			TestFpStr("would", "world", cSpellApi);
			TestFpStr("first", "fist", cSpellApi);
			TestFpStr("service", "survive", cSpellApi);
			TestFpStr("help", "held", cSpellApi);
			TestFpStr("below", "blow", cSpellApi);
			TestFpStr("hear", "her", cSpellApi);
			TestFpStr("currently", "correctly", cSpellApi);
			TestFpStr("power", "peer", cSpellApi);
			TestFpStr("mother", "matter", cSpellApi);
			TestFpStr("since", "sense", cSpellApi);
			TestFpStr("lungs", "links", cSpellApi);
			TestFpStr("over", "offer", cSpellApi);
			TestFpStr("soon", "sun", cSpellApi);
			TestFpStr("better", "bother", cSpellApi);
			TestFpStr("know", "knee", cSpellApi);
			TestFpStr("mouth", "math", cSpellApi);
			// inflVar
			TestFpStr("passed", "passes", cSpellApi);
			TestFpStr("experienced", "experiences", cSpellApi);
			TestFpStr("exercises", "exercised", cSpellApi);
			TestFpStr("advised", "advises", cSpellApi);
			TestFpStr("taken", "takes", cSpellApi);
			TestFpStr("giving", "given", cSpellApi);
			// irreg
			TestFpStr("give", "gave", cSpellApi);
			TestFpStr("understood", "understand", cSpellApi);
			TestFpStr("worse", "worst", cSpellApi);
			TestFpStr("woman", "women", cSpellApi);
			TestFpStr("sent", "send", cSpellApi);
			// derivation
			TestFpStr("vaginal", "vagina", cSpellApi);
			TestFpStr("generally", "general", cSpellApi);
			TestFpStr("intestinal", "intestine", cSpellApi);
		}
		private static void Tests(CSpellApi cSpellApi) {
			List<string> testList = new List<string>();
			TestCand("too", "to", cSpellApi);
			TestCand("then", "than", cSpellApi);
			TestCand("thing", "think", cSpellApi);
			TestCand("sisters", "sisters'", cSpellApi);
			TestCand("know", "now", cSpellApi);
			TestCand("tried", "tired", cSpellApi);
			TestCand("specially", "especially", cSpellApi);
			TestCand("law", "lat", cSpellApi);
			TestCand("domestic", "damaged", cSpellApi);
			TestCand("Weather", "whether", cSpellApi);
			TestCand("there", "their", cSpellApi);
			TestCand("then", "than", cSpellApi);
			TestCand("fine", "find", cSpellApi);
			TestCand("bowl", "bowel", cSpellApi);
			TestCand("off", "of", cSpellApi);
			TestCand("Dies", "Does", cSpellApi);
			TestCand("descended", "undescended", cSpellApi);
			TestCand("effect", "affect", cSpellApi);
			TestCand("pregnancy", "pregnant", cSpellApi);
			TestCand("leave", "live", cSpellApi);
			TestCand("affects", "effects", cSpellApi);
			TestCand("their", "there", cSpellApi);
			TestCand("you", "your", cSpellApi);
			TestCand("medical", "medicine", cSpellApi);
			TestCand("medical", "medicine", cSpellApi);
			TestCand("swollen", "swelling", cSpellApi);
			TestCand("swollen", "swelling", cSpellApi);
			TestCand("well", "swell", cSpellApi);
			TestCand("FRIENDS", "friend's", cSpellApi);
			TestCand("access", "excess", cSpellApi);
			TestCand("where", "were", cSpellApi);
			TestCand("spot", "stop", cSpellApi);
			TestCand("weather", "whether", cSpellApi);
			TestCand("were", "we're", cSpellApi);
			TestCand("small", "smell", cSpellApi);
			TestCand("bond", "bone", cSpellApi);
			TestCand("then", "than", cSpellApi);
			TestCand("leave", "live", cSpellApi);
			TestCand("meningitidis", "meningitis", cSpellApi);
			Console.WriteLine(totalNo_ + "|" + totalCandNo_);
		}
		private static bool TestCand(string inWord, string cand, CSpellApi cSpellApi) {
			HashSet<string> candSet = GetCandidates(inWord, cSpellApi);
			bool hasCand = candSet.Contains(cand);
			totalNo_++;
			if (hasCand == true) {
				totalCandNo_++;
				Console.WriteLine(inWord + ", " + cand);
			}
			return hasCand;
		}
		private static void TestDists() {
			TestDist("small", "smell");
			TestDist("to", "too");
			TestDist("affect", "effect");
			TestDist("given", "give");
			TestDist("worst", "worse");
			TestDist("caused", "cause");
			TestDist("kriz", "chris");
		}
		private static void TestDist(string str1, string str2) {
			Console.WriteLine(str1 + "|" + str2 + "|" + GetLengthDist(str1, str2) + "|" + GetLeadCharDist(str1, str2) + "|" + GetEndCharDist(str1, str2));
		}
		// test driver
		public static void MainTest(string[] args) {

			if (args.Length > 0) {
				Console.Error.WriteLine("*** Usage: java RealWord1To1Candidates");
				Environment.Exit(1);
			}
			// init
			string configFile = "../data/Config/cSpell.properties";
			CSpellApi cSpellApi = new CSpellApi(configFile);
			//Tests(cSpellApi);
			//TestDists();
			TestTestSet(cSpellApi); // test candidate rule for TP and FP
		}
		// data member
		// key: inWordLc, value: candSet
		// this canMap is used to speed up the candidate generating process.
		private static Dictionary<string, HashSet<string>> candMap_ = new Dictionary<string, HashSet<string>>();

		// for testing purpose, should be delete
		private static int totalNo_ = 0;
		private static int totalCandNo_ = 0;
		private static int totalTpNo_ = 0;
		private static int totalTpStrNo_ = 0;
		private static int totalFpNo_ = 0;
		private static int totalFpStrNo_ = 0;
	}

}
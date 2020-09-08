using System;

namespace SpellChecker.Engines.cSpell.Ranker {
	/// <summary>
	///***************************************************************************
	/// This class provides a java class to get the distance for phonetic codes
	/// of two strings. This method is used to refine the candidate list for
	/// real-word 1To1 correction.
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
	public class PhoneticDistance {
		// private constructor
		private PhoneticDistance() { }
		// check if the phonetic distance is smaller or equals the specified value
		public static bool IsValidDistance(string srcStr, string tarStr, int maxDistance) {
			int dist = GetDistance(srcStr, tarStr);
			bool flag = (dist <= maxDistance);
			return flag;
		}
		public static int GetDistance(string srcStr, string tarStr) {
			int deleteCost = 1;
			int insertCost = 1;
			int replaceCost = 1; // penity on the replace
			int swapCost = 1;
			int caseChangeCost = 0;
			bool enhancedFlag = false;
			int splitCost = insertCost;
			int maxCodeLength = 10;
			string srcStrLc = srcStr.ToLower();
			string tarStrLc = tarStr.ToLower();
			//String srcM2 = Metaphone2.GetCode(srcStrLc, maxCodeLength);
			//String tarM2 = Metaphone2.GetCode(tarStrLc, maxCodeLength);
			string srcM2 = RefinedSoundex.GetCode(srcStrLc);
			string tarM2 = RefinedSoundex.GetCode(tarStrLc);
			int dist = EditDistance.GetEditDistance(srcM2, tarM2, deleteCost, insertCost, replaceCost, swapCost, caseChangeCost, enhancedFlag);
			return dist;
		}
		// private methods
		private static void Tests() {
			TestPhoneticDistance("two", "too");
			TestPhoneticDistance("two", "to");
			TestPhoneticDistance("their", "there");
			TestPhoneticDistance("see", "sea");
			TestPhoneticDistance("it", "to");
			TestPhoneticDistance("haberman", "habermann");
			TestPhoneticDistance("robins", "robin");
			TestPhoneticDistance("guys", "guy's");
			TestPhoneticDistance("its", "it's");
			TestPhoneticDistance("too", "to");
			TestPhoneticDistance("then", "than");
			TestPhoneticDistance("anderson", "andersen");
			TestPhoneticDistance("stereotypy", "stereotypic");
			TestPhoneticDistance("thing", "think");
			TestPhoneticDistance("multi", "multiple");
			TestPhoneticDistance("gardeners", "gardner's");
			TestPhoneticDistance("mg", "my");
			TestPhoneticDistance("if", "of");
			TestPhoneticDistance("know", "now");
			TestPhoneticDistance("tried", "tired");
			TestPhoneticDistance("canyon", "cannot");
			TestPhoneticDistance("specially", "especially");
			TestPhoneticDistance("law", "lat");
			TestPhoneticDistance("domestic", "damaged");
			TestPhoneticDistance("repot", "report");
			TestPhoneticDistance("fine", "find");
			TestPhoneticDistance("Siemens", "semen");
			TestPhoneticDistance("quantity's", "quantities");
			TestPhoneticDistance("undisguised", "undiagnosed");
			TestPhoneticDistance("bowl", "bowel");
			TestPhoneticDistance("ti", "to");
			TestPhoneticDistance("off", "of");
			TestPhoneticDistance("Dies", "does");
			TestPhoneticDistance("therefor", "therefore");
			TestPhoneticDistance("hank", "thank");
			TestPhoneticDistance("adema", "edema");
			TestPhoneticDistance("lesson", "lessen");
			TestPhoneticDistance("are", "arm");
			TestPhoneticDistance("descended", "undescended");
			TestPhoneticDistance("dose", "does");
			TestPhoneticDistance("do", "due");
			TestPhoneticDistance("pregnancy", "pregnant");
			TestPhoneticDistance("cn", "can");
			TestPhoneticDistance("leave", "live");
			TestPhoneticDistance("gastrologist", "gastroenterologist");
			TestPhoneticDistance("lounger", "lounge");
			TestPhoneticDistance("relaxers", "relaxants");
			TestPhoneticDistance("nd", "and");
			TestPhoneticDistance("wat", "what");
			TestPhoneticDistance("the", "they");
			TestPhoneticDistance("Aisa", "Asia");
			TestPhoneticDistance("affects", "effects");
			TestPhoneticDistance("you", "your");
			TestPhoneticDistance("prego", "pregnancy");
			TestPhoneticDistance("medical", "medicine");
			TestPhoneticDistance("hoe", "hole");
			TestPhoneticDistance("swollen", "swelling");
			TestPhoneticDistance("tent", "tend");
			TestPhoneticDistance("well", "swell");
			TestPhoneticDistance("lanais", "lantus");
			TestPhoneticDistance("access", "excess");
			TestPhoneticDistance("rply", "reply");
			TestPhoneticDistance("bed", "bad");
			TestPhoneticDistance("tiered", "tired");
			TestPhoneticDistance("loosing", "losing");
			TestPhoneticDistance("where", "were");
			TestPhoneticDistance("twine", "twin");
			TestPhoneticDistance("aim", "am");
			TestPhoneticDistance("least", "least");
			TestPhoneticDistance("spot", "stop");
			TestPhoneticDistance("weather", "whether");
			TestPhoneticDistance("devises", "devices");
			TestPhoneticDistance("were", "we're");
			TestPhoneticDistance("versa", "vera");
			TestPhoneticDistance("Gud", "Good");
			TestPhoneticDistance("doner", "donor");
			TestPhoneticDistance("small", "smell");
			TestPhoneticDistance("bond", "bone");
			TestPhoneticDistance("hav", "have");
			TestPhoneticDistance("do", "due");
			TestPhoneticDistance("sever", "severe");
			TestPhoneticDistance("meningitidis", "meningitis");
			TestPhoneticDistance("gey", "get");
			TestPhoneticDistance("vertebras", "vertebrae");
			TestPhoneticDistance("amd", "and");
			TestPhoneticDistance("now", "know");
			TestPhoneticDistance("ever", "every");
		}
		private static void TestPhoneticDistance(string srcStr, string tarStr) {
			int edDist = EditDistance.GetEditDistance(srcStr, tarStr, 95, 95, 100, 90, 10, false);
			int pDist = GetDistance(srcStr, tarStr);
			int maxCodeLength = 10;
			string srcStrLc = srcStr.ToLower();
			string tarStrLc = tarStr.ToLower();
			//String srcM2 = Metaphone2.GetCode(srcStrLc, maxCodeLength);
			//String tarM2 = Metaphone2.GetCode(tarStrLc, maxCodeLength);
			string srcM2 = RefinedSoundex.GetCode(srcStrLc);
			string tarM2 = RefinedSoundex.GetCode(tarStrLc);
			Console.WriteLine(srcStr + "|" + tarStr + "|" + edDist + "|" + srcM2 + "|" + tarM2 + "|" + pDist);
		}
		// test Driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java PhoneticDistance");
				Environment.Exit(0);
			}
			// test
			Tests();
		}
		// data member
	}

}
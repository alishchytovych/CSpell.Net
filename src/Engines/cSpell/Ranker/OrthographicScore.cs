using System;
using SpellChecker.Engines.cSpell.Lib;

namespace SpellChecker.Engines.cSpell.Ranker {

	/// <summary>
	///***************************************************************************
	/// This class provides a java object of orthographic score. It include scores
	/// of edit distance, phonetic and overlap.
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
	public class OrthographicScore {
		// public constructor
		/*
		public OrthographicScore(String srcStr, String tarStr)
		{
		    double wf1 = 1.00;
		    double wf2 = 0.70;
		    double wf3 = 0.80;
	
		    Init(srcStr, tarStr, wf1, wf2, wf3);
		}
		*/
		// wf1, wf2, wf3 are the weighting factors for ed-Dist, phonetic, 
		// and overlaa, respectively
		// The best emperical value is 1.00, 0.70, and 0.80, set in config file
		public OrthographicScore(string srcStr, string tarStr, double wf1, double wf2, double wf3) {
			Init(srcStr, tarStr, wf1, wf2, wf3);
		}

		public virtual string GetTarStr() {
			return tarStr_;
		}
		public virtual double GetTokenScore() {
			return tokenScore_;
		}
		public virtual double GetPhoneticScore() {
			return phoneticScore_;
		}
		public virtual double GetOverlapScore() {
			return overlapScore_;
		}
		public virtual double GetScore() {
			return score_;
		}
		// Get Token and Phonetic scores only
		public virtual double GetTpScore() {
			return tpScore_;
		}
		public virtual string ToString() {
			string outStr = ToString(GlobalVars.FS_STR);
			return outStr;
		}
		public virtual string ToString(string fieldSepStr) {
			string outStr = srcStr_ + fieldSepStr + tarStr_ + fieldSepStr + string.Format("{0,1:F8}", score_) + fieldSepStr + string.Format("{0,1:F8}", tokenScore_) + fieldSepStr + string.Format("{0,1:F8}", phoneticScore_) + fieldSepStr + string.Format("{0,1:F8}", overlapScore_);
			return outStr;
		}
		// private method
		private void Init(string srcStr, string tarStr, double wf1, double wf2, double wf3) {
			srcStr_ = srcStr;
			tarStr_ = tarStr;
			// calculate score
			tokenScore_ = TokenScore.GetScore(srcStr, tarStr);
			phoneticScore_ = PhoneticScore.GetScore(srcStr, tarStr);
			overlapScore_ = OverlapScore.GetScore(srcStr, tarStr);

			// init value is 1.0, 1.0, 1.0 in Ensemble 
			//score_ = tokenScore_ + phoneticScore_ + overlapScore_;
			// use new best value: 1.0, 0.7, 0.8
			score_ = wf1 * tokenScore_ + wf2 * phoneticScore_ + wf3 * overlapScore_;
			tpScore_ = tokenScore_ + phoneticScore_;
		}
		private static void Test(string srcStr, string tarStr) {
			double wf1 = 1.00;
			double wf2 = 0.70;
			double wf3 = 0.80;
			OrthographicScore os = new OrthographicScore(srcStr, tarStr, wf1, wf2, wf3);
			Console.WriteLine(os.ToString());
		}
		private static void Tests() {
			Test("spel", "spell");
			Test("spel", "speil");
			Test("spelld", "spell");
			Test("spelld", "spelled");
			// for merge
			Test("dicti onary", "dict unary");
			Test("dicti onary", "dictionary");
			Test("diction ary", "diction arry");
			Test("diction ary", "dictionary");
			// for real word
			Test("then", "than");
			Test("bowl", "bowel");
			Test("effect", "affect");
			Test("weather", "whether");
			Test("small", "smell");
			Test("diagnost", "diagnosed");
			Test("truely", "truly");
			Test("knoledge", "knowledge");
		}
		// test Driver
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java OrthographicScore");
				Environment.Exit(0);
			}
			// test
			Tests();
		}
		// data member
		// this is error model, the error is between srcStr and tarStr
		// use srcStr & tarStr to compare edDist, metaphone, and overlap
		private string srcStr_ = "";
		private string tarStr_ = "";
		private double tokenScore_ = 0.0;
		private double phoneticScore_ = 0.0;
		private double overlapScore_ = 0.0;
		private double score_ = 0.0; // sum of ed, phonetic, overlap_
		private double tpScore_ = 0.0; // sum of tokenScore_ and phoneticScore_
	}

}
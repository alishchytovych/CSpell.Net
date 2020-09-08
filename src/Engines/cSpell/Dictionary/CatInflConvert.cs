using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Extensions;

namespace SpellChecker.Engines.cSpell.Dictionary {
	/// <summary>
	///***************************************************************************
	/// This class converts categories and inflections from number and name.
	/// 
	/// <para><b>History:</b>
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ***************************************************************************
	/// </para>
	/// </summary>
	public class CatInflConvert {
		public static string GetCatName(string catNum) {
			return catName_.GetValueOrNull(catNum);
		}
		public static string GetInflName(string inflNum) {
			return inflName_.GetValueOrNull(inflNum);
		}
		public static string GetCatNum(string catName) {
			return catNum_.GetValueOrNull(catName);
		}
		public static string GetInflNum(string inflName) {
			return inflNum_.GetValueOrNull(inflName);
		}
		// data member
		private static Dictionary<string, string> catNum_ = new Dictionary<string, string>(12);
		private static Dictionary<string, string> catName_ = new Dictionary<string, string>(12);
		private static Dictionary<string, string> inflNum_ = new Dictionary<string, string>(25);
		private static Dictionary<string, string> inflName_ = new Dictionary<string, string>(25);
		static CatInflConvert() {
			catNum_["adj"] = "1";
			catNum_["adv"] = "2";
			catNum_["aux"] = "4";
			catNum_["compl"] = "8";
			catNum_["conj"] = "16";
			catNum_["det"] = "32";
			catNum_["modal"] = "64";
			catNum_["noun"] = "128";
			catNum_["prep"] = "256";
			catNum_["pron"] = "512";
			catNum_["verb"] = "1024";
			catName_["1"] = "adj";
			catName_["2"] = "adv";
			catName_["4"] = "aux";
			catName_["8"] = "compl";
			catName_["16"] = "conj";
			catName_["32"] = "det";
			catName_["64"] = "modal";
			catName_["128"] = "noun";
			catName_["256"] = "prep";
			catName_["512"] = "pron";
			catName_["1024"] = "verb";
			inflNum_["base"] = "1";
			inflNum_["comparative"] = "2";
			inflNum_["superlative"] = "4";
			inflNum_["plural"] = "8";
			inflNum_["presPart"] = "16";
			inflNum_["past"] = "32";
			inflNum_["pastPart"] = "64";
			inflNum_["pres3s"] = "128";
			inflNum_["positive"] = "256";
			inflNum_["singular"] = "512";
			inflNum_["infinitive"] = "1024";
			inflNum_["pres123p"] = "2048";
			inflNum_["pastNeg"] = "4096";
			inflNum_["pres123pNeg"] = "8192";
			inflNum_["pres1s"] = "16384";
			inflNum_["past1p23pNeg"] = "32768";
			inflNum_["past1p23p"] = "65536";
			inflNum_["past1s3sNeg"] = "131072";
			inflNum_["pres1p23p"] = "262144";
			inflNum_["pres1p23pNeg"] = "524288";
			inflNum_["past1s3s"] = "1048576";
			inflNum_["pres"] = "2097152";
			inflNum_["pres3sNeg"] = "4194304";
			inflNum_["presNeg"] = "8388608";
			inflName_["1"] = "base";
			inflName_["2"] = "comparative";
			inflName_["4"] = "superlative";
			inflName_["8"] = "plural";
			inflName_["16"] = "presPart";
			inflName_["32"] = "past";
			inflName_["64"] = "pastPart";
			inflName_["128"] = "pres3s";
			inflName_["256"] = "positive";
			inflName_["512"] = "singular";
			inflName_["1024"] = "infinitive";
			inflName_["2048"] = "pres123p";
			inflName_["4096"] = "pastNeg";
			inflName_["8192"] = "pres123pNeg";
			inflName_["16384"] = "pres1s";
			inflName_["32768"] = "past1p23pNeg";
			inflName_["65536"] = "past1p23p";
			inflName_["131072"] = "past1s3sNeg";
			inflName_["262144"] = "pres1p23p";
			inflName_["524288"] = "pres1p23pNeg";
			inflName_["1048576"] = "past1s3s";
			inflName_["2097152"] = "pres";
			inflName_["4194304"] = "pres3sNeg";
			inflName_["8388608"] = "presNeg";
		}
	}

}
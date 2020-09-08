using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SpellChecker.Engines.cSpell.Extensions;

namespace SpellChecker.Engines.cSpell.Lib {
	/// <summary>
	///***************************************************************************
	/// This class is the configuration object that is used to store and retrieve 
	/// configurable varaibles from the configuration file. 
	/// 
	/// <para><b>History:</b>
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ***************************************************************************
	/// </para>
	/// </summary>
	public class Configuration {
		// public constructor
		/// <summary>
		/// Create a Configuration object.  There are two ways of reading 
		/// configuration files.  First, finding xxx.properties from Java class path.
		/// Second, finding file by a specified path. 
		/// </summary>
		/// <param name="fName">  the path of the configuration file or base name when
		/// using classpath. </param>
		/// <param name="useClassPath">  a boolean flag of finding configuration file 
		/// from classpath </param>
		public Configuration(string fName, bool useClassPath) {
			SetConfiguration(fName, useClassPath);
		}
		// public methods
		/// <summary>
		/// Get the size of key of config hashtable.
		/// </summary>
		/// <returns>  the size of configuration item (keys) </returns>
		public virtual int GetSize() {
			int size = 0;
			if (config_ != null) {
				size = config_.Count;
			}
			return size;
		}
		/// <summary>
		/// Get a value from configuration file by specifying the key.
		/// </summary>
		/// <param name="key">  key (name) of the configuration value to be get
		/// </param>
		/// <returns>  the value of the configuration item in a string format </returns>
		public virtual string GetProperty(string key) {
			string @out = config_.GetValueOrNull(key);
			return @out;
		}
		/// <summary>
		/// Overwrite the value if it is specified in the properties.
		/// </summary>
		/// <param name="properties">  properties to be overwrite in the configuration </param>
		public virtual void OverwriteProperties(Dictionary<string, string> properties) {
			for (IEnumerator<string> e = properties.Keys.GetEnumerator(); e.MoveNext();) {
				string key = e.Current;
				string value = properties.GetValueOrNull(key);
				config_[key] = value;
			}
		}
		/// <summary>
		/// Get system level information from configuration.  This includes
		/// CS_DIR 
		/// </summary>
		/// <returns>  the value of the configuration item in a string format </returns>
		public virtual string GetInformation() {
			StringBuilder buffer = new StringBuilder();
			buffer.Append("========== Files/Dirctory Setup ==========");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_DIR: [" + GetProperty(CS_DIR) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_CHECK_DIC_FILES: [" + GetProperty(CS_CHECK_DIC_FILES) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_SUGGEST_DIC_FILES: [" + GetProperty(CS_SUGGEST_DIC_FILES) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_SPLIT_WORD_DIC_FILES: [" + GetProperty(CS_SPLIT_WORD_DIC_FILES) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_MW_DIC_FILE: [" + GetProperty(CS_MW_DIC_FILE) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_PN_DIC_FILE: [" + GetProperty(CS_PN_DIC_FILE) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_AA_DIC_FILE: [" + GetProperty(CS_AA_DIC_FILE) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_SV_DIC_FILE: [" + GetProperty(CS_SV_DIC_FILE) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_UNIT_DIC_FILE: [" + GetProperty(CS_UNIT_DIC_FILE) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_FREQUENCY_FILE: [" + GetProperty(CS_FREQUENCY_FILE) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_W2V_IM_FILE: [" + GetProperty(CS_W2V_IM_FILE) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_W2V_OM_FILE: [" + GetProperty(CS_W2V_OM_FILE) + "]");
			buffer.Append(GlobalVars.LS_STR);

			buffer.Append("========== CSpell Mode ==========");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_FUNC_MODE: [" + GetProperty(CS_FUNC_MODE) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_RANK_MODE: [" + GetProperty(CS_RANK_MODE) + "]");
			buffer.Append(GlobalVars.LS_STR);

			buffer.Append("========== Detectors Setup ==========");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_MAX_LEGIT_TOKEN_LENGTH: [" + GetProperty(CS_MAX_LEGIT_TOKEN_LENGTH) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_DETECTOR_RW_SPLIT_WORD_MIN_LENGTH: [" + GetProperty(CS_DETECTOR_RW_SPLIT_WORD_MIN_LENGTH) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_DETECTOR_RW_SPLIT_WORD_MIN_WC: [" + GetProperty(CS_DETECTOR_RW_SPLIT_WORD_MIN_WC) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_DETECTOR_RW_SPLIT_1TO1_MIN_LENGTH: [" + GetProperty(CS_DETECTOR_RW_1TO1_WORD_MIN_LENGTH) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_DETECTOR_RW_1TO1_WORD_MIN_WC: [" + GetProperty(CS_DETECTOR_RW_1TO1_WORD_MIN_WC) + "]");
			buffer.Append(GlobalVars.LS_STR);

			buffer.Append("========== Candidates Setup ==========");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_CAN_ND_MAX_SPLIT_NO: [" + GetProperty(CS_CAN_ND_MAX_SPLIT_NO) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_CAN_NW_1TO1_WORD_MAX_LENGTH: [" + GetProperty(CS_CAN_NW_1TO1_WORD_MAX_LENGTH) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_CAN_NW_MAX_SPLIT_NO: [" + GetProperty(CS_CAN_NW_MAX_SPLIT_NO) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_CAN_NW_MAX_MERGE_NO: [" + GetProperty(CS_CAN_NW_MAX_MERGE_NO) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_CAN_NW_MERGE_WITH_HYPHEN: [" + GetProperty(CS_CAN_NW_MERGE_WITH_HYPHEN) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_CAN_RW_1TO1_WORD_MAX_LENGTH: [" + GetProperty(CS_CAN_RW_1TO1_WORD_MAX_LENGTH) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_CAN_RW_MAX_SPLIT_NO: [" + GetProperty(CS_CAN_RW_MAX_SPLIT_NO) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_CAN_RW_MAX_MERGE_NO: [" + GetProperty(CS_CAN_RW_MAX_MERGE_NO) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_CAN_RW_MERGE_WITH_HYPHEN: [" + GetProperty(CS_CAN_RW_MERGE_WITH_HYPHEN) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_CAN_RW_SHORT_SPLIT_WORD_LENGTH: [" + GetProperty(CS_CAN_RW_SHORT_SPLIT_WORD_LENGTH) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_CAN_RW_MAX_SHORT_SPLIT_WORD_NO: [" + GetProperty(CS_CAN_RW_MAX_SHORT_SPLIT_WORD_NO) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_CAN_MAX_CANDIDATE_NO: [" + GetProperty(CS_CAN_MAX_CANDIDATE_NO) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_CAN_RW_MERGE_CAND_MIN_WC: [" + GetProperty(CS_CAN_RW_MERGE_CAND_MIN_WC) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_CAN_RW_SPLIT_CAND_MIN_WC: [" + GetProperty(CS_CAN_RW_SPLIT_CAND_MIN_WC) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_CAN_RW_1TO1_CAND_MIN_LENGTH: [" + GetProperty(CS_CAN_RW_1TO1_CAND_MIN_LENGTH) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_CAN_RW_1TO1_CAND_MAX_KEY_SIZE: [" + GetProperty(CS_CAN_RW_1TO1_CAND_MAX_KEY_SIZE) + "]");
			buffer.Append(GlobalVars.LS_STR);

			buffer.Append("========== Rankers Setup ==========");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_RANKER_NW_S1_RANK_RANGE_FAC: [" + GetProperty(CS_RANKER_NW_S1_RANK_RANGE_FAC) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_RANKER_NW_S1_MIN_OSCORE: [" + GetProperty(CS_RANKER_NW_S1_MIN_OSCORE) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_RANKER_RW_1TO1_C_FAC: [" + GetProperty(CS_RANKER_RW_1TO1_C_FAC) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_RANKER_RW_SPLIT_C_FAC: [" + GetProperty(CS_RANKER_RW_SPLIT_C_FAC) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_RANKER_RW_MERGE_C_FAC: [" + GetProperty(CS_RANKER_RW_MERGE_C_FAC) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_RANKER_RW_1TO1_WORD_MIN_CS: [" + GetProperty(CS_RANKER_RW_1TO1_WORD_MIN_CS) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_RANKER_RW_1TO1_CAND_CS_FAC: [" + GetProperty(CS_RANKER_RW_1TO1_CAND_CS_FAC) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_RANKER_RW_1TO1_CAND_MIN_CS: [" + GetProperty(CS_RANKER_RW_1TO1_CAND_MIN_CS) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_RANKER_RW_1TO1_CAND_CS_DIST: [" + GetProperty(CS_RANKER_RW_1TO1_CAND_CS_DIST) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_RANKER_RW_1TO1_CAND_FS_FAC: [" + GetProperty(CS_RANKER_RW_1TO1_CAND_FS_FAC) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_RANKER_RW_1TO1_CAND_MIN_FS: [" + GetProperty(CS_RANKER_RW_1TO1_CAND_MIN_FS) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_RANKER_RW_1TO1_CAND_FS_DIST: [" + GetProperty(CS_RANKER_RW_1TO1_CAND_FS_DIST) + "]");
			buffer.Append(GlobalVars.LS_STR);

			// Score
			buffer.Append("========== Score Setup ==========");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_ORTHO_SCORE_ED_DIST_FAC: [" + GetProperty(CS_ORTHO_SCORE_ED_DIST_FAC) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_ORTHO_SCORE_PHONETIC_FAC: [" + GetProperty(CS_ORTHO_SCORE_PHONETIC_FAC) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_ORTHO_SCORE_OVERLAP_FAC: [" + GetProperty(CS_ORTHO_SCORE_OVERLAP_FAC) + "]");
			buffer.Append(GlobalVars.LS_STR);

			// Context
			buffer.Append("========== Context Setup ==========");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_W2V_SKIP_WORD: [" + GetProperty(CS_W2V_SKIP_WORD) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_NW_1TO1_CONTEXT_RADIUS: [" + GetProperty(CS_NW_1TO1_CONTEXT_RADIUS) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_NW_SPLIT_CONTEXT_RADIUS: [" + GetProperty(CS_NW_SPLIT_CONTEXT_RADIUS) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_NW_MERGE_CONTEXT_RADIUS: [" + GetProperty(CS_NW_MERGE_CONTEXT_RADIUS) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_RW_1TO1_CONTEXT_RADIUS: [" + GetProperty(CS_RW_1TO1_CONTEXT_RADIUS) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_RW_SPLIT_CONTEXT_RADIUS: [" + GetProperty(CS_RW_SPLIT_CONTEXT_RADIUS) + "]");
			buffer.Append(GlobalVars.LS_STR);
			buffer.Append("CS_RW_MERGE_CONTEXT_RADIUS: [" + GetProperty(CS_RW_MERGE_CONTEXT_RADIUS) + "]");
			return buffer.ToString();
		}
		// private methods
		private void SetConfiguration(string fName, bool useClassPath) {
			String[] lines = null;
			try {
				// get config data from fName.properties in class path
				lines = File.ReadAllLines(fName, Encoding.UTF8);
			} catch (Exception e) {
				Console.Error.WriteLine("** Configuration Error: " + e.Message);
				Console.Error.WriteLine("** Error: problem of opening/reading config file: '" + fName + "'. Use -x option to specify the config file path.");
			}
			// put properties from configSrc_ into config_
			if (lines != null) {
				foreach (var item in lines) {
					var element = item.Trim();
					if (element.ElementAt(0) == '#') continue;
					try {
						var pair = element.Split('=');
						string key = pair[0].Trim();
						string value = pair[1].Trim();
						config_[key] = value;
					} catch {
						Console.Error.WriteLine($"Incorrect config line: {item}");
					}
				}
			}
			// reset TOP_DIR
			string cSpellDir = GetProperty(CS_DIR);
			if ((!string.ReferenceEquals(cSpellDir, null)) && (cSpellDir.Equals(CS_AUTO_MODE) == true)) {
				config_[CS_DIR] = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			}
		}
		// data member
		public const string CS_AUTO_MODE = "CS_AUTO_MODE";

		/// <summary>
		/// key for the path of CSpell directory defined in configuration file </summary>
		public const string CS_DIR = "CS_DIR";
		/// <summary>
		/// key for the path of informal expression file in configuration file </summary>
		public const string CS_INFORMAL_EXP_FILE = "CS_INFORMAL_EXP_FILE";
		/// <summary>
		/// key for the path of check dictionary file in configuration file </summary>
		public const string CS_CHECK_DIC_FILES = "CS_CHECK_DIC_FILES";
		/// <summary>
		/// key for the path of suggestion dictionary file in configuration file </summary>
		public const string CS_SUGGEST_DIC_FILES = "CS_SUGGEST_DIC_FILES";
		/// <summary>
		/// key for the path of split word dictionary file in configuration file </summary>
		public const string CS_SPLIT_WORD_DIC_FILES = "CS_SPLIT_WORD_DIC_FILES";
		/// <summary>
		/// key for the path of multiwords dictionary file in configuration file </summary>
		public const string CS_MW_DIC_FILE = "CS_MW_DIC_FILE";
		/// <summary>
		/// key for the path of properNoun dictionary file in configuration file </summary>
		public const string CS_PN_DIC_FILE = "CS_PN_DIC_FILE";
		/// <summary>
		/// key for the path of Abb/Acr dictionary file in configuration file </summary>
		public const string CS_AA_DIC_FILE = "CS_AA_DIC_FILE";
		/// <summary>
		/// key for the path of spVar dictionary file in configuration file </summary>
		public const string CS_SV_DIC_FILE = "CS_SV_DIC_FILE";
		/// <summary>
		/// key for the path of unit file in configuration file </summary>
		public const string CS_UNIT_DIC_FILE = "CS_UNIT_DIC_FILE";
		/// <summary>
		/// key for the path of frequency file in configuration file </summary>
		public const string CS_FREQUENCY_FILE = "CS_FREQUENCY_FILE";
		/// <summary>
		/// key for the path of word2Vec files in configuration file </summary>
		// input matrix between inputs and hidden layer in word2Vec
		public const string CS_W2V_IM_FILE = "CS_W2V_IM_FILE";
		// output matrix between hidden layer and output in word2Vec
		public const string CS_W2V_OM_FILE = "CS_W2V_OM_FILE";

		// function mode
		public const string CS_FUNC_MODE = "CS_FUNC_MODE";
		// NW, 1To1 and Split rank mode
		public const string CS_RANK_MODE = "CS_RANK_MODE";

		// detector
		/// <summary>
		/// key for the max. legit token length </summary>
		public const string CS_MAX_LEGIT_TOKEN_LENGTH = "CS_MAX_LEGIT_TOKEN_LENGTH";
		/// <summary>
		/// key for the min split word length </summary>
		public const string CS_DETECTOR_RW_SPLIT_WORD_MIN_LENGTH = "CS_DETECTOR_RW_SPLIT_WORD_MIN_LENGTH";
		/// <summary>
		/// key for the min split word word count </summary>
		public const string CS_DETECTOR_RW_SPLIT_WORD_MIN_WC = "CS_DETECTOR_RW_SPLIT_WORD_MIN_WC";
		/// <summary>
		/// key for the min 1-to-1 word length </summary>
		public const string CS_DETECTOR_RW_1TO1_WORD_MIN_LENGTH = "CS_DETECTOR_RW_1TO1_WORD_MIN_LENGTH";
		/// <summary>
		/// key for the min 1-to-1 word word count </summary>
		public const string CS_DETECTOR_RW_1TO1_WORD_MIN_WC = "CS_DETECTOR_RW_1TO1_WORD_MIN_WC";

		// Candidates    
		/// <summary>
		/// key for the max candidates no </summary>
		public const string CS_CAN_MAX_CANDIDATE_NO = "CS_CAN_MAX_CANDIDATE_NO";
		/// <summary>
		/// key for the max recursive split  no for ND splitter </summary>
		public const string CS_CAN_ND_MAX_SPLIT_NO = "CS_CAN_ND_MAX_SPLIT_NO";
		/// <summary>
		/// key for the max length of word for non-word 1To1 </summary>
		public const string CS_CAN_NW_1TO1_WORD_MAX_LENGTH = "CS_CAN_NW_1TO1_WORD_MAX_LENGTH";
		/// <summary>
		/// key for the max space split no for non-word split </summary>
		public const string CS_CAN_NW_MAX_SPLIT_NO = "CS_CAN_NW_MAX_SPLIT_NO";
		/// <summary>
		/// key for the max space merge no for non-word merge </summary>
		public const string CS_CAN_NW_MAX_MERGE_NO = "CS_CAN_NW_MAX_MERGE_NO";
		/// <summary>
		/// key for the non-word merge with hyphen </summary>
		public const string CS_CAN_NW_MERGE_WITH_HYPHEN = "CS_CAN_NW_MERGE_WITH_HYPHEN";
		/// <summary>
		/// key for the max length of word for real-word 1To1 </summary>
		public const string CS_CAN_RW_1TO1_WORD_MAX_LENGTH = "CS_CAN_RW_1TO1_WORD_MAX_LENGTH";
		/// <summary>
		/// key for the max space split no for real-word split </summary>
		public const string CS_CAN_RW_MAX_SPLIT_NO = "CS_CAN_RW_MAX_SPLIT_NO";
		/// <summary>
		/// key for the max space merge no for real-word merge </summary>
		public const string CS_CAN_RW_MAX_MERGE_NO = "CS_CAN_RW_MAX_MERGE_NO";
		/// <summary>
		/// key for the real-word merge with hyphen </summary>
		public const string CS_CAN_RW_MERGE_WITH_HYPHEN = "CS_CAN_RW_MERGE_WITH_HYPHEN";
		/// <summary>
		/// key for the rw short split word length </summary>
		public const string CS_CAN_RW_SHORT_SPLIT_WORD_LENGTH = "CS_CAN_RW_SHORT_SPLIT_WORD_LENGTH";
		/// <summary>
		/// key for the max no of rw short split word </summary>
		public const string CS_CAN_RW_MAX_SHORT_SPLIT_WORD_NO = "CS_CAN_RW_MAX_SHORT_SPLIT_WORD_NO";
		/// <summary>
		/// key for the min wc of rw merge candidate </summary>
		public const string CS_CAN_RW_MERGE_CAND_MIN_WC = "CS_CAN_RW_MERGE_CAND_MIN_WC";
		/// <summary>
		/// key for the min wc of rw split candidate </summary>
		public const string CS_CAN_RW_SPLIT_CAND_MIN_WC = "CS_CAN_RW_SPLIT_CAND_MIN_WC";
		/// <summary>
		/// key for the min wc of rw 1-to-1 candidate </summary>
		public const string CS_CAN_RW_1TO1_CAND_MIN_LENGTH = "CS_CAN_RW_1TO1_CAND_MIN_LENGTH";
		/// <summary>
		/// key for the min wc of rw 1-to-1 candidate </summary>
		public const string CS_CAN_RW_1TO1_CAND_MIN_WC = "CS_CAN_RW_1TO1_CAND_MIN_WC";
		/// <summary>
		/// key for the max key size of rw 1-to-1 candidate </summary>
		public const string CS_CAN_RW_1TO1_CAND_MAX_KEY_SIZE = "CS_CAN_RW_1TO1_CAND_MAX_KEY_SIZE";

		// rankers
		/// <summary>
		/// key for the non-word Split and 1-to-1 rank range factor </summary>
		public const string CS_RANKER_NW_S1_RANK_RANGE_FAC = "CS_RANKER_NW_S1_RANK_RANGE_FAC";
		/// <summary>
		/// key for the non-word Split and 1-to-1 min oScore </summary>
		public const string CS_RANKER_NW_S1_MIN_OSCORE = "CS_RANKER_NW_S1_MIN_OSCORE";
		/// <summary>
		/// key for the real-word 1-to-1 context score confidence factor </summary>
		public const string CS_RANKER_RW_1TO1_C_FAC = "CS_RANKER_RW_1TO1_C_FAC";
		/// <summary>
		/// key for the real-word merge context score confidence factor </summary>
		public const string CS_RANKER_RW_SPLIT_C_FAC = "CS_RANKER_RW_SPLIT_C_FAC";
		/// <summary>
		/// key for the real-word merge context score confidence factor </summary>
		public const string CS_RANKER_RW_MERGE_C_FAC = "CS_RANKER_RW_MERGE_C_FAC";
		/// <summary>
		/// key for the real-word 1to1 word min context score </summary>
		public const string CS_RANKER_RW_1TO1_WORD_MIN_CS = "CS_RANKER_RW_1TO1_WORD_MIN_CS";
		/// <summary>
		/// key for the real-word 1to1 cand context score factor </summary>
		public const string CS_RANKER_RW_1TO1_CAND_CS_FAC = "CS_RANKER_RW_1TO1_CAND_CS_FAC";
		/// <summary>
		/// key for the real-word 1to1 word min context score </summary>
		public const string CS_RANKER_RW_1TO1_CAND_MIN_CS = "CS_RANKER_RW_1TO1_CAND_MIN_CS";
		/// <summary>
		/// key for the real-word 1to1 cand context score dist </summary>
		public const string CS_RANKER_RW_1TO1_CAND_CS_DIST = "CS_RANKER_RW_1TO1_CAND_CS_DIST";
		/// <summary>
		/// key for the real-word 1to1 cand frequency score factor </summary>
		public const string CS_RANKER_RW_1TO1_CAND_FS_FAC = "CS_RANKER_RW_1TO1_CAND_FS_FAC";
		/// <summary>
		/// key for the real-word 1to1 cand min frequency score </summary>
		public const string CS_RANKER_RW_1TO1_CAND_MIN_FS = "CS_RANKER_RW_1TO1_CAND_MIN_FS";
		/// <summary>
		/// key for the real-word 1to1 cand frequency score dist </summary>
		public const string CS_RANKER_RW_1TO1_CAND_FS_DIST = "CS_RANKER_RW_1TO1_CAND_FS_DIST";

		// score
		/// <summary>
		/// key for orthographic score weight factor of edit-distance score </summary>
		public const string CS_ORTHO_SCORE_ED_DIST_FAC = "CS_ORTHO_SCORE_ED_DIST_FAC";
		/// <summary>
		/// key for orthographic score weight factor of phonetic score </summary>
		public const string CS_ORTHO_SCORE_PHONETIC_FAC = "CS_ORTHO_SCORE_PHONETIC_FAC";
		/// <summary>
		/// key for orthographic score weight factor of overlap score </summary>
		public const string CS_ORTHO_SCORE_OVERLAP_FAC = "CS_ORTHO_SCORE_OVERLAP_FAC";

		// context
		// boolean flag for skip word in the context that does not have word2vec
		public const string CS_W2V_SKIP_WORD = "CS_W2V_SKIP_WORD";
		/// <summary>
		/// key for the nw 1-to-1 context radius, window size = 2*contextRadius+1 </summary>
		public const string CS_NW_1TO1_CONTEXT_RADIUS = "CS_NW_1TO1_CONTEXT_RADIUS";
		/// <summary>
		/// key for the nw split context radius </summary>
		public const string CS_NW_SPLIT_CONTEXT_RADIUS = "CS_NW_SPLIT_CONTEXT_RADIUS";
		/// <summary>
		/// key for the nw merge context radius </summary>
		public const string CS_NW_MERGE_CONTEXT_RADIUS = "CS_NW_MERGE_CONTEXT_RADIUS";
		/// <summary>
		/// key for the rw 1-to-1 context radius </summary>
		public const string CS_RW_1TO1_CONTEXT_RADIUS = "CS_RW_1TO1_CONTEXT_RADIUS";
		/// <summary>
		/// key for the rw split context radius </summary>
		public const string CS_RW_SPLIT_CONTEXT_RADIUS = "CS_RW_SPLIT_CONTEXT_RADIUS";
		/// <summary>
		/// key for the rw merge context radius </summary>
		public const string CS_RW_MERGE_CONTEXT_RADIUS = "CS_RW_MERGE_CONTEXT_RADIUS";

		// private data member
		private Dictionary<string, string> config_ = new Dictionary<string, string>(); // the real config vars
	}

}
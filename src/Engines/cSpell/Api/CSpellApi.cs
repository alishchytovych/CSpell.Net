using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpellChecker.Engines.cSpell.Dictionary;
using SpellChecker.Engines.cSpell.Lib;
using SpellChecker.Engines.cSpell.NdCorrector;
using SpellChecker.Engines.cSpell.NlpUtil;
using SpellChecker.Engines.cSpell.Ranker;
using SpellChecker.Engines.cSpell.Util;

namespace SpellChecker.Engines.cSpell.Api {
	/// <summary>
	///***************************************************************************
	/// This class is API of CSpell. It is the only class needed for end users.
	/// 
	/// <para><b>History:</b>
	/// <ul>
	/// <li>2018 baseline
	/// </ul>
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ****************************************************************************
	/// </para>
	/// </summary>
	public class CSpellApi : ISpellChecker {

		private readonly ILogger<CSpellApi> _logger;
		private readonly IOptions<SpellCheckerConfiguration> _config;

		public CSpellApi(ILogger<CSpellApi> logger, IOptions<SpellCheckerConfiguration> config) {
			_logger = logger;
			_config = config;
			Init2(false);
		}

		public void Recover() {
			this.Close();
			Init2(false);
		}

		// public constructor
		/// <summary>
		/// Public constructor for CSpellApi.
		/// </summary>
		public CSpellApi() {
			Init();
		}

		public CSpellApi(SpellCheckerConfiguration config, bool debugFlag = false) {
			Init();
		}
		/// <summary>
		/// CSpellApi constructor, initiate related data using a specified
		/// configuration file.
		/// </summary>
		/// <param name="configFile">   the absolute path of the configuration file </param>
		public CSpellApi(string configFile) {
			configFile_ = configFile;
			Init();
		}
		/// <summary>
		/// CSpellApi constructor, initiate related data using a specified
		/// configuration file.
		/// </summary>
		/// <param name="configFile">  the absolute path of the configuration file </param>
		/// <param name="debugFlag">   boolean flag for debug print </param>
		public CSpellApi(string configFile, bool debugFlag) {
			configFile_ = configFile;
			Init(debugFlag);
		}
		/// <summary>
		/// CSpellApi constructor, initiate related data with properties
		/// needs to be overwritten.
		/// </summary>
		/// <param name="properties">   properties to be overwritten in config </param>
		public CSpellApi(Dictionary<string, string> properties) {
			properties_ = properties;
			Init();
		}
		/// <summary>
		/// CSpellApi constructor, initiate related data with properties
		/// needs to be overwritten.
		/// </summary>
		/// <param name="configFile">   the absolute path of the configuration file </param>
		/// <param name="properties">   properties to be overwritten in config </param>
		public CSpellApi(string configFile, Dictionary<string, string> properties) {
			configFile_ = configFile;
			properties_ = properties;
			Init();
		}
		/// <summary>
		/// Get the configuration object.
		/// </summary>
		/// <returns>  Configuration object </returns>
		public virtual Configuration GetConfiguration() {
			return conf_;
		}

		public string Correct(string input) {
			_logger.LogTrace($"Correction called");
			string outStr = CorrectionApi.ProcessToStr(input, this);
			return outStr;
		}

		public (string, List<TokenObj>) CorrectExt(string input) {
			_logger.LogTrace($"Correction called");
			return CorrectionApi.ProcessToStrExt(input, this, false);
		}


		// cSpell main process: input str, output Str
		/// <summary>
		/// cSpell correction process, output to a string. Use funcmode and rankmode
		/// specified in the configuration file.
		/// </summary>
		/// <param name="inText">   input text to be corrected </param>
		/// <returns>  corrected text </returns>
		public virtual string ProcessToStr(string inText) {
			// use modes from configuration
			string outStr = CorrectionApi.ProcessToStr(inText, this);
			return outStr;
		}
		/// <summary>
		/// cSpell correction process, output to a string by 
		/// specifying funcMode and rankMode.
		/// </summary>
		/// <param name="inText">   input text to be corrected </param>
		/// <param name="funcMode"> funcMode for correction: NW/RW-Merge/Split/1To1 </param>
		/// <param name="rankMode"> rankMode for select correction from the candidate for NW
		///                     Split/1To1 </param>
		/// <returns>  corrected text </returns>
		public virtual string ProcessToStr(string inText, int funcMode, int rankMode) {
			// update modes
			this.funcMode_ = funcMode;
			this.rankMode_ = rankMode;
			string outStr = CorrectionApi.ProcessToStr(inText, this);
			return outStr;
		}
		/// <summary>
		/// cSpell correction process, output to an ArrayList of TokenObj by using 
		/// funcMode and rankMode from configuratin file.
		/// </summary>
		/// <param name="inText">   input text to be corrected </param>
		/// <returns>  an ArrayList of TokenObj </returns>
		public virtual List<TokenObj> ProcessToTokenObj(string inText) {
			bool debugFlag = false;
			return ProcessToTokenObj(inText, debugFlag);
		}
		/// <summary>
		/// cSpell correction process, output to an ArrayList of TokenObj by using 
		/// funcMode and rankMode from configuratin files, with debug print option.
		/// </summary>
		/// <param name="inText">   input text to be corrected </param>
		/// <param name="debugFlag"> boolean flag for debug print </param>
		/// <returns>  an ArrayList of TokenObj </returns>
		public virtual List<TokenObj> ProcessToTokenObj(string inText, bool debugFlag) {
			DebugPrint.Println("====== SpellApi.Process( ), funcMode: " + funcMode_ + ", rankMode: " + rankMode_ + " ======", debugFlag);
			// non-dictionary and dictionary base correction    
			List<TokenObj> inTokenList = TextObj.TextToTokenList(inText);
			List<TokenObj> outTokenList = CorrectionApi.ProcessByTokenObj(inTokenList, this, debugFlag);
			return outTokenList;
		}
		/// <summary>
		/// cSpell correction process, output to an ArrayList of TokenObj by 
		/// specifying funcMode and rankMode, with debug print option.
		/// </summary>
		/// <param name="inText">   input text to be corrected </param>
		/// <param name="funcMode"> funcMode for correction: NW/RW-Merge/Split/1To1 </param>
		/// <param name="rankMode"> rankMode for select correction from the candidate for NW
		///                     Split/1To1 </param>
		/// <param name="debugFlag"> boolean flag for debug print </param>
		/// <returns>  an ArrayList of TokenObj </returns>
		public virtual List<TokenObj> ProcessToTokenObj(string inText, int funcMode, int rankMode, bool debugFlag) {
			// update modes
			this.funcMode_ = funcMode;
			this.rankMode_ = rankMode;
			return ProcessToTokenObj(inText, debugFlag);
		}
		// non-word, real-word, 1-to-1, split, merge
		public virtual void SetFuncMode(int funcMode) {
			funcMode_ = funcMode;
		}
		// orthographic, frequency, context, combine, cSpell
		public virtual void SetRankMode(int rankMode) {
			rankMode_ = rankMode;
		}
		public virtual void SetCanMaxCandNo(int maxCandNo) {
			cMaxCandNo_ = maxCandNo;
		}
		public virtual int GetFuncMode() {
			return funcMode_;
		}
		public virtual int GetRankMode() {
			return rankMode_;
		}
		// files
		public virtual Dictionary<string, string> GetInformalExpressionMap() {
			return infExpMap_;
		}
		// dictionary
		public virtual RootDictionary GetCheckDic() {
			return checkDic_;
		}
		public virtual RootDictionary GetSuggestDic() {
			return suggestDic_;
		}
		public virtual RootDictionary GetSplitWordDic() {
			return splitWordDic_;
		}
		public virtual RootDictionary GetMwDic() {
			return mwDic_;
		}
		public virtual RootDictionary GetPnDic() {
			return pnDic_;
		}
		public virtual RootDictionary GetAaDic() {
			return aaDic_;
		}
		public virtual RootDictionary GetSvDic() {
			return svDic_;
		}
		public virtual RootDictionary GetUnitDic() {
			return unitDic_;
		}
		public virtual WordWcMap GetWordWcMap() {
			return wordWcMap_;
		}
		public virtual Word2Vec GetWord2VecIm() {
			return word2VecIm_;
		}
		public virtual Word2Vec GetWord2VecOm() {
			return word2VecOm_;
		}
		// getter - Detector
		public virtual int GetMaxLegitTokenLength() {
			return maxLegitTokenLength_;
		}
		public virtual int GetDetectorRwSplitWordMinLength() {
			return dRwSplitWordMinLength_;
		}
		public virtual int GetDetectorRwSplitWordMinWc() {
			return dRwSplitWordMinWc_;
		}
		public virtual int GetDetectorRw1To1WordMinLength() {
			return dRw1To1WordMinLength_;
		}
		public virtual int GetDetectorRw1To1WordMinWc() {
			return dRw1To1WordMinWc_;
		}
		// getter - Candidates
		public virtual int GetCanMaxCandNo() {
			return cMaxCandNo_;
		}
		public virtual int GetCanNdMaxSplitNo() {
			return cNdMaxSplitNo_;
		}
		public virtual int GetCanNw1To1WordMaxLength() {
			return cNw1To1WordMaxLength_;
		}
		public virtual int GetCanNwMaxSplitNo() {
			return cNwMaxSplitNo_;
		}
		public virtual int GetCanNwMaxMergeNo() {
			return cNwMaxMergeNo_;
		}
		public virtual bool GetCanNwMergeWithHyphen() {
			return cNwMergeWithHyphen_;
		}
		public virtual int GetCanRw1To1WordMaxLength() {
			return cRw1To1WordMaxLength_;
		}
		public virtual int GetCanRwMaxSplitNo() {
			return cRwMaxSplitNo_;
		}
		public virtual int GetCanRwMaxMergeNo() {
			return cRwMaxMergeNo_;
		}
		public virtual bool GetCanRwMergeWithHyphen() {
			return cRwMergeWithHyphen_;
		}
		public virtual int GetCanRwShortSplitWordLength() {
			return cRwShortSplitWordLength_;
		}
		public virtual int GetCanRwMaxShortSplitWordNo() {
			return cRwMaxShortSplitWordNo_;
		}
		public virtual int GetCanRwMergeCandMinWc() {
			return cRwMergeCandMinWc_;
		}
		public virtual int GetCanRwSplitCandMinWc() {
			return cRwSplitCandMinWc_;
		}
		public virtual int GetCanRw1To1CandMinLength() {
			return cRw1To1CandMinLength_;
		}
		public virtual int GetCanRw1To1CandMinWc() {
			return cRw1To1CandMinWc_;
		}
		public virtual int GetCanRw1To1CandMaxKeySize() {
			return cRw1To1CandMaxKeySize_;
		}
		// Getter - Rankers
		// used in non-word split and 1-to-1 to find candidtes within range
		public virtual double GetRankNwS1RankRangeFac() {
			return rNwS1RankRangeFac_;
		}
		// used in non-word split and 1-to-1 to find candidtes using Oscore
		public virtual double GetRankNwS1MinOScore() {
			return rNwS1MinOScore_;
		}
		// used in the real-word 1-to-1 score rules: RankRealWordByContext.java
		public virtual double GetRankRw1To1CFac() {
			return rRw1To1CFac_;
		}
		// used in the real-word split score rules: RankRealWordSplitByContext.java
		public virtual double GetRankRwSplitCFac() {
			return rRwSplitCFac_;
		}
		// used in the real-word merge score rules: RankRealWordMergeByContext.java
		public virtual double GetRankRwMergeCFac() {
			return rRwMergeCFac_;
		}
		public virtual double GetRankRw1To1WordMinCs() {
			return rRw1To1WordMinCs_;
		}
		public virtual double GetRankRw1To1CandCsFac() {
			return rRw1To1CandCsFac_;
		}
		public virtual double GetRankRw1To1CandMinCs() {
			return rRw1To1CandMinCs_;
		}
		public virtual double GetRankRw1To1CandCsDist() {
			return rRw1To1CandCsDist_;
		}
		public virtual double GetRankRw1To1CandFsFac() {
			return rRw1To1CandFsFac_;
		}
		public virtual double GetRankRw1To1CandMinFs() {
			return rRw1To1CandMinFs_;
		}
		public virtual double GetRankRw1To1CandFsDist() {
			return rRw1To1CandFsDist_;
		}
		// Scores
		public virtual double GetOrthoScoreEdDistFac() {
			return orthoScoreEdDistFac_;
		}
		public virtual double GetOrthoScorePhoneticFac() {
			return orthoScorePhoneticFac_;
		}
		public virtual double GetOrthoScoreOverlapFac() {
			return orthoScoreOverlapFac_;
		}
		// Rankers - context
		public virtual bool GetWord2VecSkipWord() {
			return word2VecSkipWord_;
		}
		public virtual int GetNw1To1ContextRadius() {
			return nw1To1ContextRadius_;
		}
		public virtual int GetNwSplitContextRadius() {
			return nwSplitContextRadius_;
		}
		public virtual int GetNwMergeContextRadius() {
			return nwMergeContextRadius_;
		}
		public virtual int GetRw1To1ContextRadius() {
			return rw1To1ContextRadius_;
		}
		public virtual int GetRwSplitContextRadius() {
			return rwSplitContextRadius_;
		}
		public virtual int GetRwMergeContextRadius() {
			return rwMergeContextRadius_;
		}

		public virtual void SetDetectNo(int detectNo) {
			detectNo_ = detectNo;
		}
		public virtual int UpdateDetectNo() {
			return detectNo_++;
		}
		public virtual int GetDetectNo() {
			return detectNo_;
		}
		public virtual void SetCorrectNo(int correctNo) {
			correctNo_ = correctNo;
		}
		public virtual int UpdateCorrectNo() {
			return correctNo_++;
		}
		public virtual int GetCorrectNo() {
			return correctNo_;
		}
		// Close persistent files, files, and database connection
		// TBD: to be used when switch to DB
		public virtual void Close() { }
		// privat methods
		// init
		private void Init() {
			bool debugFlag = false;
			Init(debugFlag);
		}

		private void Init2(bool debugFlag) {
			_logger.LogInformation("cSpellApi initialization...");
			infExpMap_ = InformalExpHandler.GetInformalExpMapFromFile(_config.Value.CS_INFORMAL_EXP_FILE);
			checkDic_.AddDictionaries2(_config.Value.CS_CHECK_DIC_FILES, debugFlag);
			suggestDic_.AddDictionaries2(_config.Value.CS_SUGGEST_DIC_FILES, debugFlag);
			splitWordDic_.AddDictionaries2(_config.Value.CS_SPLIT_WORD_DIC_FILES, debugFlag);
			mwDic_.AddDictionary(_config.Value.CS_MW_DIC_FILE);
			pnDic_.AddDictionary(_config.Value.CS_PN_DIC_FILE);
			aaDic_.AddDictionary(_config.Value.CS_AA_DIC_FILE);
			svDic_.AddDictionary(_config.Value.CS_SV_DIC_FILE);
			unitDic_.AddDictionary(_config.Value.CS_UNIT_DIC_FILE);
			wordWcMap_ = new WordWcMap(_config.Value.CS_FREQUENCY_FILE);
			word2VecIm_ = new Word2Vec(_config.Value.CS_W2V_IM_FILE);
			word2VecOm_ = new Word2Vec(_config.Value.CS_W2V_OM_FILE);

			// mode
			funcMode_ = _config.Value.CS_FUNC_MODE;
			rankMode_ = _config.Value.CS_RANK_MODE;

			// detectors
			maxLegitTokenLength_ = _config.Value.CS_MAX_LEGIT_TOKEN_LENGTH;
			dRwSplitWordMinLength_ = _config.Value.CS_DETECTOR_RW_SPLIT_WORD_MIN_LENGTH;
			dRwSplitWordMinWc_ = _config.Value.CS_DETECTOR_RW_SPLIT_WORD_MIN_WC;
			dRw1To1WordMinLength_ = _config.Value.CS_DETECTOR_RW_1TO1_WORD_MIN_LENGTH;
			dRw1To1WordMinWc_ = _config.Value.CS_DETECTOR_RW_1TO1_WORD_MIN_WC;

			// candidates
			cMaxCandNo_ = _config.Value.CS_CAN_MAX_CANDIDATE_NO;
			cNdMaxSplitNo_ = _config.Value.CS_CAN_ND_MAX_SPLIT_NO;
			cNwMaxSplitNo_ = _config.Value.CS_CAN_NW_MAX_SPLIT_NO;
			cNwMaxMergeNo_ = _config.Value.CS_CAN_NW_MAX_MERGE_NO;
			cNwMergeWithHyphen_ = _config.Value.CS_CAN_NW_MERGE_WITH_HYPHEN;
			cRwMaxSplitNo_ = _config.Value.CS_CAN_RW_MAX_SPLIT_NO;
			cRwMaxMergeNo_ = _config.Value.CS_CAN_RW_MAX_MERGE_NO;
			cRwMergeWithHyphen_ = _config.Value.CS_CAN_RW_MERGE_WITH_HYPHEN;

			cRwShortSplitWordLength_ = _config.Value.CS_CAN_RW_SHORT_SPLIT_WORD_LENGTH;
			cRwMaxShortSplitWordNo_ = _config.Value.CS_CAN_RW_MAX_SHORT_SPLIT_WORD_NO;
			cRwMergeCandMinWc_ = _config.Value.CS_CAN_RW_MERGE_CAND_MIN_WC;
			cRwSplitCandMinWc_ = _config.Value.CS_CAN_RW_SPLIT_CAND_MIN_WC;
			cRw1To1CandMinLength_ = _config.Value.CS_CAN_RW_1TO1_CAND_MIN_LENGTH;
			cRw1To1CandMinWc_ = _config.Value.CS_CAN_RW_1TO1_CAND_MIN_WC;
			cRw1To1CandMaxKeySize_ = _config.Value.CS_CAN_RW_1TO1_CAND_MAX_KEY_SIZE;

			// rankers
			rNwS1RankRangeFac_ = _config.Value.CS_RANKER_NW_S1_RANK_RANGE_FAC;
			rNwS1MinOScore_ = _config.Value.CS_RANKER_NW_S1_MIN_OSCORE;
			rRw1To1CFac_ = _config.Value.CS_RANKER_RW_1TO1_C_FAC;
			rRwSplitCFac_ = _config.Value.CS_RANKER_RW_SPLIT_C_FAC;
			rRwMergeCFac_ = _config.Value.CS_RANKER_RW_MERGE_C_FAC;
			rRw1To1WordMinCs_ = _config.Value.CS_RANKER_RW_1TO1_WORD_MIN_CS;
			rRw1To1CandCsFac_ = _config.Value.CS_RANKER_RW_1TO1_CAND_CS_FAC;
			rRw1To1CandMinCs_ = _config.Value.CS_RANKER_RW_1TO1_CAND_MIN_CS;
			rRw1To1CandCsDist_ = _config.Value.CS_RANKER_RW_1TO1_CAND_CS_DIST;
			rRw1To1CandFsFac_ = _config.Value.CS_RANKER_RW_1TO1_CAND_FS_FAC;
			rRw1To1CandMinFs_ = _config.Value.CS_RANKER_RW_1TO1_CAND_MIN_FS;
			rRw1To1CandFsDist_ = _config.Value.CS_RANKER_RW_1TO1_CAND_FS_DIST;

			// Score    
			orthoScoreEdDistFac_ = _config.Value.CS_ORTHO_SCORE_ED_DIST_FAC;
			orthoScorePhoneticFac_ = _config.Value.CS_ORTHO_SCORE_PHONETIC_FAC;
			orthoScoreOverlapFac_ = _config.Value.CS_ORTHO_SCORE_OVERLAP_FAC;

			// context
			word2VecSkipWord_ = _config.Value.CS_W2V_SKIP_WORD;
			nw1To1ContextRadius_ = _config.Value.CS_NW_1TO1_CONTEXT_RADIUS;
			nwSplitContextRadius_ = _config.Value.CS_NW_SPLIT_CONTEXT_RADIUS;
			nwMergeContextRadius_ = _config.Value.CS_NW_MERGE_CONTEXT_RADIUS;
			rw1To1ContextRadius_ = _config.Value.CS_RW_1TO1_CONTEXT_RADIUS;
			rwSplitContextRadius_ = _config.Value.CS_RW_SPLIT_CONTEXT_RADIUS;
			rwMergeContextRadius_ = _config.Value.CS_RW_MERGE_CONTEXT_RADIUS;
			_logger.LogInformation("cSpellApi initialized successfully");
		}

		// update parameter from the config file to cSpellApi
		private void Init(bool debugFlag) {
			// get config file from environment variable
			bool useClassPath = false;
			if (string.ReferenceEquals(configFile_, null)) {
				useClassPath = true;
				configFile_ = "data.Config.cSpell";
			}
			// read in configuration file
			conf_ = new Configuration(configFile_, useClassPath);
			if (properties_ != null) {
				conf_.OverwriteProperties(properties_);
			}
			string cSpellDir = conf_.GetProperty(Configuration.CS_DIR);
			// files: pre-correction
			string infExpFile = cSpellDir + conf_.GetProperty(Configuration.CS_INFORMAL_EXP_FILE);
			infExpMap_ = InformalExpHandler.GetInformalExpMapFromFile(infExpFile);
			// get dictionary for spell checker
			string checkDicFileStrs = conf_.GetProperty(Configuration.CS_CHECK_DIC_FILES);
			checkDic_.AddDictionaries(checkDicFileStrs, cSpellDir, debugFlag);
			// get dictionary for spell suggestion - candidate
			string suggestDicFileStrs = conf_.GetProperty(Configuration.CS_SUGGEST_DIC_FILES);
			suggestDic_.AddDictionaries(suggestDicFileStrs, cSpellDir, debugFlag);
			// no acr/abb dictionary: en + pn, used for split check
			string splitWordDicFileStrs = conf_.GetProperty(Configuration.CS_SPLIT_WORD_DIC_FILES);
			splitWordDic_.AddDictionaries(splitWordDicFileStrs, cSpellDir, debugFlag);
			// mw dictionary
			string mwDicFile = cSpellDir + conf_.GetProperty(Configuration.CS_MW_DIC_FILE);
			mwDic_.AddDictionary(mwDicFile);
			// properNoun dictionary
			string pnDicFile = cSpellDir + conf_.GetProperty(Configuration.CS_PN_DIC_FILE);
			pnDic_.AddDictionary(pnDicFile);
			// abb/acr dictionary
			string aaDicFile = cSpellDir + conf_.GetProperty(Configuration.CS_AA_DIC_FILE);
			aaDic_.AddDictionary(aaDicFile);
			// spVar dictionary
			string svDicFile = cSpellDir + conf_.GetProperty(Configuration.CS_SV_DIC_FILE);
			svDic_.AddDictionary(svDicFile);
			// unit file
			string unitDicFile = cSpellDir + conf_.GetProperty(Configuration.CS_UNIT_DIC_FILE);
			unitDic_.AddDictionary(unitDicFile);
			// frequency file
			string frequencyFile = cSpellDir + conf_.GetProperty(Configuration.CS_FREQUENCY_FILE);
			wordWcMap_ = new WordWcMap(frequencyFile);
			// word2Vec file
			string word2VecImFile = cSpellDir + conf_.GetProperty(Configuration.CS_W2V_IM_FILE);
			word2VecIm_ = new Word2Vec(word2VecImFile);
			string word2VecOmFile = cSpellDir + conf_.GetProperty(Configuration.CS_W2V_OM_FILE);
			word2VecOm_ = new Word2Vec(word2VecOmFile);
			// mode
			funcMode_ = int.Parse(conf_.GetProperty(Configuration.CS_FUNC_MODE));
			rankMode_ = int.Parse(conf_.GetProperty(Configuration.CS_RANK_MODE));
			// detectors
			maxLegitTokenLength_ = int.Parse(conf_.GetProperty(Configuration.CS_MAX_LEGIT_TOKEN_LENGTH));
			dRwSplitWordMinLength_ = int.Parse(conf_.GetProperty(Configuration.CS_DETECTOR_RW_SPLIT_WORD_MIN_LENGTH));
			dRwSplitWordMinWc_ = int.Parse(conf_.GetProperty(Configuration.CS_DETECTOR_RW_SPLIT_WORD_MIN_WC));
			dRw1To1WordMinLength_ = int.Parse(conf_.GetProperty(Configuration.CS_DETECTOR_RW_1TO1_WORD_MIN_LENGTH));
			dRw1To1WordMinWc_ = int.Parse(conf_.GetProperty(Configuration.CS_DETECTOR_RW_1TO1_WORD_MIN_WC));
			// candidates
			cMaxCandNo_ = int.Parse(conf_.GetProperty(Configuration.CS_CAN_MAX_CANDIDATE_NO));
			cNdMaxSplitNo_ = int.Parse(conf_.GetProperty(Configuration.CS_CAN_ND_MAX_SPLIT_NO));
			cNwMaxSplitNo_ = int.Parse(conf_.GetProperty(Configuration.CS_CAN_NW_MAX_SPLIT_NO));
			cNwMaxMergeNo_ = int.Parse(conf_.GetProperty(Configuration.CS_CAN_NW_MAX_MERGE_NO));
			cNwMergeWithHyphen_ = bool.Parse(conf_.GetProperty(Configuration.CS_CAN_NW_MERGE_WITH_HYPHEN));
			cRwMaxSplitNo_ = int.Parse(conf_.GetProperty(Configuration.CS_CAN_RW_MAX_SPLIT_NO));
			cRwMaxMergeNo_ = int.Parse(conf_.GetProperty(Configuration.CS_CAN_RW_MAX_MERGE_NO));
			cRwMergeWithHyphen_ = bool.Parse(conf_.GetProperty(Configuration.CS_CAN_RW_MERGE_WITH_HYPHEN));

			cRwShortSplitWordLength_ = int.Parse(conf_.GetProperty(Configuration.CS_CAN_RW_SHORT_SPLIT_WORD_LENGTH));
			cRwMaxShortSplitWordNo_ = int.Parse(conf_.GetProperty(Configuration.CS_CAN_RW_MAX_SHORT_SPLIT_WORD_NO));
			cRwMergeCandMinWc_ = int.Parse(conf_.GetProperty(Configuration.CS_CAN_RW_MERGE_CAND_MIN_WC));
			cRwSplitCandMinWc_ = int.Parse(conf_.GetProperty(Configuration.CS_CAN_RW_SPLIT_CAND_MIN_WC));
			cRw1To1CandMinLength_ = int.Parse(conf_.GetProperty(Configuration.CS_CAN_RW_1TO1_CAND_MIN_LENGTH));
			cRw1To1CandMinWc_ = int.Parse(conf_.GetProperty(Configuration.CS_CAN_RW_1TO1_CAND_MIN_WC));
			cRw1To1CandMaxKeySize_ = int.Parse(conf_.GetProperty(Configuration.CS_CAN_RW_1TO1_CAND_MAX_KEY_SIZE));

			// rankers
			rNwS1RankRangeFac_ = double.Parse(conf_.GetProperty(Configuration.CS_RANKER_NW_S1_RANK_RANGE_FAC));
			rNwS1MinOScore_ = double.Parse(conf_.GetProperty(Configuration.CS_RANKER_NW_S1_MIN_OSCORE));
			rRw1To1CFac_ = double.Parse(conf_.GetProperty(Configuration.CS_RANKER_RW_1TO1_C_FAC));
			rRwSplitCFac_ = double.Parse(conf_.GetProperty(Configuration.CS_RANKER_RW_SPLIT_C_FAC));
			rRwMergeCFac_ = double.Parse(conf_.GetProperty(Configuration.CS_RANKER_RW_MERGE_C_FAC));
			rRw1To1WordMinCs_ = double.Parse(conf_.GetProperty(Configuration.CS_RANKER_RW_1TO1_WORD_MIN_CS));
			rRw1To1CandCsFac_ = double.Parse(conf_.GetProperty(Configuration.CS_RANKER_RW_1TO1_CAND_CS_FAC));
			rRw1To1CandMinCs_ = double.Parse(conf_.GetProperty(Configuration.CS_RANKER_RW_1TO1_CAND_MIN_CS));
			rRw1To1CandCsDist_ = double.Parse(conf_.GetProperty(Configuration.CS_RANKER_RW_1TO1_CAND_CS_DIST));
			rRw1To1CandFsFac_ = double.Parse(conf_.GetProperty(Configuration.CS_RANKER_RW_1TO1_CAND_FS_FAC));
			rRw1To1CandMinFs_ = double.Parse(conf_.GetProperty(Configuration.CS_RANKER_RW_1TO1_CAND_MIN_FS));
			rRw1To1CandFsDist_ = double.Parse(conf_.GetProperty(Configuration.CS_RANKER_RW_1TO1_CAND_FS_DIST));

			// Score    
			orthoScoreEdDistFac_ = double.Parse(conf_.GetProperty(Configuration.CS_ORTHO_SCORE_ED_DIST_FAC));
			orthoScorePhoneticFac_ = double.Parse(conf_.GetProperty(Configuration.CS_ORTHO_SCORE_PHONETIC_FAC));
			orthoScoreOverlapFac_ = double.Parse(conf_.GetProperty(Configuration.CS_ORTHO_SCORE_OVERLAP_FAC));

			// context
			word2VecSkipWord_ = bool.Parse(conf_.GetProperty(Configuration.CS_W2V_SKIP_WORD));
			nw1To1ContextRadius_ = int.Parse(conf_.GetProperty(Configuration.CS_NW_1TO1_CONTEXT_RADIUS));
			nwSplitContextRadius_ = int.Parse(conf_.GetProperty(Configuration.CS_NW_SPLIT_CONTEXT_RADIUS));
			nwMergeContextRadius_ = int.Parse(conf_.GetProperty(Configuration.CS_NW_MERGE_CONTEXT_RADIUS));
			rw1To1ContextRadius_ = int.Parse(conf_.GetProperty(Configuration.CS_RW_1TO1_CONTEXT_RADIUS));
			rwSplitContextRadius_ = int.Parse(conf_.GetProperty(Configuration.CS_RW_SPLIT_CONTEXT_RADIUS));
			rwMergeContextRadius_ = int.Parse(conf_.GetProperty(Configuration.CS_RW_MERGE_CONTEXT_RADIUS));
		}
		private static void TestProcess(string configFile) {
			// init
			Console.WriteLine("----- Test Process Text: -----");
			string inText = "We  cant theredve spel and 987Pfimbria dianosed.Plz u r good123. ";
			CSpellApi cSpellApi = new CSpellApi(configFile);
			string outText = cSpellApi.ProcessToStr(inText);
			// print out
			Console.WriteLine("--------- CSpellApi( ) -----------");
			Console.WriteLine("In: [" + inText + "]");
			Console.WriteLine("Out: [" + outText + "]");
		}
		// test driver
		public static void MainTest(string[] args) {
			string configFile = "../data/Config/cSpell.properties";
			if (args.Length == 1) {
				configFile = args[0];
			} else if (args.Length > 0) {
				Console.WriteLine("Usage: java CSpellApi <configFile>");
				Environment.Exit(0);
			}
			TestProcess(configFile);
		}

		// data member
		private string configFile_ = null;
		private Configuration conf_ = null;
		private Dictionary<string, string> properties_ = null; // overwrite properties
		// cSpell process ranking mode
		public const int RANK_MODE_ORTHOGRAPHIC = 0; // pre-corr + orthographic
		public const int RANK_MODE_FREQUENCY = 1; // pre-corr + frequency
		public const int RANK_MODE_CONTEXT = 2; // pre-corr + context
		public const int RANK_MODE_NOISY_CHANNEL = 3; // pre-corr + noisy channel
		public const int RANK_MODE_ENSEMBLE = 4; // ensemble
		public const int RANK_MODE_CSPELL = 5; // cSpell
		private int rankMode_ = RANK_MODE_CSPELL; // ranking mode
		// cSpell process function mode
		public const int FUNC_MODE_ND = 0; // not dictionary base correct
		public const int FUNC_MODE_NW_1 = 1; // ND + NW_1To1
		public const int FUNC_MODE_NW_S = 2; // ND + NW_Split
		public const int FUNC_MODE_NW_M = 3; // ND + NW_Merge
		public const int FUNC_MODE_NW_S_1 = 4; // ND + NW_Split_1To1
		public const int FUNC_MODE_NW_A = 5; // ND + NW All, 1, S, M
		public const int FUNC_MODE_RW_1 = 6; // NW + RW_1_to_1
		public const int FUNC_MODE_RW_S = 7; // NW + RW_Split
		public const int FUNC_MODE_RW_M = 8; // NW + RW_Merge
		public const int FUNC_MODE_RW_M_S = 9; // NW + RW_Merge_Split
		public const int FUNC_MODE_RW_A = 10; // NW + RW_All
		private int funcMode_ = FUNC_MODE_RW_A; // default mode
		// candidate & correction related data
		// non-dictionary, pre-correction related data
		private Dictionary<string, string> infExpMap_ = null; // informal express Map
		// Dictinoary related data
		//RootDictionary checkDic_ = new BasicDictionary();
		// dic for spelling checker for spelling error detection
		private RootDictionary checkDic_ = DictionaryFactory.GetDictionary(DictionaryFactory.DIC_BASIC);
		// dic for spelling checker for spelling suggestion
		private RootDictionary suggestDic_ = DictionaryFactory.GetDictionary(DictionaryFactory.DIC_BASIC);
		// dic for split word suggestion - English words + proper nouns
		// no acronyms and abbreviation
		private RootDictionary splitWordDic_ = DictionaryFactory.GetDictionary(DictionaryFactory.DIC_BASIC);
		// dictionary include verified multiwords    
		private RootDictionary mwDic_ = DictionaryFactory.GetDictionary(DictionaryFactory.DIC_BASIC);
		// dictionary include properNoun    
		private RootDictionary pnDic_ = DictionaryFactory.GetDictionary(DictionaryFactory.DIC_BASIC);
		// dictionary include abbreviations and acronyms
		private RootDictionary aaDic_ = DictionaryFactory.GetDictionary(DictionaryFactory.DIC_BASIC);
		// dictionary include spVar
		private RootDictionary svDic_ = DictionaryFactory.GetDictionary(DictionaryFactory.DIC_BASIC);
		// dictionary include unit
		private RootDictionary unitDic_ = DictionaryFactory.GetDictionary(DictionaryFactory.DIC_BASIC);

		// score and ranking related data
		// frequency map: word|WC
		private WordWcMap wordWcMap_ = null;
		// wrod2Vec: word2VecObj Input and output matrix
		private Word2Vec word2VecIm_ = null;
		private Word2Vec word2VecOm_ = null;
		// detector
		private int maxLegitTokenLength_ = 30; // max legit token length
		private int dRwSplitWordMinLength_ = 3; // min RW split word length
		private int dRwSplitWordMinWc_ = 200; // min RW split word wc
		private int dRw1To1WordMinLength_ = 3; // min RW 1-to-1 word length
		private int dRw1To1WordMinWc_ = 65; // min RW 1-to-1 word wc
		// candidate
		private int cMaxCandNo_ = 25; // max candidate no, configurable
		// candidate non-word
		private int cNdMaxSplitNo_ = 5; // max non-dictionary split recurNo
		private int cNw1To1WordMaxLength_ = 25; // non-word 1To1 word max length
		private int cNwMaxSplitNo_ = 5; // max non-word split no
		private int cNwMaxMergeNo_ = 2; // max non-word merge no
		private bool cNwMergeWithHyphen_ = true; // for non-word merge
		// candidate real-word
		private int cRw1To1WordMaxLength_ = 10; // real-word 1To1 word max length
		private int cRwMaxSplitNo_ = 2; // max real-word split no
		private int cRwMaxMergeNo_ = 2; // max real-word merge no
		private bool cRwMergeWithHyphen_ = false; // for real-word merge
		// candidate
		private int cRwShortSplitWordLength_ = 3; // rw short split word length
		private int cRwMaxShortSplitWordNo_ = 2; // RW max short split word no
		private int cRwMergeCandMinWc_ = -1; // min candidate wc, configurable
		private int cRwSplitCandMinWc_ = 200; // min candidate wc, configurable
		private int cRw1To1CandMinLength_ = 2; // min candidate Length
		private int cRw1To1CandMinWc_ = -1; // min candidate wc, configurable
		private int cRw1To1CandMaxKeySize_ = 1000000000; // max key size for hashMap
		// rankers - score factor for real-word merge rule: 0.0 ~ 1.0
		private double rNwS1RankRangeFac_ = 0.05; // use for split and 1To1
		private double rNwS1MinOScore_ = 2.7; // use for split and 1To1
		private double rRw1To1CFac_ = 0.00;
		private double rRwSplitCFac_ = 0.01;
		private double rRwMergeCFac_ = 0.60;
		private double rRw1To1WordMinCs_ = -0.085;
		private double rRw1To1CandCsFac_ = 0.01;
		private double rRw1To1CandMinCs_ = 0.00;
		private double rRw1To1CandCsDist_ = 0.085; // these two numbers is close
		private double rRw1To1CandFsFac_ = 0.035;
		private double rRw1To1CandMinFs_ = -0.0006;
		private double rRw1To1CandFsDist_ = 0.02;
		// Score
		private double orthoScoreEdDistFac_ = 1.00;
		private double orthoScorePhoneticFac_ = 0.70;
		private double orthoScoreOverlapFac_ = 0.80;
		// Context - context radius, configurable
		private bool word2VecSkipWord_ = true;
		private int nw1To1ContextRadius_ = 2;
		private int nwSplitContextRadius_ = 2;
		private int nwMergeContextRadius_ = 2;
		private int rw1To1ContextRadius_ = 2;
		private int rwSplitContextRadius_ = 2;
		private int rwMergeContextRadius_ = 2;

		private int detectNo_ = 0; // detect No
		private int correctNo_ = 0; // correct No, detect not necessary correct
	}

}
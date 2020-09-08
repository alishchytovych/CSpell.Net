using System;

namespace SpellChecker {
	public class SpellCheckerConfiguration {
		//----- dictionary files ------------------------
		// CS_INFORMAL_EXP_FILE: the relative path of informal expression file.
		// CS_CHECK_DIC_FILES: dictionary used for the spelling checker
		// CS_SUGGEST_DIC_FILES: dictionary used for candidate suggestion
		// CS_SPLIT_WORD_DIC_FILES: dictionary used for the split candidate 
		// CS_MW_DIC_FILE: dictionary for multiwords, used in split
		// CS_UNIT_DIC_FILE: units, used in both spelling checker and split candidates
		// CS_SV_DIC_FILE: Spelling variant file, used for detector exceptions
		// CS_AA_DIC_FILE: Abbreviation and acronym file, used for detector exceptions
		// CS_PN_DIC_FILE: Proper noun file, used for detector exceptions
		public String CS_INFORMAL_EXP_FILE { get; set; }
		public String CS_CHECK_DIC_FILES { get; set; }
		public String CS_SUGGEST_DIC_FILES { get; set; }
		public String CS_SPLIT_WORD_DIC_FILES { get; set; }
		public String CS_MW_DIC_FILE { get; set; }
		public String CS_UNIT_DIC_FILE { get; set; }
		public String CS_SV_DIC_FILE { get; set; }
		public String CS_AA_DIC_FILE { get; set; }
		public String CS_PN_DIC_FILE { get; set; }
		//------ scoring and ranking files ---------------------------------
		public String CS_FREQUENCY_FILE { get; set; }
		public String CS_W2V_IM_FILE { get; set; }
		public String CS_W2V_OM_FILE { get; set; }
		//---- mode -----------------------------------
		public int CS_FUNC_MODE { get; set; }
		public int CS_RANK_MODE { get; set; }
		//---- detector -----------------------------------
		public int CS_MAX_LEGIT_TOKEN_LENGTH { get; set; }
		public int CS_DETECTOR_RW_SPLIT_WORD_MIN_LENGTH { get; set; }
		public int CS_DETECTOR_RW_SPLIT_WORD_MIN_WC { get; set; }
		public int CS_DETECTOR_RW_1TO1_WORD_MIN_LENGTH { get; set; }
		public int CS_DETECTOR_RW_1TO1_WORD_MIN_WC { get; set; }
		//---- candidates -----------------------------------
		public int CS_CAN_MAX_CANDIDATE_NO { get; set; }
		public int CS_CAN_ND_MAX_SPLIT_NO { get; set; }
		public int CS_CAN_NW_1TO1_WORD_MAX_LENGTH { get; set; }
		public int CS_CAN_NW_MAX_SPLIT_NO { get; set; }
		public int CS_CAN_NW_MAX_MERGE_NO { get; set; }
		public bool CS_CAN_NW_MERGE_WITH_HYPHEN { get; set; }
		public int CS_CAN_RW_1TO1_WORD_MAX_LENGTH { get; set; }
		public int CS_CAN_RW_MAX_SPLIT_NO { get; set; }
		public int CS_CAN_RW_MAX_MERGE_NO { get; set; }
		public bool CS_CAN_RW_MERGE_WITH_HYPHEN { get; set; }
		public int CS_CAN_RW_SHORT_SPLIT_WORD_LENGTH { get; set; }
		public int CS_CAN_RW_MAX_SHORT_SPLIT_WORD_NO { get; set; }
		public int CS_CAN_RW_MERGE_CAND_MIN_WC { get; set; }
		public int CS_CAN_RW_SPLIT_CAND_MIN_WC { get; set; }
		public int CS_CAN_RW_1TO1_CAND_MIN_LENGTH { get; set; }
		public int CS_CAN_RW_1TO1_CAND_MIN_WC { get; set; }
		public int CS_CAN_RW_1TO1_CAND_MAX_KEY_SIZE { get; set; }
		//---- rankers -----------------------------------
		public double CS_RANKER_NW_S1_RANK_RANGE_FAC { get; set; }
		public double CS_RANKER_NW_S1_MIN_OSCORE { get; set; }
		public double CS_RANKER_RW_MERGE_C_FAC { get; set; }
		public double CS_RANKER_RW_SPLIT_C_FAC { get; set; }
		public double CS_RANKER_RW_1TO1_C_FAC { get; set; }
		public double CS_RANKER_RW_1TO1_CAND_MIN_CS { get; set; }
		public double CS_RANKER_RW_1TO1_CAND_CS_DIST { get; set; }
		public double CS_RANKER_RW_1TO1_CAND_CS_FAC { get; set; }
		public double CS_RANKER_RW_1TO1_WORD_MIN_CS { get; set; }
		public double CS_RANKER_RW_1TO1_CAND_MIN_FS { get; set; }
		public double CS_RANKER_RW_1TO1_CAND_FS_DIST { get; set; }
		public double CS_RANKER_RW_1TO1_CAND_FS_FAC { get; set; }
		//---- score -----------------------------------
		public double CS_ORTHO_SCORE_ED_DIST_FAC { get; set; }
		public double CS_ORTHO_SCORE_PHONETIC_FAC { get; set; }
		public double CS_ORTHO_SCORE_OVERLAP_FAC { get; set; }
		//---- context setup -----------------------------------
		public bool CS_W2V_SKIP_WORD { get; set; }
		public int CS_NW_1TO1_CONTEXT_RADIUS { get; set; }
		public int CS_NW_SPLIT_CONTEXT_RADIUS { get; set; }
		public int CS_NW_MERGE_CONTEXT_RADIUS { get; set; }
		public int CS_RW_1TO1_CONTEXT_RADIUS { get; set; }
		public int CS_RW_SPLIT_CONTEXT_RADIUS { get; set; }
		public int CS_RW_MERGE_CONTEXT_RADIUS { get; set; }
	}
}
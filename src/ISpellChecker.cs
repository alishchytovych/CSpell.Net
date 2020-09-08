using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.NlpUtil;

namespace SpellChecker {
	public interface ISpellChecker {
		public String Correct(String input);
		public (String, List<TokenObj>) CorrectExt(String input);
		public void Recover();
	}
}
using System.Text.RegularExpressions;

namespace SpellChecker.Engines.cSpell {
	public class JRegex : Regex {
		public JRegex(string pattern) : base(@"\A(?:" + pattern + @")\z") { }
		public JRegex(string pattern, RegexOptions options) : base(@"\A(?:" + pattern + @")\z", options) { }
	}
}
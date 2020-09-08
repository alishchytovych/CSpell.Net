using System.Collections.Generic;
using System.Text;

namespace SpellChecker.Engines.cSpell.Extensions {
	internal static class HashSetHelper {
		public static void addAll<T>(this HashSet<T> set, HashSet<T> curSplitSet) {
			//var entries = new HashSet<T>();
			foreach (var item in curSplitSet) {
				set.Add(item);
			}
			//return entries;
		}
		public static string ToStringList<T>(this HashSet<T> set) {
			StringBuilder ret = new StringBuilder();
			ret.Append("[");
			bool start = true;
			foreach (var item in set) {
				if (!start)
					ret.Append(", ");
				else
					start = false;
				ret.Append(item);
			}
			ret.Append("]");
			return ret.ToString();
		}
	}
}
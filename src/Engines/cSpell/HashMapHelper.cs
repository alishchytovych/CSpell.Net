﻿using System.Collections.Generic;
namespace SpellChecker.Engines.cSpell.Extensions {
	internal static class HashMapHelper {
		public static HashSet<KeyValuePair<TKey, TValue>> SetOfKeyValuePairs<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) {
			HashSet<KeyValuePair<TKey, TValue>> entries = new HashSet<KeyValuePair<TKey, TValue>>();
			foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary) {
				entries.Add(keyValuePair);
			}
			return entries;
		}

		public static TValue GetValueOrNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
			TValue ret;
			dictionary.TryGetValue(key, out ret);
			return ret;
		}
	}
}
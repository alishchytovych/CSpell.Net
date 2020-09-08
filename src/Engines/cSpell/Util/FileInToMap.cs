using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SpellChecker.Engines.cSpell.Lib;

namespace SpellChecker.Engines.cSpell.Util {

	/// <summary>
	///***************************************************************************
	/// This is a utility class for file input to Map. It reads input from a file 
	/// and convert to a map&lt;String, String&gt; or map&lt;String, 
	/// Set&lt;Strings&gt;&gt;.
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
	public class FileInToMap {
		// 1st field is key, stored in String
		// 2nd field is value, stored in String
		public static Dictionary<string, string> GetHashMapByFields(string inFile) {
			bool verboseFlag = false;
			return GetHashMapByFields(inFile, verboseFlag);
		}
		public static Dictionary<string, string> GetHashMapByFields(string inFile, bool verboseFlag) {
			// print out if verbose
			if (verboseFlag == true) {
				Console.WriteLine("- Get HashMap by Fields from: " + inFile);
			}
			Dictionary<string, string> outMap = new Dictionary<string, string>();
			try {
				String[] lines = File.ReadAllLines(inFile, Encoding.UTF8);

				foreach (var line in lines) {
					if (line.StartsWith(GlobalVars.CT_STR, StringComparison.Ordinal) == false) {
						var buf = line.Split(GlobalVars.FS_STR);
						string keyStr = buf[0];
						string valueStr = buf[1];
						outMap[keyStr] = valueStr;
					}
				}
			} catch (Exception x1) {
				Console.Error.WriteLine("** Err@FileInToMap.GetHashMapByFields( ): " + x1.ToString());
			}
			return outMap;
		}
		// 1st field is key, stored in String
		// the rest fields are values, stored in hashSet<String>
		public static Dictionary<string, HashSet<string>> GetHashMapSetByFields(string inFile) {
			bool verboseFlag = false;
			return GetHashMapSetByFields(inFile, verboseFlag);
		}
		public static Dictionary<string, HashSet<string>> GetHashMapSetByFields(string inFile, bool verboseFlag) {
			if (verboseFlag == true) {
				Console.WriteLine("- Get HashMapSet by Field from: " + inFile);
			}
			Dictionary<string, HashSet<string>> outMap = new Dictionary<string, HashSet<string>>();

			try {
				String[] lines = File.ReadAllLines(inFile, Encoding.UTF8);

				// go through all lines
				foreach (var line in lines) {
					if (line.StartsWith(GlobalVars.CT_STR, StringComparison.Ordinal) == false) {
						var buf = line.Split(GlobalVars.FS_STR);
						string keyStr = buf[0];
						HashSet<string> values = new HashSet<string>();
						for (int i = 1; i < buf.Length; i++) {
							values.Add(buf[i]);
						}
						outMap[keyStr] = values;
					}
				}
			} catch (Exception x1) {
				Console.Error.WriteLine("** Err@FileInToMap.GetHashMapSetByFields( ): " + x1.ToString());
			}
			return outMap;
		}
	}

}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SpellChecker.Engines.cSpell.Lib;

namespace SpellChecker.Engines.cSpell.Util {

	/// <summary>
	///***************************************************************************
	/// This is a utility class for file input to a Set. It read input from a file 
	/// and convert to a set.
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
	public class FileInToSet {
		public static ISet<string> GetSetByLine(string inFile) {
			bool verboseFlag = false;
			bool lowerCaseFlag = true;
			return GetSetByLine(inFile, lowerCaseFlag, verboseFlag);
		}
		// use the whole line
		public static ISet<string> GetSetByLine(string inFile, bool lowerCaseFlag) {
			bool verboseFlag = false;
			return GetSetByLine(inFile, lowerCaseFlag, verboseFlag);
		}
		public static ISet<string> GetSetByLine(string inFile, bool lowerCaseFlag, bool verboseFlag) {
			ISet<string> outSet = null;
			if (verboseFlag == true) {
				Console.WriteLine("- Get Set by line from: " + inFile);
			}
			try {
				String[] lines = File.ReadAllLines(inFile, Encoding.UTF8);
				/* nio does not support certain char, used the old way
				BufferedReader reader = Files.newBufferedReader(
				    Paths.get(inFile), Charset.forName("UTF-8"));
				*/
				if (lowerCaseFlag == true) { // not case sensitive
					outSet = lines.Select(str => str.ToLower()).ToHashSet();
				} else {
					outSet = lines.ToHashSet();
				}
			} catch (Exception x1) {
				Console.Error.WriteLine("** Err@FileInToSet.GetSetByLine( ): " + x1.ToString());
			}
			return outSet;
		}
		public static HashSet<string> GetHashSetByLine(string inFile) {
			ISet<string> outSet = GetSetByLine(inFile);
			return new HashSet<string>(outSet);
		}
		public static HashSet<string> GetHashSetByLine(string inFile, bool lowerCaseFlag) {
			ISet<string> outSet = GetSetByLine(inFile, lowerCaseFlag);
			return new HashSet<string>(outSet);
		}
		// use the specified field
		public static ISet<string> GetSetByField(string inFile, int fieldNo, bool lowercaseFlag) {
			bool verboseFlag = false;
			return GetSetByField(inFile, fieldNo, lowercaseFlag, verboseFlag);
		}
		public static ISet<string> GetSetByField(string inFile, int fieldNo, bool lowercaseFlag, bool verboseFlag) {
			if (verboseFlag == true) {
				Console.WriteLine("- Get Set by field from: " + inFile);
			}
			int fNo = fieldNo - 1; // change from 1 to 0
			ISet<string> outSet = null;
			try {
				String[] lines = File.ReadAllLines(inFile, Encoding.UTF8);
				// lowercase
				if (lowercaseFlag == true) {
					outSet = lines.Where(line => line.StartsWith("#") == false).Select(line => line.Split("\\|") [fNo].ToLower()).ToHashSet();
				} else {
					outSet = lines.Where(line => line.StartsWith("#") == false).Select(line => line.Split("\\|") [fNo]).ToHashSet();
				}
			} catch (Exception x1) {
				Console.Error.WriteLine("** Err@FileInToSet.GetSetByField( ): " + x1.ToString());
			}
			return outSet;
		}
		public static HashSet<string> GetHashSetByField(string inFile, int fieldNo, bool lowercaseFlag) {
			ISet<string> outSet = GetSetByField(inFile, fieldNo, lowercaseFlag);
			return new HashSet<string>(outSet);
		}
	}

}
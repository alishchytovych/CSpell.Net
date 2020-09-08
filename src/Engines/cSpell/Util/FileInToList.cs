using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SpellChecker.Engines.cSpell.Lib;

namespace SpellChecker.Engines.cSpell.Util {

	/// <summary>
	///***************************************************************************
	/// This is a utility class for file input to List. It reads input from a file 
	/// and convert them to a list.
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
	public class FileInToList {
		// private constructor so no can instantiate
		private FileInToList() { }
		public static IList<string> GetListByLine(string inFile) {
			bool verboseFlag = false;
			return GetListByLine(inFile, verboseFlag);
		}
		public static IList<string> GetListByLine(string inFile, bool verboseFlag) {
			IList<string> outList = null;
			if (verboseFlag == true) {
				Console.WriteLine("- Get List by line from: " + inFile);
			}
			try {
				var file = File.ReadAllLines(inFile, Encoding.UTF8);
				outList = new List<string>(file);
			} catch (Exception x1) {
				Console.Error.WriteLine("** Err@FileInToList.GetListByLine( ): " + x1.ToString());
			}
			return outList;
		}
	}

}
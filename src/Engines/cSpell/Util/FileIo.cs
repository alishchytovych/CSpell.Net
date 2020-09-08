using System;
using System.IO;
using System.Text;
using SpellChecker.Engines.cSpell.Lib;

namespace SpellChecker.Engines.cSpell.Util {

	/// <summary>
	///***************************************************************************
	/// This is a utility class for file I/O.
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
	public class FileIo {
		// add line separator at the end
		public static System.Action<string> PrintlnStrToFile(StreamWriter writer) {
			return str => {
				try {
					writer.BaseStream.Write(Encoding.UTF8.GetBytes(str + System.Environment.NewLine));
				} catch { }
			};
		}
		public static System.Action<string> PrintStrToFile(StreamWriter writer) {
			return str => {
				try {
					writer.BaseStream.Write(Encoding.UTF8.GetBytes(str));
				} catch { }
			};
		}
		public static string GetStrFromFile(string inFile) {
			bool verboseFlag = false;
			return GetStrFromFile(inFile, verboseFlag);
		}
		// this is use to retrieve brat data bz there is a newline at the end
		public static string GetStrFromFileAddNewLineAtTheEnd(string inFile) {
			bool verboseFlag = false;
			return GetStrFromFileAddNewLineAtTheEnd(inFile, verboseFlag);
		}
		public static string GetStrFromFileAddNewLineAtTheEnd(string inFile, bool verboseFlag) {
			string outStr = "";
			if (verboseFlag == true) {
				Console.WriteLine("===== Get String from: " + inFile);
			}
			try {
				String[] lines = File.ReadAllLines(inFile, Encoding.UTF8);
				outStr = String.Join(GlobalVars.LS_STR, lines);
				outStr += GlobalVars.LS_STR;
			} catch (Exception x1) {
				Console.Error.WriteLine("** Err@FileIo.GetStrFromFileAddNewLineAtTheEnd( ): " + x1.ToString());
			}
			return outStr;
		}
		public static string GetStrFromFile(string inFile, bool verboseFlag) {
			string outStr = null;
			if (verboseFlag == true) {
				Console.WriteLine("===== Get String from: " + inFile);
			}
			try {
				String[] lines = File.ReadAllLines(inFile, Encoding.UTF8);
				outStr = String.Join(GlobalVars.LS_STR, lines);
			} catch (Exception x1) {
				Console.Error.WriteLine("** Err@FileIo.GetStrFromFile( ): " + x1.ToString());
			}
			return outStr;
		}
	}

}
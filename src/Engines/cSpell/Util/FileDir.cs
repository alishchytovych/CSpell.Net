using System;
using System.Collections.Generic;
using System.IO;

namespace SpellChecker.Engines.cSpell.Util {
	/// <summary>
	///***************************************************************************
	/// This is a utility class for files and directory.
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
	public class FileDir {
		public static void MainTest(string[] args) {
			string inDir = "../../SpellCorrection/CHQA_SpellCorrection_Dataset/brat";
			if (args.Length == 1) {
				inDir = args[0];
			} else if (args.Length > 0) {
				Console.Error.WriteLine("Usage: java FileDir <inDir>");
				Environment.Exit(0);
			}
			// test
			string matchPattern = "[0-9]*\\.txt";
			bool verboseFlag = true;
			List<string> fileList = GetFilesInADirectoryToList(inDir, matchPattern, verboseFlag);
			// print result
			Console.WriteLine("----- FileDir( ) -----");
			Console.WriteLine("- fileList.size(): " + fileList.Count);
			Console.WriteLine(fileList);
		}
		// public methods
		public static List<string> GetFilesInADirectoryToList(string inDir, string matchPattern) {
			bool verboseFlag = false;
			return GetFilesInADirectoryToList(inDir, matchPattern, verboseFlag);
		}
		public static List<string> GetFilesInADirectoryToList(string inDir, string matchPattern, bool verboseFlag) {
			int fileNo = 0;
			List<string> fileList = new List<string>();
			try {
				String[] files = Directory.GetFiles(inDir, matchPattern);
				foreach (var file in files) {
					fileNo++;
					fileList.Add(file);
				}
			}
			// Logic for other sort of file error
			catch (Exception x) {
				// File permission problems are caught here.
				Console.WriteLine("** Err@FileDir.GetFilesInADirectoryToList( ): exception %s%n", x);
			}
			if (verboseFlag == true) {
				Console.WriteLine("--- FileDir.GetFilesInADirectoryToList( ) ---");
				Console.WriteLine("- In Dir: " + inDir);
				Console.WriteLine("- Total file no: " + fileNo);
			}
			return fileList;
		}
	}

}
using System;
using System.Collections.Generic;
using SpellChecker.Engines.cSpell.Lib;

namespace SpellChecker.Engines.cSpell.Util {

	/// <summary>
	///***************************************************************************
	/// This is a utility class for debug print.
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
	public class DebugPrint {
		// private constructor
		private DebugPrint() { }

		// public methods
		public static void Print(string inStr, bool debugFlag) {
			if (debugFlag == true) {
				Console.Write(inStr);
			}
		}
		public static void Println(string inStr, bool debugFlag) {
			if (debugFlag == true) {
				Console.WriteLine(inStr);
			}
		}
		// print process
		public static void PrintProcess(string procStr, bool debugFlag) {
			string debugStr = "====== " + procStr + " Process ======";
			Println(debugStr, debugFlag);
		}
		public static void PrintInText(string inStr, bool debugFlag) {
			string debugStr = "--- inText: [" + inStr + "]";
			Println(debugStr, debugFlag);
		}
		// print out the str correction
		public static void PrintCorrect(string funcStr, string methodStr, string inStr, string outStr, bool debugFlag) {
			string debugStr = "- Correct: [" + inStr + GlobalVars.FS_STR + outStr + GlobalVars.FS_STR + funcStr + GlobalVars.FS_STR + methodStr + "]";
			Println(debugStr, debugFlag);
		}
		// format: inWord|nonWordMerge (!wordInDic && !wordException)
		public static void PrintNwDetect(string inWord, bool nwFlag, bool dicFlag, bool exceptionFlag, bool debugFlag) {
			string debugStr = "- Detect: [" + inWord + GlobalVars.FS_STR + nwFlag + " (" + !dicFlag + " & " + !exceptionFlag + ")]";
			Println(debugStr, debugFlag);
		}
		public static void PrintNwMergeDetect(string inWord, bool nwMergeFlag, bool dicFlag, bool exceptionFlag, bool debugFlag) {
			string debugStr = "- Detect: [" + inWord + GlobalVars.FS_STR + nwMergeFlag + " (" + !dicFlag + " & " + !exceptionFlag + ")]";
			Println(debugStr, debugFlag);
		}
		public static void PrintRwMergeDetect(string inWord, bool rwMergeFlag, bool dicFlag, bool exceptionFlag, bool debugFlag) {
			string debugStr = "- Detect: [" + inWord + GlobalVars.FS_STR + rwMergeFlag + " (" + dicFlag + " & " + !exceptionFlag + ")]";
			Println(debugStr, debugFlag);
		}
		public static void PrintRwSplitDetect(string inWord, bool rwSplitFlag, bool dicFlag, bool exceptionFlag, bool lengthFlag, bool word2VecFlag, bool wcFlag, bool debugFlag) {
			string debugStr = "- Detect: [" + inWord + GlobalVars.FS_STR + rwSplitFlag + " (" + dicFlag + " & " + !exceptionFlag + " & " +
				lengthFlag + " & " + word2VecFlag + " & " + wcFlag + ")]";
			Println(debugStr, debugFlag);
		}
		public static void PrintRw1To1Detect(string inWord, bool rwSplitFlag, bool dicFlag, bool exceptionFlag, bool lengthFlag, bool word2VecFlag, bool wcFlag, bool debugFlag) {
			string debugStr = "- Detect: [" + inWord + GlobalVars.FS_STR + rwSplitFlag + " (" + dicFlag + " & " + !exceptionFlag + " & " +
				lengthFlag + " & " + word2VecFlag + " & " + wcFlag + ")]";
			Println(debugStr, debugFlag);
		}
		public static void PrintContext(List<string> contextList, bool debugFlag) {
			string debugStr = "- Context: " + contextList;
			Println(debugStr, debugFlag);
		}
		// Noisy Channel score
		public static void PrintNScore(string nScore, bool debugFlag) {
			string debugStr = "- NScore: " + nScore;
			Println(debugStr, debugFlag);
		}
		// orthographic score
		public static void PrintOScore(string oScore, bool debugFlag) {
			string debugStr = "- OScore: " + oScore;
			Println(debugStr, debugFlag);
		}
		// context score
		public static void PrintCScore(string cScore, bool debugFlag) {
			string debugStr = "- CScore: " + cScore;
			Println(debugStr, debugFlag);
		}
		// frequency score
		public static void PrintFScore(string fScore, bool debugFlag) {
			string debugStr = "- FScore: " + fScore;
			Println(debugStr, debugFlag);
		}
		// cSpell score: all
		public static void PrintScore(string score, bool debugFlag) {
			string debugStr = "- Score: " + score;
			Println(debugStr, debugFlag);
		}
	}

}
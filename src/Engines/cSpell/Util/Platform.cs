using System;

namespace SpellChecker.Engines.cSpell.Util {
	/// <summary>
	///***************************************************************************
	/// This class checks the type of platform.
	/// 
	/// <para><b>History:</b>
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ***************************************************************************
	/// </para>
	/// </summary>
	public class Platform {
		// private constructor
		private Platform() { }
		// public methods
		/*
		 * This method detects if the plaform is a windows bases OS or not.
		 *
		 * @return true if the platform is a window based OS.
		 */
		public static bool IsWindow() {
			bool flag = false;
			var osName = System.Environment.OSVersion;
			if (osName.VersionString.ToLower().IndexOf("windows", StringComparison.Ordinal) > -1) {
				flag = true;
			}
			return flag;
		}
	}

}
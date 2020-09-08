using System.IO;
using System.Text;

namespace SpellChecker.Engines.cSpell.Util {
	/// <summary>
	///***************************************************************************
	/// This class is the output for cSpell.
	/// 
	/// <para><b>History:</b>
	/// 
	/// @author chlu
	/// 
	/// @version    V-2018
	/// ***************************************************************************
	/// </para>
	/// </summary>
	public class Out {
		// public methods
		/// <summary>
		/// Print message to a designated output buffer
		/// </summary>
		/// <param name="outWriter">  the designated outputs </param>
		/// <param name="msg">  massage for printing out </param>
		/// <param name="fileOutput">  a boolean flag for out, true: file, false: std </param>
		/// <param name="toStringFlag">  a boolean flag for sending output to a String
		/// </param>
		/// <exception cref="IOException"> if excaption caught when print out </exception>
		public virtual void Print(StreamWriter outWriter, string msg, bool fileOutput, bool toStringFlag) {
			if (toStringFlag == true) { // output to a String
				if (string.ReferenceEquals(outString_, null)) {
					outString_ = msg;
				} else {
					outString_ += msg;
				}
			} else if (fileOutput == false) { // std output
				outWriter.Write(msg);
				outWriter.Flush();
			} else {
				outWriter.Write(msg);
			}
		}
		/// <summary>
		/// Print message and append a linebreaker to a designated output buffer
		/// </summary>
		/// <param name="outWriter">  the designated outputs </param>
		/// <param name="msg">  massage for printing out </param>
		/// <param name="fileOutFlag">  a boolean flag for out, true: file, false: std </param>
		/// <param name="toStringFlag">  a boolean flag for sending output to a String
		/// </param>
		/// <exception cref="IOException"> if excaption caught when print out </exception>
		public virtual void Println(StreamWriter outWriter, string msg, bool fileOutFlag, bool toStringFlag) {
			if (toStringFlag == true) { // output to a String
				StringBuilder buffer = new StringBuilder();
				if (string.ReferenceEquals(outString_, null)) {
					buffer.Append(msg);
					buffer.Append(System.Environment.NewLine.ToString());
					outString_ = buffer.ToString();
				} else {
					buffer.Append(outString_);
					buffer.Append(msg);
					buffer.Append(System.Environment.NewLine.ToString());
					outString_ = buffer.ToString();
				}
			} else if (fileOutFlag == false) { // std output
				outWriter.WriteLine(msg);
				outWriter.Flush();
			} else { // file output
				outWriter.WriteLine(msg);
			}
		}
		/// <summary>
		/// Reset the output string
		/// </summary>
		public virtual void ResetOutString() {
			outString_ = null;
		}
		/// <summary>
		/// Get the output String
		/// </summary>
		/// <returns> output string </returns>
		public virtual string GetOutString() {
			return outString_;
		}
		// data member
		private string outString_ = null;
	}

}
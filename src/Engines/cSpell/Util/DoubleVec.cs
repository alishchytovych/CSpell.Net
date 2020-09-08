using System;

namespace SpellChecker.Engines.cSpell.Util {
	/// <summary>
	///***************************************************************************
	/// This is a simple implementation of Vector operations in double.
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
	public class DoubleVec {
		public static void MainTest(string[] args) {
			if (args.Length > 0) {
				Console.WriteLine("Usage: java DoubleVec");
				Environment.Exit(0);
			}
			// test
			Test();
		}
		// private constructor
		// init a vector with all element to 0.0
		public DoubleVec(int size) {
			vec_ = new double[size];
		}
		public DoubleVec(double[] vec) {
			// use clone, not = to avoid the assign the reference
			vec_ = (double[]) vec.Clone();
		}
		// index start from 0
		public virtual double GetElement(int index) {
			double @out = 0.0d;
			if ((vec_ != null) && ((index >= 0) && (index < vec_.Length))) {
				@out = vec_[index];
			}
			return @out;
		}
		public virtual void SetElement(double element, int index) {
			if ((vec_ != null) && ((index >= 0) && (index < vec_.Length))) {
				vec_[index] = element;
			}
		}
		public virtual double[] GetVec() {
			return vec_;
		}
		public virtual int GetSize() {
			return vec_.Length;
		}
		public virtual void Add(DoubleVec vec) {
			if ((vec != null) && (GetSize() == vec.GetSize())) {
				for (int i = 0; i < GetSize(); i++) {
					vec_[i] = vec_[i] + vec.GetElement(i);
				}
			}
		}
		public virtual void Minus(DoubleVec vec) {
			if ((vec != null) && (GetSize() == vec.GetSize())) {
				for (int i = 0; i < GetSize(); i++) {
					vec_[i] = vec_[i] - vec.GetElement(i);
				}
			}
		}
		public virtual void Divide(int num) {
			if (num != 0) {
				for (int i = 0; i < GetSize(); i++) {
					vec_[i] = vec_[i] / num;
				}
			}
		}
		// public methods
		public virtual string ToString() {
			string outStr = "[";
			for (int i = 0; i < vec_.Length; i++) {
				if (i == vec_.Length - 1) {
					outStr += vec_[i] + "]";
				} else {
					outStr += vec_[i] + ", ";
				}
			}
			return outStr;
		}

		// private methods
		private static void Test() {
			DoubleVec vec = new DoubleVec(5);
			Console.WriteLine("- size: " + vec.GetSize());
			Console.WriteLine("- vec: " + vec.ToString());
			double[] a1 = new double[] { 1.0d, 2.0d, 3.0d };
			DoubleVec v1 = new DoubleVec(a1);
			Console.WriteLine("- size: " + v1.GetSize());
			Console.WriteLine("- v1: " + v1.ToString());
			a1[0] = 5.0d;
			Console.WriteLine("- v1 (check ref): " + v1.ToString());
			double[] a2 = new double[] { 3.0d, 5.0d, 3.0d };
			DoubleVec v2 = new DoubleVec(a2);
			Console.WriteLine("- v2: " + v2.ToString() + ", size:" + v2.GetSize());
			v2.Add(v1);
			Console.WriteLine("- Add, v1:  " + v1.ToString());
			Console.WriteLine("- Add, v2: " + v2.ToString());
			v2.Minus(v1);
			Console.WriteLine("- Minus, v1:  " + v1.ToString());
			Console.WriteLine("- Minus, v2: " + v2.ToString());
			v2.Divide(4);
			Console.WriteLine("- Div, v1: " + v1.ToString());
			Console.WriteLine("- Div, v2: " + v2.ToString());
		}
		// data member
		private double[] vec_ = null;
	}

}
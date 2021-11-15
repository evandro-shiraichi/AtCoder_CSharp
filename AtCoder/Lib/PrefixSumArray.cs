using AtCoder.ABC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AtCoder.Lib {

	class PrefixSumArray<T> : IEnumerable<T>
		where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
		static readonly IMathArithmeticOperator<T> math_ = IMathArithmeticOperator<T>.GetOperator();

		int length_;
		public int Count {
			get { return length_; }
		}

		T[] array_;

		public T this[int i] {
			get {
				if (i < 0 || i >= length_)
					throw new ArgumentOutOfRangeException();

				return array_[i];
			}

			set {
				if (i < 0 || i >= length_)
					throw new ArgumentOutOfRangeException();

				array_[i] = value;
			}
		}

		public PrefixSumArray(int length) : this(new T[length]) { }

		public PrefixSumArray(T[] array) {
			length_ = array.Length;
			array_ = array;
		}

		public void Update() {
			for (int i = 1; i < length_; i++)
				array_[i] = math_.Add(array_[i], array_[i - 1]);
		}

		public IEnumerator<T> GetEnumerator() {
			return ((IEnumerable<T>)array_).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return array_.GetEnumerator();
		}
	}

	class PrefixSumMatrix<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
		static readonly IMathArithmeticOperator<T> math_ = IMathArithmeticOperator<T>.GetOperator();

		int h_;
		int w_;
		T[,] array_;

		public T this[int i, int j] {
			get {
				if (i < 0 || i >= h_ || j < 0 || j >= w_)
					throw new ArgumentOutOfRangeException();

				return array_[i, j];
			}

			set {
				if (i < 0 || i >= h_ || j < 0 || j >= w_)
					throw new ArgumentOutOfRangeException();

				array_[i, j] = value;
			}
		}

		public PrefixSumMatrix(int h, int w) : this(h, w, new T[h, w]) { }

		public PrefixSumMatrix(int h, int w, T[,] map) {
			h_ = h;
			w_ = w;
			array_ = map;
		}

		public void Update() {
			for (int i = 0; i < h_; i++)
				for (int j = 1; j < w_; j++)
					array_[i, j] = math_.Add(array_[i, j], array_[i, j - 1]);

			for (int i = 1; i < h_; i++)
				for (int j = 0; j < w_; j++)
					array_[i, j] = math_.Add(array_[i, j], array_[i - 1, j]);
		}
	}

}

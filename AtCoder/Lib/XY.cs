using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace AtCoder.Lib
{

	class MinusIndexer<T>
		: IEnumerable<T>
	{
		private T[] array_;
		private int center_;

		public MinusIndexer(int n)
		{
			center_ = n + 1;
			array_ = new T[center_ * 2 + 1];
		}

		public int MaxRange
		{
			get
			{
				return center_ - 1;
			}
		}

		public int MinRange
		{
			get
			{
				return -center_ + 1;
			}
		}

		public int Length
		{
			get
			{
				return array_.Length;
			}
		}

		public T this[int index]
		{
			get
			{
				return array_[index + center_];
			}

			set
			{
				array_[index + center_] = value;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() { return array_.GetEnumerator(); }

		public IEnumerator<T> GetEnumerator() { return GetEnumerator(); }
	}

	struct XY<T>
	{
		public T X;
		public T Y;

		public XY(T[] xy)
		{
			X = xy[0];
			Y = xy[1];
		}

		public XY(T x, T y)
		{
			X = x;
			Y = y;
		}

		public override bool Equals(object obj)
		{
			if (obj is XY<T> xy) {
				return X.Equals(xy.X) && Y.Equals(xy.Y);
			} else {
				return false;
			}
		}

		public override int GetHashCode()
		{
			return 17 * X.GetHashCode() + Y.GetHashCode();
		}
	}
}

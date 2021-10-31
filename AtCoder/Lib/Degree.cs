using System;
using System.Collections.Generic;
using System.Text;
using AtCoder.ABC;

namespace AtCoder.Lib {

	struct Degree<T> : IComparable<Degree<T>> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
		private static IMathArithmeticOperator<T> op_ = IMathArithmeticOperator<T>.GetOperator();

		private readonly int orthant;

		public readonly T X;

		public readonly T Y;

		public readonly T CenterX;

		public readonly T CenterY;

		public Degree(T x, T y) {
			op_ = IMathArithmeticOperator<T>.GetOperator();

			X = x;
			Y = y;

			CenterX = default;
			CenterY = default;

			orthant = Initialize(CenterX, CenterY, X, Y);
		}

		public Degree(T centerX, T centerY, T x, T y) {
			op_ = IMathArithmeticOperator<T>.GetOperator();

			X = x;
			Y = y;

			CenterX = centerX;
			CenterY = centerY;

			orthant = Initialize(CenterX, CenterY, X, Y);
		}

		private static int Initialize(T centerX, T centerY, T x, T y) {
			if (x.Equals(centerX) && y.Equals(centerY))
				return 0;
			else if (y.CompareTo(centerY) >= 0 && x.CompareTo(centerX) > 0)
				return 1;
			else if (y.CompareTo(centerY) > 0 && x.CompareTo(centerX) <= 0)
				return 2;
			else if (y.CompareTo(centerY) <= 0 && x.CompareTo(centerX) < 0)
				return 3;
			else
				return 4;
		}

		public int CompareTo(Degree<T> other) {
			if (orthant != other.orthant)
				return orthant.CompareTo(other.orthant);

			var (x1, y1, x2, y2) = (X, Y, other.X, other.Y);

			if (orthant == 2)
				(x1, y1, x2, y2) = (y1, op_.Negate(x1), y2, op_.Negate(x2));
			else if (orthant == 3)
				(x1, y1, x2, y2) = (op_.Negate(x1), op_.Negate(y1), op_.Negate(x2), op_.Negate(y2));
			else if (orthant == 4)
				(x1, y1, x2, y2) = (op_.Negate(y1), x1, op_.Negate(y2), x2);
			else if (orthant != 1)
				throw new Exception();

			return op_.Multiply(y1, x2).CompareTo(op_.Multiply(y2, x1));
		}
	}
}

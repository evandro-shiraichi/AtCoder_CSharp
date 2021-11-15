using AtCoder.ABC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtCoder.Lib {

	static class ConvexHull {
		public static IEnumerable<Point> DoConvexHull(IEnumerable<Point> points) {
			var sortedPoints = points.OrderBy(x => x).ToArray();
			var upper = DoUpperConvexHullCore(sortedPoints).ToArray()[..^1];
			var lower = DoLowerConvexHullCore(sortedPoints);

			return upper.Concat(lower);
		}

		public static IEnumerable<Point> DoUpperConvexHull(IEnumerable<Point> points) {
			var sortedPoints = points.OrderBy(x => x).ToArray();
			return DoUpperConvexHullCore(sortedPoints);
		}

		public static IEnumerable<Point> DoLowerConvexHull(IEnumerable<Point> points) {
			var sortedPoints = points.OrderBy(x => x).ToArray();
			return DoLowerConvexHullCore(sortedPoints);
		}

		static IEnumerable<Point> DoUpperConvexHullCore(IList<Point> sortedPoints) {
			return DoConvexHullCore(sortedPoints, PointHelper.LEFT).Reverse();
		}

		static IEnumerable<Point> DoLowerConvexHullCore(IList<Point> sortedPoints) {
			return DoConvexHullCore(sortedPoints, PointHelper.RIGHT);
		}

		static IEnumerable<Point> DoConvexHullCore(IList<Point> sortedPoints, int turnDirection) {
			var stack = new Stack<Point>();
			stack.Push(sortedPoints[0]);
			stack.Push(sortedPoints[1]);

			for (int i = 2; i < sortedPoints.Count; i++) {
				Point lastOnePoint;
				Point lastTwoPoint;
				var point = sortedPoints[i];

				do {
					lastOnePoint = stack.Pop();
					lastTwoPoint = stack.Pop();
					if (PointHelper.ISP(lastTwoPoint, lastOnePoint, point) == turnDirection) {
						stack.Push(lastTwoPoint);
					} else {
						stack.Push(lastTwoPoint);
						stack.Push(lastOnePoint);
						break;
					}
				} while (stack.Count >= 2);

				stack.Push(point);
			}

			return stack;
		}
	}

	static class PointHelper {
		public static readonly int LEFT = 1;

		public static readonly int RIGHT = -1;

		public static readonly int CAB = -2;

		public static readonly int ABC = 2;

		public static readonly int ACB = 0;

		public static double Sgn(this double a) {
			return (a < -Constants.Epsilon) ? -1 : (a > Constants.Epsilon) ? 1 : 0;
		}

		public static int ISP(Point a, Point b, Point c) {
			var cross = (b - a).CrossProduct(c - a).Sgn();
			if (cross > 0)
				return LEFT;
			else if (cross < 0)
				return RIGHT;
			else if ((b - a).InnerProduct(c - a).Sgn() < 0)
				return CAB;
			else if ((a - b).InnerProduct(c - b).Sgn() < 0)
				return ABC;
			else
				return ACB;
		}

		public static bool IsLineCrossing(Point a, Point b, Point c, Point d) {
			return (b - a).CrossProduct(c - d).Sgn() != 0;
		}

		public static bool IsLineParallel(Point a, Point b, Point c, Point d) {
			if (IsLineCrossing(a, b, c, d))
				return false;
			else
				return (b - a).CrossProduct(c - a).Sgn() != 0;
		}

		public static bool IsSameLine(Point a, Point b, Point c, Point d) {
			return IsLineCrossing(a, b, c, d) == false && IsLineParallel(a, b, c, d) == false;
		}

		public static bool IsSegmentLineCrossing(Point a, Point b, Point c, Point d) {
			return ISP(a, b, c) * ISP(a, b, d) <= 0 && ISP(c, d, a) * ISP(c, d, b) <= 0;
		}

		public static bool IsSegmentLineCrossingExternal(Point a, Point b, Point c, Point d) {
			return ISP(a, b, c) * ISP(a, b, d) < 0 && ISP(c, d, a) * ISP(c, d, b) < 0;
		}

		public static Point Intersection(Point a, Point b, Point c, Point d) {
			var t = (((b - a) * (c - a)).CrossProduct(d - c) / (b - a).CrossProduct(d - c));
			return (a.X + t, a.Y + t);
		}

		public static double DistanceBetweenLineAndPoint(Point a, Point b, Point c) {
			return Math.Abs((c - a).CrossProduct(b - a) / (b - a).GetDistance());
		}

		public static double DistanceBetweenSegmentLineAndPoint(Point a, Point b, Point c) {
			if ((b - a).InnerProduct(c - a).Sgn() < 0) {
				return Math.Min((c - a).GetDistance(), (c - b).GetDistance());
			}

			return DistanceBetweenLineAndPoint(a, b, c);
		}

		public static double GetSignedArea(params Point[] points) {
			if (points.Length < 3)
				throw new InvalidOperationException();

			var area = 0.0;

			for (int i = 0; i < points.Length - 1; i++) {
				area += points[i].CrossProduct(points[i + 1]);
			}

			area += points.Last().CrossProduct(points.First());

			return area / 2.0;
		}

		public static double GetArea(params Point[] points) {
			return Math.Abs(GetSignedArea(points));
		}
	}

	struct Point : IComparable, IComparable<Point>, IEquatable<Point>, IFormattable {
		public double X;
		public double Y;

		public Point(double x, double y) {
			X = x;
			Y = y;
		}

		public bool Equals(Point other) {
			return (X - other.X).Sgn() == 0 && (Y - other.Y).Sgn() == 0;
		}

		public int CompareTo(object obj) {
			if (obj is Point pt) {
				return CompareTo(pt);
			}

			throw new NotImplementedException();
		}

		public int CompareTo(Point other) {
			if ((X - other.X).Sgn() == 0) {
				return (Y - other.Y).Sgn().CompareTo(0);
			}

			return (X - other.X).Sgn().CompareTo(0);
		}

		public static bool operator !=(Point a, Point b) {
			return !a.Equals(b);
		}

		public static bool operator ==(Point a, Point b) {
			return a.Equals(b);
		}

		public static bool operator <(Point a, Point b) {
			return a.CompareTo(b) < 0;
		}

		public static bool operator <=(Point a, Point b) {
			return a.CompareTo(b) <= 0;
		}

		public static bool operator >(Point a, Point b) {
			return a.CompareTo(b) > 0;
		}

		public static bool operator >=(Point a, Point b) {
			return a.CompareTo(b) >= 0;
		}

		private double Determinant(Point p) {
			return (X * p.Y) - (Y * p.X);
		}

		public double CrossProduct(Point p) {
			return Determinant(p);
		}

		private double DotProduct(Point p) {
			return (X * p.X) + (Y * p.Y);
		}

		public double InnerProduct(Point p) {
			return DotProduct(p);
		}

		public double GetArg() {
			return Math.Atan2(Y, X);
		}

		public double GetDistance2() {
			return GetDistance2((0, 0));
		}

		public double GetDistance2(Point p) {
			var x = Math.Abs(X - p.X);
			var y = Math.Abs(Y - p.Y);

			return (x * x) + (y * y);
		}

		public double GetDistance() {
			return GetDistance((0, 0));
		}

		public double GetDistance(Point p) {
			return Math.Sqrt(GetDistance2(p));
		}

		public static Point operator +(Point p1, Point p2) {
			return (p1.X + p2.X, p1.Y + p2.Y);
		}

		public static Point operator +(Point p1, double d) {
			return p1 + (d, d);
		}

		public static Point operator +(double d, Point p1) {
			return (d, d) + p1;
		}

		public static Point operator -(Point p1, Point p2) {
			return (p1.X - p2.X, p1.Y - p2.Y);
		}

		public static Point operator -(Point p1, double d) {
			return p1 - (d, d);
		}

		public static Point operator -(double d, Point p1) {
			return (d, d) - p1;
		}

		public static Point operator *(Point p1, double k) {
			return (p1.X * k, p1.Y * k);
		}

		public static Point operator *(Point p1, Point p2) {
			return ((p1.X * p2.X) - (p1.Y * p2.Y), (p1.X * p2.Y) + (p1.Y * p2.X));
		}

		public static Point operator *(double k, Point p1) {
			return p1 * k;
		}

		public static Point operator /(Point p1, Point p2) {
			return (p1.X / p2.X, p1.Y / p2.Y);
		}

		public static Point operator -(Point p) {
			return (-p.X, -p.Y);
		}

		public override string ToString() {
			return $"({X}, {Y})";
		}

		public string ToString(string format, IFormatProvider formatProvider) {
			return $"{X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}";
		}

		public override int GetHashCode() {
			return HashCode.Combine(X, Y);
		}

		public override bool Equals(object obj) {
			if (obj is Point pt)
				return this == pt;
			else
				return false;
		}

		public static implicit operator Point((double x, double y) point) {
			return new Point(point.x, point.y);
		}

		public static implicit operator (double x, double y)(Point point) {
			return (point.X, point.Y);
		}
	}
}

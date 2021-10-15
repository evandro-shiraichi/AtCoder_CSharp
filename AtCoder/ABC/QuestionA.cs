using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.RegularExpressions;

namespace AtCoder.ABC {
	class QuestionA {
		public static void Main(string[] args) {
			var scanner = new Scanner();
		}
	}

	class PrefixSumArray<T> {
		MathProvider<T> math_;

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

		public PrefixSumArray(int length) : this(length, new T[length]) { }

		public PrefixSumArray(int length, T[] array) {
			if (typeof(T) == typeof(int))
				math_ = new IntMathProvider() as MathProvider<T>;
			else if (typeof(T) == typeof(long))
				math_ = new LongMathProvider() as MathProvider<T>;
			else if (typeof(T) == typeof(double))
				math_ = new DoubleMathProvider() as MathProvider<T>;

			length_ = length;
			array_ = array;
		}

		public void Update() {
			for (int i = 1; i < length_; i++)
				array_[i] = math_.Add(array_[i], array_[i - 1]);
		}
	}

	class PrefixSumMatrix<T> {
		MathProvider<T> math_;

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
			if (typeof(T) == typeof(int))
				math_ = new IntMathProvider() as MathProvider<T>;
			else if (typeof(T) == typeof(long))
				math_ = new LongMathProvider() as MathProvider<T>;
			else if (typeof(T) == typeof(double))
				math_ = new DoubleMathProvider() as MathProvider<T>;

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

	static class HelperExtensions {
		public static void UpdateMax(this ref int max, int val) {
			max = Math.Max(max, val);
		}
		public static void UpdateMax(this ref long max, long val) {
			max = Math.Max(max, val);
		}
		public static void UpdateMax(this ref double max, double val) {
			max = Math.Max(max, val);
		}

		public static void UpdateMin(this ref int max, int val) {
			max = Math.Min(max, val);
		}
		public static void UpdateMin(this ref long max, long val) {
			max = Math.Min(max, val);
		}
		public static void UpdateMin(this ref double max, double val) {
			max = Math.Min(max, val);
		}
	}

	public static class ArrayHelper {
		public static T[] CreateArray<T>(int w) {
			var array = new T[w];
			return array;
		}

		public static int[] Fill<T>(this int[] array, int intialValue) {
			for (int i = 0; i < array.Length; i++)
				array[i] = intialValue;

			return array;
		}

		public static long[] Fill<T>(this long[] array, long intialValue) {
			for (int i = 0; i < array.Length; i++)
				array[i] = intialValue;

			return array;
		}

		public static double[] Fill<T>(this double[] array, double intialValue) {
			for (int i = 0; i < array.Length; i++)
				array[i] = intialValue;

			return array;
		}

		public static T[] Fill<T>(this T[] array, Func<T> intialValue) {
			for (int i = 0; i < array.Length; i++)
				array[i] = intialValue();

			return array;
		}

		public static int[] CreateArrayFilled<T>(int w, int initialValue) {
			return Fill<int>(CreateArray<int>(w), initialValue);
		}

		public static long[] CreateArrayFilled<T>(int w, long initialValue) {
			return Fill<long>(CreateArray<long>(w), initialValue);
		}

		public static double[] CreateArrayFilled<T>(int w, double initialValue) {
			return Fill<double>(CreateArray<double>(w), initialValue);
		}

		public static T[] CreateArrayFilled<T>(int w, Func<T> initialValue) {
			return Fill(CreateArray<T>(w), initialValue);
		}

		public static T[][] CreateMap<T>(int h, int w) {
			var map = new T[h][];

			for (int i = 0; i < h; i++)
				map[i] = new T[w];

			return map;
		}

		public static int[][] FillMap<T>(this int[][] map, int initialValue) {
			var h = map.Length;

			for (int i = 0; i < h; i++)
				Fill<int>(map[i], initialValue);

			return map;
		}

		public static long[][] FillMap<T>(this long[][] map, long initialValue) {
			var h = map.Length;

			for (int i = 0; i < h; i++)
				Fill<long>(map[i], initialValue);

			return map;
		}

		public static double[][] FillMap<T>(this double[][] map, double initialValue) {
			var h = map.Length;

			for (int i = 0; i < h; i++)
				Fill<double>(map[i], initialValue);

			return map;
		}

		public static T[][] FillMap<T>(this T[][] map, Func<T> initialValue) {
			var h = map.Length;

			for (int i = 0; i < h; i++)
				Fill(map[i], initialValue);

			return map;
		}

		public static int[,] CreateMapFilled<T>(int h, int w, int initialValue) {
			var map = new int[h, w];

			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
					map[i, j] = initialValue;

			return map;
		}

		public static long[,] CreateMapFilled<T>(int h, int w, long initialValue) {
			var map = new long[h, w];

			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
					map[i, j] = initialValue;

			return map;
		}

		public static double[,] CreateMapFilled<T>(int h, int w, double initialValue) {
			var map = new double[h, w];

			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
					map[i, j] = initialValue;

			return map;
		}

		public static T[,] CreateMapFilled<T>(int h, int w, Func<T> initialValue) {
			var map = new T[h, w];

			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
					map[i, j] = initialValue();

			return map;
		}

		public static void WriteArray<T>(IEnumerable<T> array, string sep) {
			Console.WriteLine(string.Join(sep, array));
		}

		public static void WriteMap<T>(IEnumerable<IEnumerable<T>> map, string sep) {
			foreach (var array in map)
				WriteArray(array, sep);
		}

		public static void WriteArray<T, R>(IEnumerable<T> array, string sep, Func<T, R> converter) {
			Console.WriteLine(string.Join(sep, array.Select(x => converter(x))));
		}

		public static void WriteMap<T, R>(IEnumerable<IEnumerable<T>> map, string sep, Func<T, R> converter) {
			foreach (var array in map)
				WriteArray(array, sep, converter);
		}

		private static int LowerBoundCore<T>(IList<T> array, T lowerValue, Func<T, T, int> comaparer) {
			var (ng, ok) = (-1, array.Count);

			while (Math.Abs(ok - ng) > 1) {
				var c = (ng + ok) / 2;

				if (comaparer(array[c], lowerValue) >= 0) {
					ok = c;
				} else {
					ng = c;
				}
			}

			if (ok == array.Count) {
				ok = -1;
			}

			return ok;
		}

		public static int LowerBound<T>(this IList<T> array, T lowerValue)
			where T : IComparable {
			return LowerBoundCore(array, lowerValue, (x, y) => x.CompareTo(y));
		}

		public static int LowerBound<T>(this IList<T> array, T lowerValue, Func<T, T, int> comparer) {
			return LowerBoundCore(array, lowerValue, comparer);
		}

		public static List<T> LongestIncreasingSubsequense<T>(IList<T> array)
			where T : IComparable {
			var list = new List<T>();

			foreach (var a in array) {
				var index = list.LowerBound(a);

				if (index == -1) {
					list.Add(a);
				} else {
					list[index] = a;
				}
			}

			return list;
		}
	}

	public static class MathHelper {
		public static int GetDist2(int x1, int y1, int x2, int y2) {
			return ((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2));
		}
	}

	class Scanner {
		private readonly char[] delimiter_ = new char[] { ' ' };
		private readonly string filePath_;
		private readonly Func<string> reader_;
		private string[] buf_;
		private int index_;

		public Scanner(string file = "") {
			if (string.IsNullOrWhiteSpace(file))
				reader_ = Console.ReadLine;
			else {
				filePath_ = file;
				var fs = new StreamReader(file);
				reader_ = fs.ReadLine;
			}
			buf_ = new string[0];
			index_ = 0;
		}

		public string NextLine() => reader_();
		public string Next() {
			if (index_ < buf_.Length)
				return buf_[index_++];

			string st = reader_();
			while (st == "")
				st = reader_();

			buf_ = st.Split(delimiter_, StringSplitOptions.RemoveEmptyEntries);
			if (buf_.Length == 0)
				return Next();

			index_ = 0;
			return buf_[index_++];
		}

		public int Int() => int.Parse(Next());
		public long Long() => long.Parse(Next());
		public double Double() => double.Parse(Next());

		public int[] ArrayInt(int N, int add = 0) {
			int[] Array = new int[N];
			for (int i = 0; i < N; i++)
				Array[i] = Int() + add;
			return Array;
		}

		public int[] SortedArrayInt(int N, int add = 0, bool descendingOrder = false) {
			var array = ArrayInt(N, add);

			if (descendingOrder)
				Array.Sort(array, (x, y) => y.CompareTo(x));
			else
				Array.Sort(array);

			return array;
		}

		public T[] ArrayIntTo<T>(int N, Func<int, T> converter, int add = 0) {
			T[] Array = new T[N];
			for (int i = 0; i < N; i++)
				Array[i] = converter(Int() + add);
			return Array;
		}

		public long[] ArrayLong(int N, long add = 0) {
			long[] Array = new long[N];
			for (int i = 0; i < N; i++)
				Array[i] = Long() + add;
			return Array;
		}

		public long[] SortedArrayLong(int N, int add = 0, bool descendingOrder = false) {
			var array = ArrayLong(N, add);

			if (descendingOrder)
				Array.Sort(array, (x, y) => y.CompareTo(x));
			else
				Array.Sort(array);

			return array;
		}

		public T[] ArrayLongTo<T>(int N, Func<long, T> converter, int add = 0) {
			T[] Array = new T[N];
			for (int i = 0; i < N; i++)
				Array[i] = converter(Long() + add);
			return Array;
		}

		public double[] ArrayDouble(int N, double add = 0) {
			double[] Array = new double[N];
			for (int i = 0; i < N; i++)
				Array[i] = Double() + add;
			return Array;
		}

		public double[] SortedArrayDouble(int N, int add = 0, bool descendingOrder = false) {
			var array = ArrayDouble(N, add);

			if (descendingOrder)
				Array.Sort(array, (x, y) => y.CompareTo(x));
			else
				Array.Sort(array);

			return array;
		}

		public T[] ArrayDoubleTo<T>(int N, Func<double, T> converter, int add = 0) {
			T[] Array = new T[N];
			for (int i = 0; i < N; i++)
				Array[i] = converter(Double() + add);
			return Array;
		}

		public string[] ArrayString(int h) {
			var array = new string[h];

			for (int i = 0; i < h; i++)
				array[i] = Next();

			return array;
		}

		public T[] ArrayStringTo<T>(int h, Func<string, T> converter) {
			var array = new T[h];

			for (int i = 0; i < h; i++)
				array[i] = converter(Next());

			return array;
		}

		public int[,] IntMap(int h, int w, int add = 0) {
			var map = new int[h, w];

			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
					map[i, j] = Int() + add;

			return map;
		}

		public T[,] IntMapTo<T>(int h, int w, Func<int, T> converter, int add = 0) {
			var map = new T[h, w];

			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
					map[i, j] = converter(Int() + add);

			return map;
		}

		public long[,] LongMap(int h, int w, int add = 0) {
			var map = new long[h, w];

			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
					map[i, j] = Long() + add;

			return map;
		}

		public T[,] LongMapTo<T>(int h, int w, Func<long, T> converter, int add = 0) {
			var map = new T[h, w];

			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
					map[i, j] = converter(Long() + add);

			return map;
		}

		public double[,] DoubleMap(int h, int w, int add = 0) {
			var map = new double[h, w];

			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
					map[i, j] = Double() + add;

			return map;
		}

		public T[,] DoubleMapTo<T>(int h, int w, Func<double, T> converter, int add = 0) {
			var map = new T[h, w];

			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
					map[i, j] = converter(Double() + add);

			return map;
		}

		public (int, int) Int2(int add = 0) {
			return (Int() + add, Int() + add);
		}

		public (int, int, int) Int3(int add = 0) {
			return (Int() + add, Int() + add, Int() + add);
		}

		public (long, long) Long2(int add = 0) {
			return (Long() + add, Long() + add);
		}

		public (long, long, long) Long3(int add = 0) {
			return (Long() + add, Long() + add, Long() + add);
		}

		public (double, double) Double2(int add = 0) {
			return (Double() + add, Double() + add);
		}

		public (double, double, double) Double3(int add = 0) {
			return (Double() + add, Double() + add, Double() + add);
		}

		public void Save(string text) {
			if (string.IsNullOrWhiteSpace(filePath_))
				return;

			File.WriteAllText(filePath_ + "_output.txt", text);
		}
	}

	class DoubleMathProvider : MathProvider<double> {
		public override double Divide(double a, double b) {
			return a / b;
		}

		public override double Multiply(double a, double b) {
			return a * b;
		}

		public override double Add(double a, double b) {
			return a + b;
		}

		public override double Negate(double a) {
			return -a;
		}
	}

	class IntMathProvider : MathProvider<int> {
		public override int Divide(int a, int b) {
			return a / b;
		}

		public override int Multiply(int a, int b) {
			return a * b;
		}

		public override int Add(int a, int b) {
			return a + b;
		}

		public override int Negate(int a) {
			return -a;
		}
	}

	class LongMathProvider : MathProvider<long> {
		public override long Divide(long a, long b) {
			return a / b;
		}

		public override long Multiply(long a, long b) {
			return a * b;
		}

		public override long Add(long a, long b) {
			return a + b;
		}

		public override long Negate(long a) {
			return -a;
		}
	}

	abstract class MathProvider<T> {
		public abstract T Divide(T a, T b);
		public abstract T Multiply(T a, T b);
		public abstract T Add(T a, T b);
		public abstract T Negate(T a);
		public virtual T Subtract(T a, T b) {
			return Add(a, Negate(b));
		}
	}
}

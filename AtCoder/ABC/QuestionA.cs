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

	class PrefixSumArray<T> : IEnumerable<T>
		where T : IComparable {
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
			else
				throw new ApplicationException();

			length_ = length;
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

	class PrefixSumMatrix<T> where T : IComparable {
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
			else
				throw new ApplicationException();

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

	class PriorityQueue<TObject, TPriority> where TPriority : IComparable<TPriority> {
		private readonly Func<TObject, TPriority> selector_;
		private readonly int reverseFactor;
		private TObject[] heap_;
		int size;

		public TObject Top => heap_[0];
		public int Count {
			get { return size; }
		}

		public PriorityQueue(
			Func<TObject, TPriority> selector,
			bool isReverse = false)
			: this(1024, selector, isReverse) { }

		public PriorityQueue(
			int capacity,
			Func<TObject, TPriority> selector,
			bool isReverse = false) {
			heap_ = new TObject[capacity];
			selector_ = selector;
			reverseFactor = isReverse ? -1 : 1;
		}

		public void Enqueue(TObject x) {
			if (heap_.Length == size) {
				Extend(size * 2);
			}

			int i = size++;

			while (i > 0) {
				int p = (i - 1) / 2;

				if (reverseFactor * selector_(heap_[p]).CompareTo(selector_(x)) >= 0) {
					break;
				}

				heap_[i] = heap_[p];
				i = p;
			}

			heap_[i] = x;
		}

		public TObject Dequeue() {
			TObject ret = heap_[0];

			TObject x = heap_[--size];

			int i = 0;
			while (i * 2 + 1 < size) {
				int a = i * 2 + 1;
				int b = i * 2 + 2;

				if (b < size && reverseFactor * selector_(heap_[b]).CompareTo(selector_(heap_[a])) > 0) {
					a = b;
				}

				if (reverseFactor * selector_(heap_[a]).CompareTo(selector_(x)) <= 0) {
					break;
				}

				heap_[i] = heap_[a];
				i = a;
			}

			heap_[i] = x;

			return ret;
		}

		void Extend(int newSize) {
			var newHeap = new TObject[newSize];

			heap_.CopyTo(newHeap, 0);
			heap_ = newHeap;
		}
	}

	class Dubling<TransitionType, ValueType> {
		private readonly TransitionType[] transitions_;
		private readonly ValueType initialValue_;
		private readonly long max_;
		private readonly Func<ValueType, TransitionType, ValueType> valueConverter_;

		public Dubling(
			long max,
			TransitionType initialTransition,
			ValueType initialValue,
			Func<TransitionType, TransitionType> transitionFunc,
			Func<ValueType, TransitionType, ValueType> valueConverter) {
			initialValue_ = initialValue;
			max_ = max;
			valueConverter_ = valueConverter;
			var powerIndex = max.Get2PowerIndexRoundingOff() + 1;
			transitions_ = new TransitionType[powerIndex];
			transitions_[0] = initialTransition;

			for (int i = 1; i < powerIndex; i++) {
				transitions_[i] = transitionFunc(transitions_[i - 1]);
			}
		}

		public ValueType Query(long i) {
			return QueryCore(initialValue_, i);
		}

		public ValueType QueryCore(ValueType now, long i) {
			if (i > max_ || i < 0)
				throw new Exception();

			if (i == 0)
				return now;

			var powerIndex = i.Get2PowerIndexRoundingOff();
			now = valueConverter_(now, transitions_[powerIndex]);
			return QueryCore(now, i - MathHelper.Pow(2, powerIndex));
		}
	}

	class Flow<T> where T : IComparable {
		MathProvider<T> math_;

		public Flow(int node_size) {
			if (typeof(T) == typeof(int))
				math_ = new IntMathProvider() as MathProvider<T>;
			else if (typeof(T) == typeof(long))
				math_ = new LongMathProvider() as MathProvider<T>;
			else if (typeof(T) == typeof(double))
				math_ = new DoubleMathProvider() as MathProvider<T>;
			else
				throw new ApplicationException();

			V = node_size;
			G = Enumerable.Repeat(0, V).Select(_ => new List<Edge>()).ToList();
			level = Enumerable.Repeat(0, V).ToList();
			iter = Enumerable.Repeat(0, V).ToList();
		}

		private class Edge {
			public Edge(int to, T cap, int rev) {
				To = to;
				Cap = cap;
				Rev = rev;
			}
			public int To { get; set; }
			public T Cap { get; set; }
			public int Rev { get; set; }
		}

		private readonly List<List<Edge>> G;
		private readonly int V;
		private List<int> level;
		private List<int> iter;

		public void AddEdge(int from, int to, T cap) {
			G[from].Add(new Edge(to, cap, G[to].Count));
			G[to].Add(new Edge(from, math_.Zero(), G[from].Count - 1));
		}

		public T MaxFlow(int s, int t) {
			T flow = math_.Zero();
			while (true) {
				BFS(s);
				if (level[t] < 0) { return flow; }
				iter = Enumerable.Repeat(0, V).ToList();
				var f = DFS(s, t, math_.MaxValue());
				while (f.CompareTo(math_.Zero()) > 0) {
					flow = math_.Add(flow, f);
					f = DFS(s, t, math_.MaxValue());
				}
			}
		}

		private void BFS(int s) {
			level = Enumerable.Repeat(-1, V).ToList();
			level[s] = 0;
			var que = new Queue<int>();
			que.Enqueue(s);
			while (que.Count != 0) {
				var v = que.Dequeue();
				for (int i = 0; i < G[v].Count; i++) {
					var e = G[v][i];
					if (e.Cap.CompareTo(math_.Zero()) > 0 && level[e.To] < 0) {
						level[e.To] = level[v] + 1;
						que.Enqueue(e.To);
					}
				}
			}
		}

		private T DFS(int v, int t, T f) {
			if (v == t)
				return f;
			for (int i = iter[v]; i < G[v].Count; i++) {
				iter[v] = i;
				var e = G[v][i];
				if (e.Cap.CompareTo(math_.Zero()) > 0 && level[v] < level[e.To]) {
					var d = DFS(e.To, t, math_.Min(f, e.Cap));
					if (d.CompareTo(math_.Zero()) > 0) {
						e.Cap = math_.Subtract(e.Cap, d);
						G[e.To][e.Rev].Cap = math_.Add(G[e.To][e.Rev].Cap, d);
						return d;
					}
				}
			}
			return math_.Zero();
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

		public static void Swap<T>(ref T a, ref T b) {
			(a, b) = (b, a);
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

		public static int[,] CreateMapFilled(int h, int w, int initialValue) {
			var map = new int[h, w];

			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
					map[i, j] = initialValue;

			return map;
		}

		public static long[,] CreateMapFilled(int h, int w, long initialValue) {
			var map = new long[h, w];

			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
					map[i, j] = initialValue;

			return map;
		}

		public static double[,] CreateMapFilled(int h, int w, double initialValue) {
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

		public static long Pow(long a, long b, long mod = 0L) {
			var temp = a;
			var ans = 1L;

			for (int i = 1; i <= b; i <<= 1) {
				if ((b & i) != 0) {
					ans *= temp;
					if (mod != 0) {
						ans %= mod;
					}
				}

				temp *= temp;
				if (mod != 0) {
					temp %= mod;
				}
			}

			return ans;
		}

		public static int Get2PowerIndexRoundingOff(this long n) {
			var powerIndex = 0;

			while (1 << powerIndex < n) {
				powerIndex++;
			}

			if (1 << powerIndex > n) {
				powerIndex--;
			}

			return powerIndex;
		}

		public static int Get2PowerIndexRoundingOff(this int n) {
			var powerIndex = 0;

			while (1 << powerIndex < n) {
				powerIndex++;
			}

			if (1 << powerIndex > n) {
				powerIndex--;
			}

			return powerIndex;
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

		public override double Zero() {
			return 0.0;
		}

		public override double MaxValue() {
			return double.MaxValue;
		}

		public override double MinValue() {
			return double.MinValue;
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

		public override int Zero() {
			return 0;
		}

		public override int MaxValue() {
			return int.MaxValue;
		}

		public override int MinValue() {
			return int.MinValue;
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

		public override long Zero() {
			return 0;
		}

		public override long MaxValue() {
			return long.MaxValue;
		}

		public override long MinValue() {
			return long.MinValue;
		}
	}

	abstract class MathProvider<T> where T : IComparable { 
		public abstract T Divide(T a, T b);
		public abstract T Multiply(T a, T b);
		public abstract T Add(T a, T b);
		public abstract T Negate(T a);
		public virtual T Subtract(T a, T b) {
			return Add(a, Negate(b));
		}
		public virtual T Min(T a, T b) {
			return a.CompareTo(b) > 0 ? b : a;
		}
		public virtual T Max(T a, T b) {
			return a.CompareTo(b) > 0 ? a : b;
		}
		public abstract T Zero();
		public abstract T MaxValue();
		public abstract T MinValue();
	}
}

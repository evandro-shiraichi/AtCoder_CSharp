using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

	static class Constants {
		public static readonly int Mod = 1000000007;
		public static readonly int InverseMax = 510000;
	}

	class CompressArray<T> : IEnumerable<(long, T)> {
		private readonly SortedSet<long> sortedSet_ = new SortedSet<long>();
		private readonly T default_;
		private Dictionary<int, long> decompressIndexDict_;
		private Dictionary<long, int> compressIndexDict_;

		private T[] array_;

		public int Length {
			get { return array_.Length; }
		}

		public T this[int i] {
			get { return array_[i]; }
			set { array_[i] = value; }
		}

		public CompressArray(IList<long> indexes, T defaultValue = default) {
			default_ = defaultValue;
			foreach(var index in indexes) {
				sortedSet_.Add(index);
			}

			Update();
		}

		public CompressArray(T defaultValue = default) {
			default_ = defaultValue;
		}

		public void AddIndex(long i) {
			sortedSet_.Add(i);
		}

		public void Update() {
			array_ = Enumerable.Repeat(default_, sortedSet_.Count).ToArray();
			compressIndexDict_ = new Dictionary<long, int>();
			decompressIndexDict_ = new Dictionary<int, long>();

			var i = 0;
			foreach (var set in sortedSet_) {
				compressIndexDict_.Add(set, i);
				decompressIndexDict_.Add(i, set);
				i++;
			}
		}

		public int CompressIndex(long index) {
			return compressIndexDict_[index];
		}

		public long DecompressIndex(int index) {
			return decompressIndexDict_[index];
		}

		public IList<T> Decompress(long max = -1) {
			if(max < 0) {
				max = sortedSet_.Last();
			}
			var decompress = Enumerable.Repeat(default_, (int)max).ToArray();

			foreach(var (i, t) in this) {
				decompress[i] = t;
			}

			return decompress;
		}

		public IEnumerator<(long, T)> GetEnumerator() {
			return Enumerable.Range(0, Length).Select(x => (decompressIndexDict_[x], array_[x])).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}
	}

	class PrefixSumArray<T> : IEnumerable<T>
		where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
		readonly IMathArithmeticOperator<T> math_;

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
			math_ = IMathArithmeticOperator<T>.GetOperator();

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
		readonly IMathArithmeticOperator<T> math_;

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
			math_ = IMathArithmeticOperator<T>.GetOperator();

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

	class Dequeue<T> {
		private int _capacity;
		private T[] _array;

		private int _firstIndex = 0;
		private int _lastIndex = 1;

		public Dequeue(int capacity = 16) {
			_capacity = capacity;
			_array = new T[_capacity];
		}

		public T this[int i] {
			get {
				if (i < 0 || i >= Count)
					throw new ArgumentOutOfRangeException();
				return _array[ToIndex(_firstIndex + 1 + i)];
			}
			set {
				if (i < 0 || i >= Count)
					throw new ArgumentOutOfRangeException();
				_array[ToIndex(_firstIndex + 1 + i)] = value;
			}
		}

		public int Count {
			get { return _lastIndex - _firstIndex - 1; }
		}

		public bool Any() {
			return Count > 0;
		}

		private int ToIndex(int index) {
			index %= _capacity;
			if (index < 0)
				index += _capacity;
			return index;
		}

		public void PushBack(T data) {
			if (_capacity == Count)
				Resize();

			_array[ToIndex(_lastIndex++)] = data;
		}

		public void PushFront(T data) {
			if (_capacity == Count)
				Resize();

			_array[ToIndex(_firstIndex--)] = data;
		}

		public T PopBack() {
			if (Any() == false)
				throw new InvalidOperationException();

			var ret = _array[ToIndex(_lastIndex - 1)];
			_lastIndex--;
			return ret;
		}

		public T PopFront() {
			if (Any() == false)
				throw new InvalidOperationException();

			var ret = _array[ToIndex(_firstIndex + 1)];
			_firstIndex++;
			return ret;
		}

		private void Resize() {
			var newArray = new T[_capacity * 2];

			for (int i = _firstIndex; i < _lastIndex - 1; i++) {
				var index = i - _firstIndex;
				newArray[index] = _array[ToIndex(i + 1)];
			}
			_firstIndex = -1;
			_lastIndex = _capacity;
			_capacity *= 2;
			_array = newArray;
		}
	}

	class PermutaionInt {
		List<IList<int>> permuations_;
		readonly int max_;

		public PermutaionInt(int max) {
			max_ = max;
		}

		public IEnumerable<IList<int>> GetAllPermutation() {
			if (permuations_ is null) {
				permuations_ = new List<IList<int>>();
				DFS(new Stack<int>(), new HashSet<int>());
			}

			return permuations_;
		}

		void DFS(Stack<int> nowPerm, HashSet<int> yet) {
			if (nowPerm.Count == max_) {
				permuations_.Add(nowPerm.Reverse().ToArray());
				return;
			}

			for (int i = 0; i < max_; i++) {
				if (yet.Contains(i))
					continue;

				yet.Add(i);
				nowPerm.Push(i);
				DFS(nowPerm, yet);
				nowPerm.Pop();
				yet.Remove(i);
			}
		}
	}

	class PermutationString {
		List<string> permutations_;
		string str_;

		HashSet<string> yetString_ = new HashSet<string>();
		HashSet<int> yetIndex_ = new HashSet<int>();

		public PermutationString(string str) {
			var temp = str.ToArray();
			Array.Sort(temp);
			str_ = new string(temp);
		}

		public IEnumerable<string> GetAllPermutation() {
			if (permutations_ is null) {
				permutations_ = new List<string>();
				DFS("");
			}

			return permutations_;
		}

		void DFS(string nowS) {
			if (yetString_.Contains(nowS))
				return;

			yetString_.Add(nowS);

			if (nowS.Length == str_.Length) {
				permutations_.Add(nowS);
				return;
			}

			for (int i = 0; i < str_.Length; i++) {
				if (yetIndex_.Contains(i))
					continue;

				yetIndex_.Add(i);
				DFS(nowS + str_[i]);
				yetIndex_.Remove(i);
			}
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
			var powerIndex = MathHelper.Get2PowerIndexRoundingOff(max) + 1;
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

			var powerIndex = MathHelper.Get2PowerIndexRoundingOff(i);
			now = valueConverter_(now, transitions_[powerIndex]);
			return QueryCore(now, i - MathHelper.Pow(2, powerIndex));
		}
	}

	class Flow<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
		IMathArithmeticOperator<T> math_;

		public Flow(int node_size) {
			math_ = IMathArithmeticOperator<T>.GetOperator();

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
			G[to].Add(new Edge(from, math_.Zero, G[from].Count - 1));
		}

		public T MaxFlow(int s, int t) {
			T flow = math_.Zero;
			while (true) {
				BFS(s);
				if (level[t] < 0) { return flow; }
				iter = Enumerable.Repeat(0, V).ToList();
				var f = DFS(s, t, math_.MaxValue);
				while (f.CompareTo(math_.Zero) > 0) {
					flow = math_.Add(flow, f);
					f = DFS(s, t, math_.MaxValue);
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
					if (e.Cap.CompareTo(math_.Zero) > 0 && level[e.To] < 0) {
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
				if (e.Cap.CompareTo(math_.Zero) > 0 && level[v] < level[e.To]) {
					var d = DFS(e.To, t, math_.Min(f, e.Cap));
					if (d.CompareTo(math_.Zero) > 0) {
						e.Cap = math_.Substract(e.Cap, d);
						G[e.To][e.Rev].Cap = math_.Add(G[e.To][e.Rev].Cap, d);
						return d;
					}
				}
			}
			return math_.Zero;
		}
	}

	static class DPHelper {
		public static T DoKnapSackDP<T>((int weight, T worth)[] ww, int maxWeight) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
			var op = IMathArithmeticOperator<T>.GetOperator();
			
			var dp = new T[maxWeight + 1];

			for (int i = 0; i < ww.Length; i++) {
				var (Width, Worth) = ww[i];
				var newDp = new T[maxWeight + 1];

				for (int j = 0; j <= maxWeight; j++) {
					var temp1 = j - 1 >= 0 ? newDp[j - 1] : op.Zero;
					var temp2 = j - Width >= 0 ? op.Add(dp[j - Width], Worth) : op.Zero;
					newDp[j] = MathHelper.Max(temp1, dp[j], temp2);
				}

				dp = newDp;
			}

			return dp[maxWeight];
		}

		public static T DoLimitKnapSackDP<T>((int weight, T worth)[] ww, int maxWeight, int limitNum) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
			var op = IMathArithmeticOperator<T>.GetOperator();

			var dp = new T[limitNum + 1, maxWeight + 1];
			var ans = op.Zero;

			for (int k = 0; k < ww.Length; k++) {
				var newDP = new T[limitNum + 1, maxWeight + 1];
				var (weight, worth) = ww[k];
				var max = op.Zero;

				var maxJ = Math.Min(limitNum + 1, k + 1);

				for (int j = 1; j < limitNum + 1; j++) {
					for (int i = 1; i < maxWeight + 1; i++) {
						var temp = i - weight >= 0 ? op.Add(dp[j - 1, i - weight], worth) : op.Zero;
						max = MathHelper.Max(dp[j - 1, i], dp[j, i - 1], dp[j, i], temp);
						newDP[j, i] = max;
					}
				}

				ans.UpdateMax(max);
				dp = newDP;
			}

			return ans;
		}
	}

	static class HelperExtensions {
		public static void UpdateMax<T>(this ref T max, T val) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
			max = MathHelper.Max(max, val);
		}

		public static void UpdateMin<T>(this ref T max, T val) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
			max = MathHelper.Min(max, val);
		}

		public static void Swap<T>(ref T a, ref T b) {
			(a, b) = (b, a);
        }
	}

	static class ArrayHelper {
		public static T[] CreateArray<T>(int w) {
			var array = new T[w];
			return array;
		}

		public static T[] Fill<T>(this T[] array, T intialValue) {
			for (int i = 0; i < array.Length; i++)
				array[i] = intialValue;

			return array;
		}

		public static T[] Fill<T>(this T[] array, Func<T> intialValue) {
			for (int i = 0; i < array.Length; i++)
				array[i] = intialValue();

			return array;
		}

		public static T[] CreateArrayFilled<T>(int w, T initialValue) {
			return Fill(CreateArray<T>(w), initialValue);
		}

		public static T[] CreateArrayFilled<T>(int w, Func<T> initialValue) {
			return Fill(CreateArray<T>(w), initialValue);
		}

		public static T[][] CreateJagMap<T>(int h, int w) {
			var map = new T[h][];

			for (int i = 0; i < h; i++)
				map[i] = new T[w];

			return map;
		}

		public static T[][] FillJagMap<T>(this T[][] map, T initialValue) {
			var h = map.Length;

			for (int i = 0; i < h; i++)
				Fill(map[i], initialValue);

			return map;
		}

		public static T[][] FillJagMap<T>(this T[][] map, Func<T> initialValue) {
			var h = map.Length;

			for (int i = 0; i < h; i++)
				Fill(map[i], initialValue);

			return map;
		}

		public static T[,] CreateMapFilled<T>(int h, int w, T initialValue) {
			var map = new T[h, w];

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

		public static void WriteMap<T>(IEnumerable<IEnumerable<T>> map, string sep) {
			foreach (var array in map)
				WriteArray(array, sep);
		}

		public static void WriteArray<T>(IEnumerable<T> array, string sep) {
			Console.WriteLine(string.Join(sep, array));
		}

		public static void WriteMap<T, R>(IEnumerable<IEnumerable<T>> map, string sep, Func<T, R> converter) {
			foreach (var array in map)
				WriteArray(array, sep, converter);
		}

		public static void WriteArray<T, R>(IEnumerable<T> array, string sep, Func<T, R> converter) {
			Console.WriteLine(string.Join(sep, array.Select(x => converter(x))));
		}

		private static int LowerBound<T>(this IList<T> array, T lowerValue, Func<T, T, int> comaparer) {
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
			return array.LowerBound(lowerValue, (x, y) => x.CompareTo(y));
		}

		public static List<T> LongestIncreasingSubsequense<T>(IList<T> array, Func<T, T, int> comaparer) {
			var list = new List<T>();

			foreach (var a in array) {
				var index = list.LowerBound(a, comaparer);

				if (index == -1) {
					list.Add(a);
				} else {
					list[index] = a;
				}
			}

			return list;
		}

		public static List<T> LongestIncreasingSubsequense<T>(IList<T> array) where T : IComparable {
			return LongestIncreasingSubsequense(array, (x, y) => x.CompareTo(y));
		}
	}

	static class MathHelper {
		public static T GetDist2<T>(T x1, T y1, T x2, T y2) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
			var op = IMathArithmeticOperator<T>.GetOperator();
			return op.Add(op.Multiply(op.Substract(x1, x2), op.Substract(x1, x2)), op.Multiply(op.Substract(y1, y2), op.Substract(y1, y2)));
		}

		public static bool LineSegmentIntersectionJudge<T>(T x1, T y1, T x2, T y2, T x3, T y3, T x4, T y4) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
			var op = IMathArithmeticOperator<T>.GetOperator();
			T s, t;
			s = op.Substract(op.Multiply(op.Substract(x1, x2), op.Substract(y3, y1)), op.Multiply(op.Substract(y1, y2), op.Substract(x3, x1)));
			t = op.Substract(op.Multiply(op.Substract(x1, x2), op.Substract(y4, y1)), op.Multiply(op.Substract(y1, y2), op.Substract(x4, x1)));

			if (op.Multiply(s, t).CompareTo(op.Zero) > 0)
				return false;

			s = op.Substract(op.Multiply(op.Substract(x3, x4), op.Substract(y1, y3)), op.Multiply(op.Substract(y3, y4), op.Substract(x1, x3)));
			t = op.Substract(op.Multiply(op.Substract(x3, x4), op.Substract(y2, y3)), op.Multiply(op.Substract(y3, y4), op.Substract(x2, x3)));

			return op.Multiply(s, t).CompareTo(op.Zero) <= 0;
		}

		public static T Pow<T>(T a, T b) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
			var op = IMathArithmeticOperator<T>.GetOperator();
			return op.Pow(a, b);
		}

		public static int Get2PowerIndexRoundingOff<T>(T n) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
			var op = IMathArithmeticOperator<T>.GetOperator();
			var powerIndex = 0;

			while (op.BitShiftLeft(op.One, powerIndex).CompareTo(n) < 0) {
				powerIndex++;
			}

			if (op.BitShiftLeft(op.One, powerIndex).CompareTo(n) > 0) {
				powerIndex--;
			}

			return powerIndex;
		}

		public static T Max<T>(T a, T b) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
			if(a.CompareTo(b) > 0) {
				return a;
			} else {
				return b;
			}
		}

		public static T Max<T>(T a, T b, T c) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
			return Max(a, Max(b, c));
		}

		public static T Max<T>(T a, T b, T c, T d) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
			return Max(d, Max(a, Max(b, c)));
		}

		public static T Min<T>(T a, T b) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
			if (a.CompareTo(b) > 0) {
				return b;
			} else {
				return a;
			}
		}

		public static T Min<T>(T a, T b, T c) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
			return Min(a, Min(b, c));
		}

		public static T Min<T>(T a, T b, T c, T d) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
			return Min(d, Min(a, Min(b, c)));
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

	interface IMathArithmeticOperator<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
		public static IMathArithmeticOperator<T> GetOperator() {
			var type = typeof(T);
			if (type == typeof(ModInt)) {
				return new ModIntOperaor() as IMathArithmeticOperator<T>;
			}
			if (type == typeof(int)) {
				return new IntOperator() as IMathArithmeticOperator<T>;
			}
			if (type == typeof(long)) {
				return new LongOperator() as IMathArithmeticOperator<T>;
			}
			if (type == typeof(double)) {
				return new DoubleOperator() as IMathArithmeticOperator<T>;
			}

			return null;
		}

		public T MaxValue { get; }
		public T MinValue { get; }
		public T Zero { get; }
		public T One { get; }

		public T Add(T x, T y);
		public T Substract(T x, T y);
		public T Multiply(T x, T y);
		public T Divide(T x, T y);
		public T Pow(T x, T y);
		public T Mod(T x, T y);

		public T Negate(T x);

		public T Choose(T n, T r);
		public T Factorial(T x);

		public T BitShiftLeft(T x, int y);
		public T BitShiftRight(T x, int y);

		public T Max(T x, T y);
		public T Min(T x, T y);
	}

	static class ChooseHelper<T, Top>
			where Top : struct, IMathArithmeticOperator<T>
			where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
		public static Dictionary<(T n, T r), T> ChooseDictionary = new Dictionary<(T n, T r), T>();

		public static T Choose(T n, T r) {
			var op = new Top();

			if ((n.CompareTo(r) < 0) || (n.CompareTo(op.Zero) < 0) || r.CompareTo(op.Zero) < 0) {
				throw new Exception();
			}

			r = op.Min(r, op.Substract(n, r));

			if (ChooseDictionary.ContainsKey((n, r)))
				return ChooseDictionary[(n, r)];

			if (r.Equals(op.One)) {
				return n;
			} else if (r.Equals(op.Zero)) {
				return op.One;
			}

			var tempA = Choose(op.Substract(n, op.One), r);
			var tempB = Choose(op.Substract(n, op.One), op.Substract(r, op.One));
			var temp = op.Add(tempA, tempB);

			ChooseDictionary.Add((n, r), temp);

			return temp;
		}
	}

	static class ModIntHelper {
		public static readonly ModIntOperaor ModOperator = new ModIntOperaor();

		public static ModInt[] FactorialArray;
		public static ModInt[] FactorialInverseArray;
		public static ModInt[] InverseArray;

		static void ModChooseInit() {
			var fac = new ModInt[Constants.InverseMax];
			var finv = new ModInt[Constants.InverseMax];
			var inv = new ModInt[Constants.InverseMax];

			fac[0] = fac[1] = 1;
			finv[0] = finv[1] = 1;
			inv[1] = 1;
			for (int i = 2; i < Constants.InverseMax; i++) {
				fac[i] = fac[i - 1] * i;
				inv[i] = Constants.Mod - inv[Constants.Mod % i] * (Constants.Mod / i);
				finv[i] = finv[i - 1] * inv[i];
			}

			FactorialArray = fac;
			FactorialInverseArray = finv;
			InverseArray = inv;
		}

		public static long Choose(long n, long r) {
			if (n > Constants.InverseMax)
				throw new Exception();

			if (FactorialArray is null)
				ModChooseInit();

			if (n < r || n < 0 || r < 0)
				return 0;

			return FactorialArray[n] * (FactorialInverseArray[r] * FactorialInverseArray[n - r]);
		}

		public static long ModInv(long a) {
			return ModOperator.Pow(a, Constants.Mod - 2);
		}

		// 拡張ユークリッド版
		public static long ModInv(long a, long m) {
			var (b, u, v) = (m, 1L, 0L);

			while (b > 0) {
				var t = a / b;
				a -= t * b;
				(a, b) = (b, a);
				u -= t * v;
				(u, v) = (v, u);
			}

			u %= m;

			if (u < 0)
				u += m;

			return u;
		}
	}

	struct ModInt : IComparable, IComparable<ModInt>, IConvertible, IEquatable<ModInt>, IFormattable {
		public long Value;

		public ModInt(long val) {
			Value = (int)(val % Constants.Mod);
		}

		public override bool Equals(object obj) {
			if (obj is ModInt modInt)
				return Equals(modInt.Value);
			else if (obj is int integer)
				return Value == integer;
			else if (obj is long longer)
				return Value == longer;
			else
				return false;
		}

		public override int GetHashCode() {
			return Value.GetHashCode();
		}

		public override string ToString() {
			return Value.ToString();
		}

		public int CompareTo(object obj) {
			if (obj is ModInt modInt)
				return Value.CompareTo(modInt.Value);
			else if (obj is int integer)
				return Value.CompareTo(integer);
			else if (obj is long longer)
				return Value.CompareTo(longer);
			else if (obj is double doubler)
				return Value.CompareTo(doubler);
			else
				throw new Exception();
		}

		public int CompareTo(ModInt other) {
			return Value.CompareTo(other.Value);
		}

		public TypeCode GetTypeCode() {
			return TypeCode.Object;
		}

		public bool ToBoolean(IFormatProvider provider) {
			return Convert.ToBoolean(Value, provider);
		}

		public byte ToByte(IFormatProvider provider) {
			return Convert.ToByte(Value, provider);
		}

		public char ToChar(IFormatProvider provider) {
			return Convert.ToChar(Value, provider);
		}

		public DateTime ToDateTime(IFormatProvider provider) {
			return Convert.ToDateTime(Value, provider);
		}

		public decimal ToDecimal(IFormatProvider provider) {
			return Convert.ToDecimal(Value, provider);
		}

		public double ToDouble(IFormatProvider provider) {
			return Convert.ToDouble(Value, provider);
		}

		public short ToInt16(IFormatProvider provider) {
			return Convert.ToInt16(Value, provider);
		}

		public int ToInt32(IFormatProvider provider) {
			return Convert.ToInt32(Value, provider);
		}

		public long ToInt64(IFormatProvider provider) {
			return Convert.ToInt64(Value, provider);
		}

		public sbyte ToSByte(IFormatProvider provider) {
			return Convert.ToSByte(Value, provider);
		}

		public float ToSingle(IFormatProvider provider) {
			return Convert.ToSingle(Value, provider);
		}

		public string ToString(IFormatProvider provider) {
			return Convert.ToString(Value, provider);
		}

		public object ToType(Type conversionType, IFormatProvider provider) {
			throw new InvalidCastException();
		}

		public ushort ToUInt16(IFormatProvider provider) {
			return Convert.ToUInt16(Value, provider);
		}

		public uint ToUInt32(IFormatProvider provider) {
			return Convert.ToUInt32(Value, provider);
		}

		public ulong ToUInt64(IFormatProvider provider) {
			return Convert.ToUInt64(Value, provider);
		}

		public bool Equals( ModInt other) {
			return Value.Equals(other.Value);
		}

		public string ToString(string format, IFormatProvider formatProvider) {
			return Value.ToString(format, formatProvider);
		}

		public static ModInt operator +(ModInt x, long y) {
			return new ModInt(ModIntHelper.ModOperator.Add(x, y));
		}

		public static ModInt operator -(ModInt x, long y) {
			return new ModInt(ModIntHelper.ModOperator.Substract(x, y));
		}

		public static ModInt operator *(ModInt x, long y) {
			return new ModInt(ModIntHelper.ModOperator.Multiply(x, y));
		}

		public static ModInt operator /(ModInt x, long y) {
			return new ModInt(ModIntHelper.ModOperator.Divide(x, y));
		}

		public static ModInt operator %(ModInt x, long y) {
			return new ModInt(ModIntHelper.ModOperator.Mod(x, y));
		}

		public static ModInt operator ++(ModInt x) {
			return x + 1;
		}

		public static ModInt operator --(ModInt x) {
			return x - 1;
		}

		public static ModInt operator +(ModInt x) {
			return new ModInt(x.Value);
		}

		public static ModInt operator -(ModInt x) {
			return new ModInt(ModIntHelper.ModOperator.Negate(x));
		}

		public static bool operator ==(ModInt x, long y) {
			return x.Equals(y);
		}

		public static bool operator !=(ModInt x, long y) {
			return x.Equals(y) == false;
		}

		public static implicit operator ModInt(long x) {
			while (x < 0)
				x += Constants.Mod;

			return new ModInt(x % Constants.Mod);
		}

		public static implicit operator long(ModInt x) {
			return x.Value;
		}
	}

	struct IntOperator : IMathArithmeticOperator<int> {
		public int MaxValue => int.MaxValue;

		public int MinValue => int.MinValue;

		public int Zero => 0;

		public int One => 1;

		public int Add(int x, int y) {
			return x + y;
		}

		public int BitShiftLeft(int x, int y) {
			return x << y;
		}

		public int BitShiftRight(int x, int y) {
			return x >> y;
		}

		public int Choose(int n, int r) {
			return ChooseHelper<int, IntOperator>.Choose(n, r);
		}

		public int Divide(int x, int y) {
			return x / y;
		}

		public int Factorial(int x) {
			var temp = 1;
			for(int i = 2; i <= x; i++) {
				temp *= i;
			}

			return temp;
		}

		public int Max(int x, int y) {
			return Math.Max(x, y);
		}

		public int Min(int x, int y) {
			return Math.Min(x, y);
		}

		public int Mod(int x, int y) {
			return x % y;
		}

		public int Multiply(int x, int y) {
			return x * y;
		}

		public int Negate(int x) {
			return -x;
		}

		public int Pow(int x, int y) {
			var ret = 1;
			var temp = x;

			for(int i = 1; i <= y; i <<= 1) {
				if ((i & y) != 0)
					ret *= temp;

				temp *= temp;
			}

			return ret;
		}

		public int Substract(int x, int y) {
			return x - y;
		}
	}

	struct LongOperator : IMathArithmeticOperator<long> {
		public long MaxValue => long.MaxValue;

		public long MinValue => long.MinValue;

		public long Zero => 0;

		public long One => 1;

		public long Add(long x, long y) {
			return x + y;
		}

		public long BitShiftLeft(long x, int y) {
			return x << y;
		}

		public long BitShiftRight(long x, int y) {
			return x >> y;
		}

		public long Choose(long n, long r) {
			return ChooseHelper<long, LongOperator>.Choose(n, r);
		}

		public long Divide(long x, long y) {
			return x / y;
		}

		public long Factorial(long x) {
			var temp = 1L;
			for (long i = 2; i <= x; i++) {
				temp *= i;
			}

			return temp;
		}

		public long Max(long x, long y) {
			return Math.Max(x, y);
		}

		public long Min(long x, long y) {
			return Math.Min(x, y);
		}

		public long Mod(long x, long y) {
			return x % y;
		}

		public long Multiply(long x, long y) {
			return x * y;
		}

		public long Negate(long x) {
			return -x;
		}

		public long Pow(long x, long y) {
			var ret = 1L;
			var temp = x;

			for (long i = 1; i <= y; i <<= 1) {
				if ((i & y) != 0)
					ret *= temp;

				temp *= temp;
			}

			return ret;
		}

		public long Substract(long x, long y) {
			return x - y;
		}
	}

	struct DoubleOperator : IMathArithmeticOperator<double> {
		public double MaxValue => double.MaxValue;

		public double MinValue => double.MinValue;

		public double Zero => 0;

		public double One => 1;

		public double Add(double x, double y) {
			return x + y;
		}

		public double BitShiftLeft(double x, int y) {
			throw new NotImplementedException();
		}

		public double BitShiftRight(double x, int y) {
			throw new NotImplementedException();
		}

		public double Choose(double n, double r) {
			return ChooseHelper<double, DoubleOperator>.Choose(n, r);
		}

		public double Divide(double x, double y) {
			return x / y;
		}

		public double Factorial(double x) {
			var temp = 1.0;
			for (double i = 2; i <= x; i++) {
				temp *= i;
			}

			return temp;
		}

		public double Max(double x, double y) {
			return Math.Max(x, y);
		}

		public double Min(double x, double y) {
			return Math.Min(x, y);
		}

		public double Mod(double x, double y) {
			return x % y;
		}

		public double Multiply(double x, double y) {
			return x * y;
		}

		public double Negate(double x) {
			return -x;
		}

		public double Pow(double x, double y) {
			return Math.Pow(x, y);
		}

		public double Substract(double x, double y) {
			return x - y;
		}
	}

	struct ModIntOperaor : IMathArithmeticOperator<ModInt> {
		public ModInt MaxValue => Constants.Mod - 1;

		public ModInt MinValue => 0;

		public ModInt Zero => 0;

		public ModInt One => 1;

		public ModInt Add(ModInt x, ModInt y) {
			return (x.Value + y.Value) % Constants.Mod;
		}

		public ModInt BitShiftLeft(ModInt x, int y) {
			long t =  x.Value << y;

			while (t < 0)
				t += Constants.Mod;

			return t;
		}

		public ModInt BitShiftRight(ModInt x, int y) {
			long t = x.Value >> y;

			while (t < 0)
				t += Constants.Mod;

			return t;
		}

		public ModInt Choose(ModInt n, ModInt r) {
			return ModIntHelper.Choose(n, r);
		}

		public ModInt Divide(ModInt x, ModInt y) {
			return x.Value * ModIntHelper.ModInv(y);
		}

		public ModInt Factorial(ModInt x) {
			long temp = 1L;
			for (long i = 2; i <= x.Value; i++) {
				temp *= i;
				temp %= Constants.Mod;
			}

			return temp;
		}

		public ModInt Max(ModInt x, ModInt y) {
			return Math.Max(x.Value, y.Value);
		}

		public ModInt Min(ModInt x, ModInt y) {
			return Math.Min(x.Value, y.Value);
		}

		public ModInt Mod(ModInt x, ModInt y) {
			return x.Value % y.Value;
		}

		public ModInt Multiply(ModInt x, ModInt y) {
			return (x.Value * y.Value) % Constants.Mod;
		}

		public ModInt Negate(ModInt x) {
			long y = -x.Value;

			while (y < 0)
				y += Constants.Mod;

			y %= Constants.Mod;

			return y;
		}

		public ModInt Pow(ModInt x, ModInt y) {
			long res = 1L;
			long a = x.Value;
			long n = y.Value;
			while (n > 0) {
				if ((n & 1) != 0) {
					res *= a;
					res %= Constants.Mod;
				}

				a *= a;
				a %= Constants.Mod;
				n >>= 1;
			}
			return res;
		}

		public ModInt Substract(ModInt x, ModInt y) {
			long ret = x.Value - y.Value;

			while (ret < 0)
				ret += Constants.Mod;

			ret %= Constants.Mod;

			return ret;
		}
	}
}

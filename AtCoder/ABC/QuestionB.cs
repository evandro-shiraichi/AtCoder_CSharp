using System;
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

namespace AtCoder.ABC
{
	class QuestionB
	{
		public static void Main(string[] args)
		{
			var scanner = new Scanner();
		}

		class Scanner
		{
			private readonly char[] delimiter_ = new char[] { ' ' };
			private readonly string filePath_;
			private readonly Func<string> reader_;
			private string[] buf_;
			private int index_;

			public Scanner(string file = "")
			{
				if (string.IsNullOrWhiteSpace(file)) {
					reader_ = Console.ReadLine;
				} else {
					filePath_ = file;
					var fs = new StreamReader(file);
					reader_ = fs.ReadLine;
				}
				buf_ = new string[0];
				index_ = 0;
			}

			public string NextLine() => reader_();
			public string Next()
			{
				if (index_ < buf_.Length) {
					return buf_[index_++];
				}

				string st = reader_();
				while (st == "") {
					st = reader_();
				}

				buf_ = st.Split(delimiter_, StringSplitOptions.RemoveEmptyEntries);
				if (buf_.Length == 0) {
					return Next();
				}

				index_ = 0;
				return buf_[index_++];
			}

			public int Int() => int.Parse(Next());
			public long Long() => long.Parse(Next());
			public double Double() => double.Parse(Next());

			public int[] ArrayInt(int N, int add = 0)
			{
				int[] Array = new int[N];
				for (int i = 0; i < N; i++) {
					Array[i] = Int() + add;
				}
				return Array;
			}

			public int[] SortedArrayInt(int N, int add = 0, bool descendingOrder = false)
			{
				var array = ArrayInt(N, add);

				if (descendingOrder) {
					Array.Sort(array, (x, y) => y.CompareTo(x));
				} else {
					Array.Sort(array);
				}

				return array;
			}

			public T[] ArrayIntTo<T>(int N, Func<int, T> converter, int add = 0)
			{
				T[] Array = new T[N];
				for (int i = 0; i < N; i++) {
					Array[i] = converter(Int() + add);
				}
				return Array;
			}

			public long[] ArrayLong(int N, long add = 0)
			{
				long[] Array = new long[N];
				for (int i = 0; i < N; i++) {
					Array[i] = Long() + add;
				}
				return Array;
			}

			public long[] SortedArrayLong(int N, int add = 0, bool descendingOrder = false)
			{
				var array = ArrayLong(N, add);

				if (descendingOrder) {
					Array.Sort(array, (x, y) => y.CompareTo(x));
				} else {
					Array.Sort(array);
				}

				return array;
			}

			public T[] ArrayLongTo<T>(int N, Func<long, T> converter, int add = 0)
			{
				T[] Array = new T[N];
				for (int i = 0; i < N; i++) {
					Array[i] = converter(Long() + add);
				}
				return Array;
			}

			public double[] ArrayDouble(int N, double add = 0)
			{
				double[] Array = new double[N];
				for (int i = 0; i < N; i++) {
					Array[i] = Double() + add;
				}
				return Array;
			}

			public double[] SortedArrayDouble(int N, int add = 0, bool descendingOrder = false)
			{
				var array = ArrayDouble(N, add);

				if (descendingOrder) {
					Array.Sort(array, (x, y) => y.CompareTo(x));
				} else {
					Array.Sort(array);
				}

				return array;
			}

			public T[] ArrayDoubleTo<T>(int N, Func<double, T> converter, int add = 0)
			{
				T[] Array = new T[N];
				for (int i = 0; i < N; i++) {
					Array[i] = converter(Double() + add);
				}
				return Array;
			}

			public string[] ArrayString(int h)
			{
				var array = new string[h];

				for (int i = 0; i < h; i++) {
					array[i] = Next();
				}

				return array;
			}

			public T[] ArrayStringTo<T>(int h, Func<string, T> converter)
			{
				var array = new T[h];

				for (int i = 0; i < h; i++) {
					array[i] = converter(Next());
				}

				return array;
			}

			public int[][] IntMap(int h, int w, int add = 0)
			{
				var map = new int[h][];

				for (int i = 0; i < h; i++) {
					map[i] = ArrayInt(w, add);
				}

				return map;
			}

			public T[][] IntMapTo<T>(int h, int w, Func<int, T> converter, int add = 0)
			{
				var map = new T[h][];

				for (int i = 0; i < h; i++) {
					map[i] = ArrayIntTo(w, converter, add);
				}

				return map;
			}

			public long[][] LongMap(int h, int w, int add = 0)
			{
				var map = new long[h][];

				for (int i = 0; i < h; i++) {
					map[i] = ArrayLong(w, add);
				}

				return map;
			}

			public T[][] LongMapTo<T>(int h, int w, Func<long, T> converter, int add = 0)
			{
				var map = new T[h][];

				for (int i = 0; i < h; i++) {
					map[i] = ArrayLongTo(w, converter, add);
				}

				return map;
			}

			public double[][] DoubleMap(int h, int w, int add = 0)
			{
				var map = new double[h][];

				for (int i = 0; i < h; i++) {
					map[i] = ArrayDouble(w, add);
				}

				return map;
			}

			public T[][] DoubleMapTo<T>(int h, int w, Func<double, T> converter, int add = 0)
			{
				var map = new T[h][];

				for (int i = 0; i < h; i++) {
					map[i] = ArrayDoubleTo(w, converter, add);
				}

				return map;
			}

			public (int, int) Int2(int add = 0)
			{
				return (Int() + add, Int() + add);
			}

			public (int, int, int) Int3(int add = 0)
			{
				return (Int() + add, Int() + add, Int() + add);
			}

			public (long, long) Long2(int add = 0)
			{
				return (Long() + add, Long() + add);
			}

			public (long, long, long) Long3(int add = 0)
			{
				return (Long() + add, Long() + add, Long() + add);
			}

			public (double, double) Double2(int add = 0)
			{
				return (Double() + add, Double() + add);
			}

			public (double, double, double) Double3(int add = 0)
			{
				return (Double() + add, Double() + add, Double() + add);
			}

			public void Save(string text)
			{
				if (string.IsNullOrWhiteSpace(filePath_)) {
					return;
				}

				File.WriteAllText(filePath_ + "_output.txt", text);
			}
		}

		class ArrayHelper
		{
			public static T[] CreateArray<T>(int w)
			{
				var array = new T[w];
				return array;
			}

			public static T[] Fill<T>(T[] array, Func<T> intialValue)
			{
				for (int i = 0; i < array.Length; i++) {
					array[i] = intialValue();
				}

				return array;
			}

			public static T[] FillArray<T>(T[] array, Func<T> initialValue)
			{
				Fill(array, initialValue);
				return array;
			}

			public static T[] CreateArrayFilled<T>(int w, Func<T> initialValue)
			{
				return FillArray(CreateArray<T>(w), initialValue);
			}

			public static T[][] CreateMap<T>(int h, int w)
			{
				var map = new T[h][];

				for (int i = 0; i < h; i++) {
					map[i] = new T[w];
				}

				return map;
			}

			public static T[][] FillMap<T>(T[][] map, Func<T> initialValue)
			{
				var h = map.Length;

				for (int i = 0; i < h; i++) {
					Fill(map[i], initialValue);
				}

				return map;
			}

			public static T[][] CreateMapFilled<T>(int h, int w, Func<T> initialValue)
			{
				return FillMap(CreateMap<T>(h, w), initialValue);
			}

			public static void WriteArray<T>(IEnumerable<T> array, string sep)
			{
				Console.WriteLine(string.Join(sep, array));
			}

			public static void WriteMap<T>(IEnumerable<IEnumerable<T>> map, string sep)
			{
				foreach (var array in map) {
					WriteArray(array, sep);
				}
			}
			public static void WriteArray<T, R>(IEnumerable<T> array, string sep, Func<T, R> converter)
			{
				Console.WriteLine(string.Join(sep, array.Select(x => converter(x))));
			}

			public static void WriteMap<T, R>(IEnumerable<IEnumerable<T>> map, string sep, Func<T, R> converter)
			{
				foreach (var array in map) {
					WriteArray(array, sep, converter);
				}
			}
		}
	}
}

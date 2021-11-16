using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtCoder.Lib {

	public static class Permutation {
		public static IEnumerable<int[]> Permutate(int n) {
			var src = Enumerable.Range(0, n).ToArray();
			do {
				yield return src;
			} while (Next(src, 0, n));
		}

		public static IEnumerable<T[]> Permutate<T>(T[] src)
			where T : IComparable<T> {
			int n = src.Length;
			do {
				yield return src;
			} while (Next(src, 0, n));
		}

		public static bool Next<T>(T[] src, int index, int length)
			where T : IComparable<T> {
			if (length <= 1)
				return false;

			int last = index + length - 1;
			int i = last;
			while (true) {
				int ii = i;
				i--;

				if (src[i].CompareTo(src[ii]) < 0) {
					int j = last;
					while (src[i].CompareTo(src[j]) >= 0)
						--j;

					(src[j], src[i]) = (src[i], src[j]);
					Array.Reverse(src, ii, last - ii + 1);
					return true;
				}

				if (i == index) {
					Array.Reverse(src, index, length);
					return false;
				}
			}
		}

		public static bool Prev<T>(T[] src, int index, int length)
			where T : IComparable<T> {
			if (length <= 1)
				return false;

			int last = index + length - 1;
			int i = last;
			while (true) {
				int ii = i;
				--i;

				if (src[ii].CompareTo(src[i]) < 0) {
					int j = last;
					while (src[j].CompareTo(src[i]) >= 0)
						--j;

					(src[j], src[i]) = (src[i], src[j]);
					Array.Reverse(src, ii, last - ii + 1);
					return true;
				}

				if (i == index) {
					Array.Reverse(src, index, length);
					return false;
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtCoder.Lib
{
	class BinarySearch
	{

		public static IEnumerable<(int x1, int x2, int y1, int y2)> PairAllSearch(int h, int w)
		{
			for (int y1 = 0; y1 < h; y1++)
				for (int y2 = y1; y2 < h; y2++)
					for (int x1 = 0; x1 < w; x1++)
						for (int x2 = x1; x2 < w; x2++)
							yield return (x1, x2, y1, y2);
		}

		/// <summary>
		/// 指定した値以上の最小値を二分探索（Sortが昇順の場合）
		/// </summary>
		/// <typeparam name="T">比較可能な型</typeparam>
		/// <param name="source">昇順Sortされた配列</param>
		/// <param name="target">指定する値</param>
		/// <returns>指定した値以上の最小値のインデックス</returns>
		public static int UpperBound<T>(T[] source, T target)
			where T : IComparable<T>
		{
			var ng = -1;
			var ok = source.Length;

			while (Math.Abs(ok - ng) > 1) {
				var center = (ng + ok) / 2;

				if (source[center].CompareTo(target) >= 0) {
					ok = center;
				} else {
					ng = center;
				}
			}

			return ok;
		}

		/// <summary>
		/// 指定した値以下の最大値を二分探索（Sortが降順の場合）
		/// </summary>
		/// <typeparam name="T">比較可能な型</typeparam>
		/// <param name="source">降順Sortされた配列</param>
		/// <param name="target">指定する値</param>
		/// <returns>指定した値以下の最大値のインデックス</returns>
		public static int LowerBound<T>(T[] source, T target)
			where T : IComparable<T>
		{
			var ok = -1;
			var ng = source.Length;

			while (Math.Abs(ok - ng) > 1) {
				var center = (ng + ok) / 2;

				if (source[center].CompareTo(target) <= 0) {
					ok = center;
				} else {
					ng = center;
				}
			}

			return ok;
		}
	}
}

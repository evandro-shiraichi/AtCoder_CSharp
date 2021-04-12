using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtCoder.Lib
{
	class BinarySearch
	{
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

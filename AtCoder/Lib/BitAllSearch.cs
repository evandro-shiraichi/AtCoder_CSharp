using AtCoder.ABC;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtCoder.Lib
{
	class BitAllSearchs {
		public static T AnswerBinarySearch<T>(T ok, T ng, T threshold, Func<T, bool> checker)
		where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
			var math = IMathArithmeticOperator<T>.GetOperator();
			var two = math.Add(math.One, math.One);

			while (math.Abs(math.Substract(ok, ng)).CompareTo(threshold) > 0) {
				var c = math.Divide(math.Add(ok, ng), two);

				if (checker(c)) {
					ok = c;
				} else {
					ng = c;
				}
			}

			return ok;
		}
		/// <summary>
		/// ビット全探索
		/// </summary>
		/// <param name="n">何ビット必要か</param>
		/// <returns>各探索で立っている（1になっている）ビットのList</returns>
		public static IEnumerable<List<int>> BitAllSearch(int n)
		{
			var max = 1 << n;

			for (int i = 0; i < max; i++) {
				var list = new List<int>();

				for (int j = 0; j < n; j++) {
					if ((i >> j & 1) != 0) {
						list.Add(j);
					}
				}

				yield return list;
			}
		}
	}
}

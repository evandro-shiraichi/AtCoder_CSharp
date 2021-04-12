using System;
using System.Collections.Generic;
using System.Text;

namespace AtCoder.Lib
{
	class BitAllSearchs
	{
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

using System;
using System.Collections.Generic;
using System.Text;

namespace AtCoder.Lib {
    class Graphs {
		static void DoWarshallFloyd(long[,] d) {
			var n = d.GetLength(0);

			for (int i = 0; i < n; i++) {
				d[i, i] = 0;
			}

			for (int k = 0; k < n; k++) {
				for (int i = 0; i < n; i++) {
					for (int j = 0; j < n; j++) {
						d[i, j] = Math.Min(d[i, j], d[i, k] + d[k, j]);
					}
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace AtCoder.Lib
{
	class Graphs
	{
        long[][] d = new long[2][];

        void WarshallFloyd(int n)
        {
            for (int k = 0; k < n; k++) {
                for (int i = 0; i < n; i++) {
                    for (int j = 0; j < n; j++) {
                        d[i][j] = Math.Min(d[i][j], d[i][k] + d[k][j]);
                    }
                }
            }
        }
    }
}

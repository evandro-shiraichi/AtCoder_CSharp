using System;
using System.Collections.Generic;
using System.Text;

namespace AtCoder.Lib {
    class DigitDPS {
		static long DigitDP(string str, Func<int, bool> checker) {
			var dp = new long[str.Length + 1, 2, 2];

			//dp[上から何桁目, chekcerがtrueかどうか, str未満確定かどうか]
			dp[0, 0, 0] = 1;

			for (int i = 0; i < str.Length; i++) {
				int D = str[i] - '0';
				for (int j = 0; j < 2; j++) {
					for (int k = 0; k < 2; k++) {
						for (int d = 0; d < (k >= 1 ? 10 : D + 1); d++) {
							var newJ = j;
							var newK = k;
							if (checker(d))
								newJ = 1;
							if (d < D)
								newK = 1;
							dp[i + 1, newJ, newK] += dp[i, j, k];
						}
					}
				}
			}

			return dp[str.Length, 1, 0] + dp[str.Length, 1, 1];
		}
	}
}

using AtCoder.ABC;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtCoder.Lib {
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

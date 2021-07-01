using System;
using System.Collections.Generic;
using System.Text;

namespace AtCoder.Lib
{
	static class Mod
	{
		public static readonly long mod = 1000000007;
		//public static readonly long mod = 13;

		public static (long[] fac, long[] finv, long[] inv) ModBinomialCoefficientsInit(int MAX = 510000, int MOD = 1000000007)
		{
			var fac = new long[MAX];
			var finv = new long[MAX];
			var inv = new long[MAX];

			fac[0] = fac[1] = 1;
			finv[0] = finv[1] = 1;
			inv[1] = 1;
			for (int i = 2; i < MAX; i++)
			{
				fac[i] = fac[i - 1] * i % MOD;
				inv[i] = MOD - inv[MOD % i] * (MOD / i) % MOD;
				finv[i] = finv[i - 1] * inv[i] % MOD;
			}

			return (fac, finv, inv);
		}

		// 二項係数計算
		public static long ModBinomialCoefficients(
			int n,
			int k,
			long[] fac,
			long[] finv,
			long[] inv,
			int MOD = 1000000007)
		{
			if (n < k) return 0;
			if (n < 0 || k < 0) return 0;
			return fac[n] * (finv[k] * finv[n - k] % MOD) % MOD;
		}

		public static long Pow(long a, long b)
		{
			var temp = a;
			var ans = 1L;

			for (int i = 1; i <= b; i <<= 1) {
				if ((b & i) != 0) {
					ans *= temp;
					if (mod != 0) {
						ans %= mod;
					}
				}

				temp *= temp;
				if (mod != 0) {
					temp %= mod;
				}
			}

			return ans;
		}

		public static long ModPow(long n, long k)
		{
			var answer = 1L;
			var tempMultiple = n;

			while (k > 0) {
				if ((k & 1) != 0) {
					answer *= tempMultiple;
					answer %= mod;
				}

				tempMultiple *= tempMultiple;
				tempMultiple %= mod;
				k >>= 1;
			}

			return answer;
		}

		public static long ModInv(long a)
		{
			return ModPow(a, mod - 2);
		}

		public static long ModInv(long a, long m)
		{
			var (b, u, v) = (m, 1L, 0L);

			while(b > 0) {
				var t = a / b;
				a -= t * b;
				(a, b) = Swap(a, b);
				u -= t * v;
				(u, v) = Swap(u, v);
			}

			u %= m;

			if(u < 0) {
				u += m;
			}

			return u;
		}

		public static (T, T) Swap<T>(T a, T b)
		{
			return (b, a);
		}
	}
}

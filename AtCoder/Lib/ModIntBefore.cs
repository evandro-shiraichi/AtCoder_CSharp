using System;
using System.Collections.Generic;
using System.Text;

namespace AtCoder.Lib
{
	class ModIntBefore
	{
		private readonly int MAX;
		private readonly long MOD;

		private long[] fac_;
		private long[] finv_;
		private long[] inv_;

		public long Number;

		public ModIntBefore(long integer, long mod = 1000000007, int max = 510000)
		{
			Number = integer;
			MOD = mod;
			MAX = max;
		}

		public long Plus(long a)
		{
			Number = (Number + a) % MOD;
			return Number;
		}

		public long Minus(long a)
		{
			var temp = Number - a;

			if (temp < 0)
			{
				temp += MOD;
			}

			Number = temp % MOD;
			return Number;
		}

		public long Multi(long a)
		{
			Number = (Number * a) % MOD;
			return Number;
		}

		public long Div(long a)
		{
			Multi(ModInv(a, MOD));
			return Number;
		}


		private long ModInv(long a)
		{
			return ModPow(a, MOD - 2);
		}

		// 拡張ユークリッド版
		private long ModInv(long a, long m)
		{
			var (b, u, v) = (m, 1L, 0L);

			while (b > 0)
			{
				var t = a / b;
				a -= t * b;
				(a, b) = (b, a);
				u -= t * v;
				(u, v) = (v, u);
			}

			u %= m;

			if (u < 0)
			{
				u += m;
			}

			return u;
		}
		public long Pow(long a)
		{
			Number = ModPow(Number, a);
			return Number;
		}

		private long ModPow(long n, long k)
		{
			var answer = 1L;
			var tempMultiple = n;

			while (k > 0)
			{
				if ((k & 1) != 0)
				{
					answer *= tempMultiple;
					answer %= MOD;
				}

				tempMultiple *= tempMultiple;
				tempMultiple %= MOD;
				k >>= 1;
			}

			return answer;
		}

		// 二項係数計算
		public long BinomialCoefficients(int n, int k)
		{
			if (n < k) return 0;
			if (n < 0 || k < 0) return 0;

			if (fac_ is null)
			{
				BinomialCoefficientsInit();
			}

			return fac_[n] * (finv_[k] * finv_[n - k] % MOD) % MOD;
		}

		private (long[] fac, long[] finv, long[] inv) BinomialCoefficientsInit()
		{
			fac_ = new long[MAX];
			finv_ = new long[MAX];
			inv_ = new long[MAX];

			fac_[0] = fac_[1] = 1;
			finv_[0] = finv_[1] = 1;
			inv_[1] = 1;
			for (int i = 2; i < MAX; i++)
			{
				fac_[i] = fac_[i - 1] * i % MOD;
				inv_[i] = MOD - inv_[MOD % i] * (MOD / i) % MOD;
				finv_[i] = finv_[i - 1] * inv_[i] % MOD;
			}

			return (fac_, finv_, inv_);
		}

		public override string ToString()
		{
			return Number.ToString();
		}

		public override int GetHashCode()
		{
			return Number.GetHashCode();
		}
	}
}

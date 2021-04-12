using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtCoder.Lib
{
    static class Calc
    {
        public static long nPr(int n, int r)
        {
            var t = 1L;

            for(int i = n; i > (n - r); i--) {
                t *= i;
			}

            return t;
        }

        public static long Pow(long a, long b, long mod = 0L)
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

        public class NCR
		{
			public int n;
			public int r;

			public NCR(int nn, int rr)
			{
				n = nn;
				r = rr;
			}

			public override int GetHashCode()
			{
				return n.GetHashCode() + r.GetHashCode() * 117;
			}

			public override bool Equals(object obj)
			{
				if (obj is null) {
					return false;
				}

				if (obj is NCR ncr) {
					return this.n == ncr.n && this.r == ncr.r;
				}

				return false;
			}
		}

		public static Dictionary<NCR, long> NcrDict = new Dictionary<NCR, long>();

		public static long nCr(int n, int r)
		{
			if (n < r) {
				throw new Exception();
			}
            
			r = Math.Min(r, n - r);

			var cr = new NCR(n, r);

			if (NcrDict.ContainsKey(cr)) {
				return NcrDict[cr];
			}

			if (r == 1) {
				return n;
			} else if (r == 0) {
				return 1;
			}

			var temp = nCr(n - 1, r) + nCr(n - 1, r - 1);

			NcrDict.Add(cr, temp);

			return temp;
		}
        public static long LCM(long a, long b)
        {
            return a * b / GCD(a, b);
        }

        public static long GCD(long a, long b)
        {
            if (a < b) {
                return GCD(b, a);
            }

            while (true) {
                var r = a % b;
                if (r == 0) {
                    return b;
                }

                a = b;
                b = r;
            }
        }

        public static long CountPrime(long n)
        {
            var res = new HashSet<long>();
            for (long i = 2; i * i <= n; i++) {
                if (n % i != 0) {
                    continue;
                }

                if (res.Contains(i) == false) {
                    res.Add(i);
                }

                while (n % i == 0) {
                    n /= i;
                }
            }

            if (n != 1) {
                if (res.Contains(n) == false) {
                    res.Add(n);
                }
            }

            return res.Count;
        }

        public static IEnumerable<long> GetPrime(long n)
        {
            var res = new HashSet<long>();
            for (long i = 2; i * i <= n; i++) {
                if (n % i != 0) {
                    continue;
                }

                if (res.Contains(i) == false) {
                    res.Add(i);
                    yield return i;
                }

                while (n % i == 0) {
                    n /= i;
                }
            }

            if (n != 1) {
                if (res.Contains(n) == false) {
                    res.Add(n);
                    yield return n;
                }
            }
        }
    }
}

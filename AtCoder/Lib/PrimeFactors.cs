using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtCoder.Lib
{
    class PrimeFactors
    {
        public static IEnumerable<long> PrimeFactorsImpl(long n)
        {
            long i = 2;
            long tmp = n;

            while (i * i <= n)
            {
                if (tmp % i == 0)
                {
                    tmp /= i;
                    yield return i;
                }
                else
                {
                    i++;
                }
            }

            if (tmp != 1) {
                yield return tmp;
            }
        }
    }
}

using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01__PINQ
{
   public class _01__Calculating_Prime_Numbers
    {
        public static void Show()
        {
            {
                IEnumerable<int> numbers = Enumerable.Range(3, 1000000);
                var parallelQuery =
                     from n in numbers.AsParallel().AsOrdered()
                     where Enumerable.Range(2, (int)Math.Sqrt(n)).All(i => n % i > 0)
                     select n;
                int[] primes = parallelQuery.ToArray();
                primes.Take(100).Dump();
            }
        }

    }
}

using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace _01__PINQ
{
    public class _03__Parallel_Features
    {
        public static void Show()
        {
            // 并行索引
            {
                int i = 0;
                (from n in Enumerable.Range(0, 999)
                .AsParallel()
                 select n * i++)         // AsParallel 之后，会在并行线程上执行查询，处理次序非顺序，因此 i 无法对应输入元素的位置
                .Dump("Unsafe");
            }
            {
                Enumerable.Range(0, 999)
               .AsParallel()            // 带索引版本的 Select
               .Select((n, i) => n * i).Dump("Safe");
            }

            // Changing degree of parallelism
            {
                "The Quick Brown Fox"
    .AsParallel().WithDegreeOfParallelism(2)
    .Where(c => !char.IsWhiteSpace(c))
    .AsParallel().WithDegreeOfParallelism(3)   // Forces Merge + Partition
    .Select(c => char.ToUpper(c));
            }

            // Cacellation
            {
                IEnumerable<int> million = Enumerable.Range(3, 1000000);

                var cancelSource = new CancellationTokenSource();

                var primeNumberQuery =
                    from n in million.AsParallel().WithCancellation(cancelSource.Token)
                    where Enumerable.Range(2, (int)Math.Sqrt(n)).All(i => n % i > 0)
                    select n;

                new Thread(() =>
                {
                    Thread.Sleep(100);      // Cancel query after
                    cancelSource.Cancel();   // 100 milliseconds.
                }
                           ).Start();
                try
                {
                    // Start query running:
                    int[] primes = primeNumberQuery.ToArray();
                    // We'll never get here because the other thread will cancel us.
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Query canceled");
                }
            }
        }
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace _03__Principles_of_Asynchrony
{
    public class _01__GetPrimeCounts
    {
        public static void Show()
        {
            DisplayPrimeCountsAsync();
            //DisplayPrimeCountsAsync();

            //DisplayPrimeCountsWithAwaiter();        // 异步 乱序执行 
            Console.WriteLine("Show Done.");

            //DisplayPrimeCountsWithAwaiter();
            return;
            // sync 
            {
                DisplayPrimeCounts();
            }
            // task  粗粒度并发
            {
                Task.Run(() => DisplayPrimeCounts());
            }
            // awaiter  细粒度并发
            {
                DisplayPrimeCountsWithAwaiter();        // 异步 乱序执行 
                DisplayPrimeCountsWithTaskSource();     // 异步 顺序执行
            }
            // sync 细粒度并发
            {
                DisplayPrimeCountsAsync();              // 异步 顺序执行
            }

        }

        public static void DisplayPrimeCounts()
        {
            for (int i = 0; i < 10; i++)
                Console.WriteLine(GetPrimesCount(i * 1000000 + 2, 1000000) +
                 " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1));
        }

        public static int GetPrimesCount(int start, int count)
        {
            int primesCount = ParallelEnumerable.Range(start, count).Count(n =>
            {
                return Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i =>
                {
                    return n % i > 0;
                });
            });
            return primesCount;

        }

        public static void DisplayPrimeCountsWithAwaiter()
        {
            // 循环10次迭代 非阻塞 并行执行
            for (int i = 0; i < 10; i++)
            {
                //var awaiter = GetPrimesCountAsync(i * 1000000 + 2, 1000000).GetAwaiter();
                //awaiter.OnCompleted(() =>
                //   Console.WriteLine(awaiter.GetResult() + " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1)));
                var t = GetPrimesCountAsync(i * 1000000 + 2, 1000000);
                t.ContinueWith((r) =>
                    Console.WriteLine(r.Result + " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1)));
            }
            Console.WriteLine("Done!");  // "Done!" 会提前输出

        }

        public static Task DisplayPrimeCountsWithTaskSource()
        {
            var machine = new PrimesStateMachine();
            machine.DisplayPrimeCountsFrom(0);
            return machine.Task;
        }

        public static async Task DisplayPrimeCountsAsync()
        {
            var task1 = GetPrimesCountAsync(10, 20);
            var task2 = GetPrimesCountAsync(20, 30);
            var task3 = GetPrimesCountAsync(30, 40);

            await task1; Console.WriteLine("task1");
            await task2; Console.WriteLine("task2");
            await task3; Console.WriteLine("task3");

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId);

                //await之后在哪个线程上执行 ?
                //  · 在await表达式之后，编译器依赖于continuation(通过awaiter模式）来继续执行
                //  · 如果在富客户端应用的UI线程上，同步上下文会保证 后续 是在原线程上执行;
                //  · 否则，就会在task结束的线程上继续执行。
                var result = await GetPrimesCountAsync(i * 1000000 + 2, 1000000) +
                  " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1); // 遇到 await, 跳到主线程，开辟工作线程

                Console.WriteLine(result);

                //var w = GetPrimesCountAsync(i * 1000000 + 2, 1000000).GetAwaiter();
                //w.OnCompleted(() =>
                //{
                //    var result = w.GetResult() + " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1);
                //    Console.WriteLine(result);
                //});
            }
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);

            Console.WriteLine("Done!");
        }

        public static Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run(() =>
            {
                int primesCount = ParallelEnumerable.Range(start, count).Count(n =>
                {
                    Task.Delay(5000);
                    //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                    return Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i =>
                    {
                        return n % i > 0;
                    });
                });
                Console.WriteLine("PrimesCount: " + primesCount);
                return primesCount;
            });
        }
    }

    public class PrimesStateMachine        // Even more awkward!!
    {
        TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();
        public Task Task { get { return _tcs.Task; } }

        public void DisplayPrimeCountsFrom(int i)
        {
            var awaiter = GetPrimesCountAsync(i * 1000000 + 2, 1000000).GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                Console.WriteLine(awaiter.GetResult() + " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1));
                if (i++ < 10) DisplayPrimeCountsFrom(i);
                else { Console.WriteLine("Done!"); _tcs.SetResult(null); }
            });
        }

        Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run(() =>
               ParallelEnumerable.Range(start, count).Count(n =>
                 Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));
        }
    }
}

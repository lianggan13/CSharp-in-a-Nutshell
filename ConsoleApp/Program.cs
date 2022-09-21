using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static  void Main(string[] args)
        {

            //AsyncCancelOperation cancelOperation = new AsyncCancelOperation();
            //cancelOperation.Show();

            //ProgressReportor reportor = new ProgressReportor();
            //reportor.Show();

            //TaskCombiner taskCombiner = new TaskCombiner();
            //taskCombiner.Show();

            UIAwait uIAwait = new UIAwait();
            uIAwait.Show();
            Console.WriteLine("弹出，回弹到UI消息循环【执行点】回到主UI线程...");
            
            Console.ReadKey();

            // TaskCompletionSource
            // 给任务附加延续
            var tcs = new TaskCompletionSource<int>();
            new Thread(() =>
            {
                Thread.Sleep(5000);
                Console.WriteLine("task completed.");
                tcs.SetResult(43);
            })
            { IsBackground = true }.Start();
            Task<int> task = tcs.Task;
            Console.WriteLine(task.Result);
            // 延续启动线程
            //for (int i = 0; i < 10000; i++)
            //{
            //    MyDelay(5000).GetAwaiter().OnCompleted(() => Console.WriteLine(42));
            //}

            //Task.Delay(5000).GetAwaiter().OnCompleted(() => Console.WriteLine(42));

            // 同步操作 sync operaion 先完成其工作在返回调用者
            // 异步操作 async operation 大部分工作是在返回给调用者完成的
            // 调用时 立即返回给调用者 非阻塞方法 Delegate.BeginInvoke Thread.Start Task.Run GetAwaiter.OnCompleted

            // I/O密集型 计算密集型
            DisplayPrimeCounts();

            /* 补充:使用异步函数进行程序设计的基本原则
                    1.首先 以同步方式实现方法
                    2.其次 将同步方法改为异步方法调用，并使用await
                    3.最后 除了"最顶级"的方法(如UI控制事件处理器)之外，将异步方法的返回类型修改为 Task或Task<TResult>，使其成为可等待的方法
             */    

            Console.WriteLine("主UI线程----------------------------------------结束");
            Console.ReadKey();
        }
        

        /// <summary>
        /// 延时函数(使用TaskCompletionSource实现I/O密集异步方法的标准手段)
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static Task MyDelay(int milliseconds)
        {
            var tcs = new TaskCompletionSource<object>();
            var timer = new System.Timers.Timer(milliseconds) { AutoReset = false };
            timer.Elapsed += delegate { timer.Dispose(); tcs.SetResult(null); };
            timer.Start();
            return tcs.Task;
        }

        public static async void DisplayPrimeCounts()
        {
            for (int i = 0; i < 10; i++)
            {
                // 同步
                // Console.WriteLine(GetPrimesCount(i * 100000 + 2, 100000)
                //    + " primes between " + (i * 100000) + " and " + ((i + 1) * 100000 - 1));

                // 异步
                //var awaiter = GetPrimesCountAsync(i * 100000 + 2, 100000).GetAwaiter();
                //awaiter.OnCompleted(() =>
                //        Console.WriteLine(awaiter.GetResult() + " primes between " + (i * 100000) + " and " + ((i + 1) * 100000 - 1)));              

                // 异步 并 保证执行顺序
                var awaiter =  GetPrimesCountAsync(i * 100000 + 2, 100000).GetAwaiter();
                
                Console.WriteLine(await GetPrimesCountAsync(i * 100000 + 2, 100000)
                      + " primes between " + (i * 100000) + " and " + ((i + 1) * 100000 - 1));

            }
            Console.WriteLine("Done!");
        }

  

        private static int GetPrimesCount(int start, int count)
        {
            return ParallelEnumerable.Range(start, count).Count(n =>
                    Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0));

        }
        private static Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run(() =>
                    ParallelEnumerable.Range(start, count).Count(n =>
                    Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0))
                );

        }

    }
}

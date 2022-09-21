using System;
using System.Threading;
using System.Threading.Tasks;

namespace _02__Tasks
{
    /// <summary>
    /// 创建Task的另一种方式: TaskCompletionSource(附属任务)
    /// </summary>
    public class _04TaskCompletionSource
    {
        Task<TResult> Run<TResult>(Func<TResult> func)  // 本地函数
        {
            var tcs = new TaskCompletionSource<TResult>();
            new Thread(() =>
            {
                try { tcs.SetResult(func()); }
                catch (Exception ex) { tcs.SetException(ex); }
            }).Start();
            return tcs.Task;
        }

        public void Show()
        {
            // TaskCompletionSource
            {
                TakeMeasures();
            }

            // Run define func 
            {
                Func<int> func = () => { Thread.Sleep(3000); return 42; };
                Task<int> task = Run<int>(func);    // 调用自己的 Run 方法
                Console.WriteLine(task.Result);     // 42 会阻塞
                Console.WriteLine("Main Thread Point #2");
            }
            // My Async Delay method
            {
                Task Delay(int milliseconds)
                {
                    var tcs = new TaskCompletionSource<object>();
                    var timer = new System.Timers.Timer(milliseconds) { AutoReset = false };    // 只引发一次
                    timer.Elapsed += delegate { timer.Dispose(); tcs.SetResult(null); };
                    timer.Start();
                    return tcs.Task;
                }
                Delay(5000).GetAwaiter().OnCompleted(() => Console.WriteLine(42));  // 42 异步 非阻塞 内部使用定时器
                Console.WriteLine("Main Thread Point #3");

                for (int i = 0; i < 10000; i++)
                    Delay(5000).GetAwaiter().OnCompleted(() => Console.Write(42));  // 42 异步 非阻塞 内部使用定时器
                Console.WriteLine("Main Thread Point #4");
            }
            // Task Delay
            {
                Task.Delay(5000).GetAwaiter().OnCompleted(() => Console.WriteLine(42));
                Task.Delay(5000).ContinueWith(t => Console.WriteLine(42));
            }

        }

        /// <summary>
        /// 小试牛刀
        /// </summary>
        public void TakeMeasures()
        {
            // Print 42 after 5 seconds
            var tcs = new TaskCompletionSource<int>();
            new Thread(() => { Thread.Sleep(5000); tcs.SetResult(42); }).Start();
            Task<int> task = tcs.Task;          // Our "slave" task.
            Console.WriteLine(task.Result);     // 42 会阻塞
            Console.WriteLine("Main Thread Point #1");
        }


        /// <summary>
        /// 利用TaskCompletionSource创建任务
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public Task<TResult> CreateTaskCompletionSourceTask<TResult>(Func<TResult> func)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            new Thread(() =>
            {
                try
                {
                    tcs.SetResult(func());
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                    throw;
                }
            }).Start();
            return tcs.Task;
        }


    }
}

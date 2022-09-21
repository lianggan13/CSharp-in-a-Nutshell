using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace _05__Asynchronous_Patters
{
    /// <summary>
    /// 任务组合器
    /// </summary>
    public class _03__Task_Combinators
    {
        public static void Show()
        {
#if false
            // WhenAny
            {
                new MyTask().TaskWhenAnyShow();
                new MyTask().TaskWhenAnyAwaitShow();
                new MyTask().TaskWhenAnyTimeoutShow();
            }
#elif false
            // WhenAll
            {
                new MyTask().TaskWhenAllShow();
                new MyTask().TaskWhenAllExpShow();
                new MyTask().TaskWhenAllReturnShow();
            }
#elif true
            // MyCombinators
            {
                new MyTask().TaskWhenAllShow();
                new MyTask().TaskWhenAllExpShow();
                new MyTask().TaskWhenAllReturnShow();
            }
#endif
        }

        class MyTask
        {
            public async void TaskWhenAny()
            {
                Task<int> winningTask = await Task.WhenAny(Delay1(), Delay2(), Delay3());
                Console.WriteLine("Done");
                Console.WriteLine(winningTask.Result);
            }

            public async void TaskWhenAnyAwait()
            {
                Task<int> winningTask = await Task.WhenAny(Delay1(), Delay2(), Delay3());
                Console.WriteLine("Done");
                Console.WriteLine(await winningTask);
            }

            public async void TaskWhenAnyTimeout()
            {
                // 添加 超时和取消功能
                Task<string> task = SomeAsyncFunc();
                Task winner = await (Task.WhenAny(task, Delay1(), Delay2(), Delay3(), Task.Delay(5000)));
                if (winner != task) throw new TimeoutException();
                string result = await task;   // Unwrap result/re-throw
            }

            public async void TaskWhenAllShow()
            {
                Task<int[]> all = Task.WhenAll(Delay1(), Delay2(), Delay3());
                int[] r = await all;
                Console.WriteLine("Done");
                r.Dump();
            }

            public async void TaskWhenAllExpShow()
            {
                Task task1 = Task.Run(() => { throw null; });
                Task task2 = Task.Run(() => { throw null; });
                Task all = Task.WhenAll(task1, task2);
                try { await all; }
                catch
                {
                    Console.WriteLine(all.Exception.InnerExceptions.Count);   // 2 
                }
            }
            public async void TaskWhenAllReturnShow()
            {
                Task<int> task1 = Task.Run(() => 1);
                Task<int> task2 = Task.Run(() => 2);
                int[] results = await Task.WhenAll(task1, task2);   // { 1, 2 }	
                results.Dump();
            }

            public async void TaskWhenAllWebDownloadShow()
            {
                int totalSize = await GetTotalSize("http://www.linqpad.net http://www.albahari.com http://stackoverflow.com".Split());
                totalSize.Dump();
            }

            public async void TaskWhenAllWebDownloadAysncLambdaShow()
            {
                int totalSize = await GetTotalSize("http://www.linqpad.net http://www.albahari.com http://stackoverflow.com".Split());
                totalSize.Dump();
            }
            public async void TaskMyCombinatorsShow()
            {
                // 任务超时 抛出异常
                string result = await WithTimeout(SomeAsyncFunc(), TimeSpan.FromSeconds(2));
                result.Dump();
            }
            public async void TaskWithCancellationShow()
            {
                // 任务超时 取消 或 抛异常
                var cts = new CancellationTokenSource(3000);  // Cancel after 3 seconds
                string result = await WithCancellation(SomeAsyncFunc(), cts.Token);
                result.Dump();
            }

            public async void TaskWithWhenAllOrErrorhow()
            {
                // 任务错误 终止所有任务
                Task<int> task1 = Task.Run(() => { throw null; return 42; });
                Task<int> task2 = Task.Delay(5000).ContinueWith(ant => 53);

                int[] results = await WhenAllOrError(task1, task2);
            }

            async Task<int> Delay1() { await Task.Delay(5000); return 5; }
            async Task<int> Delay2() { await Task.Delay(6000); return 6; }
            async Task<int> Delay3() { await Task.Delay(7000); return 7; }

            async Task<string> SomeAsyncFunc()
            {
                await Task.Delay(10000);
                return "foo";
            }
            async Task<int> GetTotalSize(string[] uris)
            {
                IEnumerable<Task<byte[]>> downloadTasks = uris.Select(uri => new WebClient().DownloadDataTaskAsync(uri));
                // 所有任务完成之后，再处理字节数组
                byte[][] contents = await Task.WhenAll(downloadTasks);
                return contents.Sum(c => c.Length);
            }
            async Task<int> GetTotalSizeWithAsyncLambda(string[] uris)
            {
                IEnumerable<Task<int>> downloadTasks = uris.Select(
                    async uri => (await new WebClient().DownloadDataTaskAsync(uri)).Length);
                // 下载完成后，立即将字节数组转换为长度 （异步 Lambda + Linq）
                int[] contentLengths = await Task.WhenAll(downloadTasks);
                return contentLengths.Sum();
            }

            async Task<TResult> WithTimeout<TResult>(Task<TResult> task, TimeSpan timeout)
            {
                Task winner = await (Task.WhenAny(task, Task.Delay(timeout)));
                if (winner != task) throw new TimeoutException();   // 判断 完成的任务是 task 还是 Task.Delay(timeout)
                return await task;   // Unwrap result/re-throw
            }

            Task<TResult> WithCancellation<TResult>(Task<TResult> task, CancellationToken cancelToken)
            {
                var tcs = new TaskCompletionSource<TResult>();
                var reg = cancelToken.Register(() => { tcs.TrySetCanceled(); }); // 取消 Token 时 回调的方法
                task.ContinueWith(ant =>
                {
                    reg.Dispose();
                    if (ant.IsCanceled)
                        tcs.TrySetCanceled();
                    else if (ant.IsFaulted)
                        tcs.TrySetException(ant.Exception.InnerException);
                    else
                        tcs.TrySetResult(ant.Result);
                });
                return tcs.Task;
            }

            async Task<TResult[]> WhenAllOrError<TResult>(params Task<TResult>[] tasks)
            {
                // TaskCompletionSource 对象，在任意一个任务出错时终止最终的任务
                var killJoy = new TaskCompletionSource<TResult[]>();

                foreach (var task in tasks)
                    await task.ContinueWith(ant =>    // ContinueWith 不要访问任务的结果 不需要回弹到 UI 线程
                    {
                        if (ant.IsCanceled)
                            killJoy.TrySetCanceled();
                        else if (ant.IsFaulted)
                            killJoy.TrySetException(ant.Exception.InnerException);
                    });

                return await await Task.WhenAny(killJoy.Task, Task.WhenAll(tasks)); // 执行中存在错误 或 全部正正常执行 
            }
        }
    }
}

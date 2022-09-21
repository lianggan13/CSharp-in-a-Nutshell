using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyAsyncThread
{
    /// <summary>
    /// await/async关键字
    /// 任何一个方法 都可以增加async
    /// await 放在task前面     一般成对出现  只有async是没有意义的  只有await是报错的
    /// await/async 要么不用  要么用到底
    /// </summary>
    public class AwaitAsyncClass
    {
        public static void TestShow()
        {
            Test();
        }


        private static void   Test()
        {
            Console.WriteLine($"当前主线程id={Thread.CurrentThread.ManagedThreadId}");
            {
                NoReturnNoAwait();
            }
            {
                NoReturn();
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(300);
                    Console.WriteLine($"Main Thread Task ManagedThreadId={Thread.CurrentThread.ManagedThreadId} i={i}");
                }
            }
            {
                //Task t = NoReturnTask();
                //Console.WriteLine($"Main Thread Task ManagedThreadId={Thread.CurrentThread.ManagedThreadId}");
                //t.Wait();//主线程等待Task的完成
                //await t;//await后的代码会由线程池的线程执行
            }
            {
                //Task<long> t = SumAsync();
                //Console.WriteLine($"Main Thread Task ManagedThreadId={Thread.CurrentThread.ManagedThreadId}");
                //long lResult = t.Result;//访问result   主线程等待Task的完成
                //t.Wait();//等价于上一行
            }
            {
                //Task<int> t = SumFactory();
                //Console.WriteLine($"Main Thread Task ManagedThreadId={Thread.CurrentThread.ManagedThreadId}");
                //long lResult = t.Result;//没有await和async 普通的task
                //t.Wait();
            }
            //Console.WriteLine($"Test Sleep Start {Thread.CurrentThread.ManagedThreadId}");
            //Thread.Sleep(10000);
            //Console.WriteLine($"Test Sleep End {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine($"Main Thread Task ManagedThreadId={Thread.CurrentThread.ManagedThreadId}");
            Console.Read();
        }

        /// <summary>
        /// 只有async没有await，会有个warn
        /// 跟普通方法没有区别
        /// </summary>
        private static async void NoReturnNoAwait()
        {
            //主线程执行
            Console.WriteLine($"NoReturnNoAwait Sleep before Task,ThreadId={Thread.CurrentThread.ManagedThreadId}");
            Task task = Task.Run(() =>//启动新线程完成任务
            {
                Console.WriteLine($"NoReturnNoAwait Sleep before,ThreadId={Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(3000);
                Console.WriteLine($"NoReturnNoAwait Sleep after,ThreadId={Thread.CurrentThread.ManagedThreadId}");
            });

            //主线程执行
            Console.WriteLine($"NoReturnNoAwait Sleep after Task,ThreadId={Thread.CurrentThread.ManagedThreadId}");
        }

        /// <summary>
        /// async/await 
        /// 不能单独await
        /// await 只能放在task前面
        /// 不推荐void返回值，使用Task来代替
        /// Task和Task<T>能够使用await, Task.WhenAny, Task.WhenAll等方式组合使用。Async Void 不行
        /// </summary>
        private static async Task NoReturn() // 如果返回值为 void,调用者正常调用，如果返回值 是Task,IDE 是提示调用者 是否使用 await 修饰符 进行异步等待  private static async void NoReturn()
        {
            //主线程执行
            Console.WriteLine($"NoReturn Sleep before await,ThreadId={Thread.CurrentThread.ManagedThreadId}");
            TaskFactory taskFactory = new TaskFactory();
            Task task = taskFactory.StartNew(() =>
            {
                Console.WriteLine($"NoReturn Sleep before,ThreadId={Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(3000);
                Console.WriteLine($"NoReturn Sleep after,ThreadId={Thread.CurrentThread.ManagedThreadId}");
            });

            task.ContinueWith(t =>
            {
                Console.WriteLine($"NoReturn Sleep after await,ThreadId={Thread.CurrentThread.ManagedThreadId}");
            });
            await task;//主线程到这里就返回了，执行主线程任务

            //一流水儿的写下去的，耗时任务就用await
            //nodejs
            //子线程执行   其实是封装成委托，在task之后成为回调（编译器功能  状态机实现）
            //task.ContinueWith()
            //这个回调的线程是不确定的：可能是主线程  可能是子线程  也可能是其他线程
            Console.WriteLine($"NoReturn Sleep after await,ThreadId={Thread.CurrentThread.ManagedThreadId}");
        }

        /// <summary>
        /// 无返回值  async Task == async void
        /// Task和Task<T>能够使用await, Task.WhenAny, Task.WhenAll等方式组合使用。Async Void 不行
        /// </summary>
        /// <returns></returns>
        private static async Task NoReturnTask()
        {
            //这里还是主线程的id
            Console.WriteLine($"NoReturnTask Sleep before await,ThreadId={Thread.CurrentThread.ManagedThreadId}");

            Task task = Task.Run(() =>
             {
                 Console.WriteLine($"NoReturnTask Sleep before,ThreadId={Thread.CurrentThread.ManagedThreadId}");
                 Thread.Sleep(3000);
                 Console.WriteLine($"NoReturnTask Sleep after,ThreadId={Thread.CurrentThread.ManagedThreadId}");
             });
            await task;
            Console.WriteLine($"NoReturnTask Sleep after await,ThreadId={Thread.CurrentThread.ManagedThreadId}");

            //return new TaskFactory().StartNew(() => { });  //不能return  没有async才行
        }

        /// <summary>
        /// 带返回值的Task  
        /// 要使用返回值就一定要等子线程计算完毕
        /// </summary>
        /// <returns>async 就只返回long</returns>
        private static async Task<long> SumAsync()
        {
            Console.WriteLine($"SumAsync 111 start ManagedThreadId={Thread.CurrentThread.ManagedThreadId}");
            long result = 0;

            await Task.Run(() =>
            {
                for (int k = 0; k < 10; k++)
                {
                    Console.WriteLine($"SumAsync {k} await Task.Run ManagedThreadId={Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(1000);
                }

                for (long i = 0; i < 999999999; i++)
                {
                    result += i;
                }
            });

            Console.WriteLine($"SumFactory 111   end ManagedThreadId={Thread.CurrentThread.ManagedThreadId}");
            await Task.Run(() =>
            {
                for (int k = 0; k < 10; k++)
                {
                    Console.WriteLine($"SumAsync {k} await Task.Run ManagedThreadId={Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(1000);
                }

                for (long i = 0; i < 999999999; i++)
                {
                    result += i;
                }
            });

            Console.WriteLine($"SumFactory 111   end ManagedThreadId={Thread.CurrentThread.ManagedThreadId}");

            await Task.Run(() =>
            {
                for (int k = 0; k < 10; k++)
                {
                    Console.WriteLine($"SumAsync {k} await Task.Run ManagedThreadId={Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(1000);
                }

                for (long i = 0; i < 999999999; i++)
                {
                    result += i;
                }
            });

            Console.WriteLine($"SumFactory 111   end ManagedThreadId={Thread.CurrentThread.ManagedThreadId}");

            return result;
        }

        /// <summary>
        /// 真的返回Task  不是async  
        /// 
        /// 要使用返回值就一定要等子线程计算完毕
        /// </summary>
        /// <returns>没有async Task</returns>
        private static Task<int> SumFactory()
        {
            Console.WriteLine($"SumFactory 111 start ManagedThreadId={Thread.CurrentThread.ManagedThreadId}");
            TaskFactory taskFactory = new TaskFactory();
            Task<int> iResult = taskFactory.StartNew<int>(() =>
            {
                Thread.Sleep(3000);
                Console.WriteLine($"SumFactory 123 Task.Run ManagedThreadId={Thread.CurrentThread.ManagedThreadId}");
                return 123;
            });
            //Console.WriteLine($"This is {iResult.Result}");
            Console.WriteLine($"SumFactory 111   end ManagedThreadId={Thread.CurrentThread.ManagedThreadId}");
            return iResult;
        }
    }
}

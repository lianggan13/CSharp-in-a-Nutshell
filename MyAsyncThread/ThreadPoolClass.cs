using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyAsyncThread
{
    /// 1 thread提供了太多的API  给三岁小孩一把枪   只给了一把筷子
    /// 2 无限使用线程，，加以限制
    /// 3 重用线程，避免重复的创建和销毁
    public class ThreadPoolClass
    {
        public ThreadPoolClass()
        {
            Console.WriteLine($"****************btnThreadPool_Click Start {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
            ThreadPool.QueueUserWorkItem(t => this.DoSomethingLong("btnThreadPool_Click"));
            ThreadPool.QueueUserWorkItem(t => this.DoSomethingLong("btnThreadPool_Click"));
            ThreadPool.QueueUserWorkItem(t => this.DoSomethingLong("btnThreadPool_Click"));
            ThreadPool.QueueUserWorkItem(t => this.DoSomethingLong("btnThreadPool_Click"));
            ThreadPool.QueueUserWorkItem(t => this.DoSomethingLong("btnThreadPool_Click"));

            Thread.Sleep(10 * 1000);
            Console.WriteLine("前面的计算都完成了。。。。。。。。");
            ThreadPool.QueueUserWorkItem(t => this.DoSomethingLong("btnThreadPool_Click"));
            ThreadPool.QueueUserWorkItem(t => this.DoSomethingLong("btnThreadPool_Click"));
            ThreadPool.QueueUserWorkItem(t => this.DoSomethingLong("btnThreadPool_Click"));
            ThreadPool.QueueUserWorkItem(t => this.DoSomethingLong("btnThreadPool_Click"));
            ThreadPool.QueueUserWorkItem(t => this.DoSomethingLong("btnThreadPool_Click"));



            {
                ThreadPool.GetMaxThreads(out int workerThreads, out int completionPortThreads);
                Console.WriteLine($"GetMaxThreads workerThreads={workerThreads} completionPortThreads={completionPortThreads}");
            }
            {
                ThreadPool.GetMinThreads(out int workerThreads, out int completionPortThreads);
                Console.WriteLine($"GetMinThreads workerThreads={workerThreads} completionPortThreads={completionPortThreads}");
            }
            Console.WriteLine("****************************");
            ThreadPool.SetMaxThreads(16, 16);
            ThreadPool.SetMinThreads(8, 8);
            {
                ThreadPool.GetMaxThreads(out int workerThreads, out int completionPortThreads);
                Console.WriteLine($"GetMaxThreads workerThreads={workerThreads} completionPortThreads={completionPortThreads}");
            }
            {
                ThreadPool.GetMinThreads(out int workerThreads, out int completionPortThreads);
                Console.WriteLine($"GetMinThreads workerThreads={workerThreads} completionPortThreads={completionPortThreads}");
            }
            //ThreadPool啥都没有

            //类  包含了一个bool属性
            //false--WaitOne等待--Set--true--WaitOne直接过去
            //true--WaitOne直接过去--ReSet--false--WaitOne等待
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem(t =>
            {
                this.DoSomethingLong("btnThreadPool_Click");
                manualResetEvent.Set();
                //manualResetEvent.Reset();
            });
            manualResetEvent.WaitOne();

            //一般来说，不要阻塞线程池的线程 二班来说也是如此
            for (int i = 0; i < 20; i++)
            {
                int k = i;
                ThreadPool.QueueUserWorkItem(t =>
                {
                    Console.WriteLine(k);
                    if (k < 18)
                    {
                        manualResetEvent.WaitOne();//设置为false
                    }
                    else
                    {
                        manualResetEvent.Set();
                    }
                });
            }
            if (manualResetEvent.WaitOne())
            {
                Console.WriteLine("没有死锁、、、");
            }

            Console.WriteLine("等着QueueUserWorkItem完成后才执行");

            Console.WriteLine($"****************btnThreadPool_Click End   {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
        }

        /// <summary>
        /// 一个比较耗时耗资源的私有方法
        /// </summary>
        /// <param name="name"></param>
        private void DoSomethingLong(string name)
        {
            Console.WriteLine($"****************DoSomethingLong {name} Start {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
            long lResult = 0;
            for (int i = 0; i < 1000000000; i++)
            {
                lResult += i;
            }
            //Thread.Sleep(2000);
            
            Console.WriteLine($"****************DoSomethingLong {name}   End {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {lResult}***************");
        }
    }
}

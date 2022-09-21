using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyAsyncThread
{
   public class ThreadClass
    {
        public ThreadClass()
        {
            Console.WriteLine($"****************btnThreads_Click Start {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
            for (int i = 0; i < 5; i++)
            {
                new Thread(() => this.DoSomethingLong("btnThreads_Click")).Start();
            }
            Thread.Sleep(10 * 1000);
            Console.WriteLine("前面的计算都完成了。。。。。。。。");
            for (int i = 0; i < 5; i++)
            {
                new Thread(() => this.DoSomethingLong("btnThreads_Click")).Start();
            }


            this.ThreadWithCallback(() => Console.WriteLine($"这里是action  {Thread.CurrentThread.ManagedThreadId.ToString("00")}")
                               , () => Console.WriteLine($"这里是callback  {Thread.CurrentThread.ManagedThreadId.ToString("00")}"));

            Func<int> func = this.ThreadWithReturn<int>(() =>
            {
                Thread.Sleep(2000);
                return DateTime.Now.Millisecond;
            });
            Console.WriteLine("12324546576586789789");

            int iResult = func.Invoke();
            Console.WriteLine(iResult);


            //Action action = () => this.DoSomethingLong("btnThreads_Click");
            ThreadStart threadStart = () => this.DoSomethingLong("btnThreads_Click");
            //Thread thread = new Thread(action);
            Thread thread = new Thread(threadStart);
            thread.Start();
            try
            {
                thread.Abort();//销毁，方式是抛异常   也不建议    不一定及时/有些动作发出收不回来
            }
            catch (Exception)
            {
                Thread.ResetAbort();//取消Abort异常
            }

            //线程等待
            thread.Join(500);//最多等500
            Console.WriteLine("等待500ms");
            thread.Join();//当前线程等待thread完成
      
            // 循环判断 延时等待线程结束
            while (thread.ThreadState != ThreadState.Stopped)
            {
                Thread.Sleep(100);//当前线程 休息100ms  
            }

            //默认是前台线程，启动之后一定要完成任务的，阻止进程退出
            thread.IsBackground = true;//指定后台线程：随着进程退出
            thread.Priority = ThreadPriority.Highest;//线程优先级  
            //CPU会优先执行 Highest   不代表说Highest就最先
            Console.WriteLine($"****************btnThreads_Click End   {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
        }

        //启动子线程计算--完成委托后，该线程去执行后续回调委托
        private void ThreadWithCallback(Action act, Action callback)
        {
            Thread thread = new Thread(() =>
            {
                act.Invoke();
                callback.Invoke();
            });
            thread.Start();
        }


        //带返回的异步调用  需要获取返回值
        /// <summary>
        /// 又要结果 要不阻塞 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        private Func<T> ThreadWithReturn<T>(Func<T> func)
        {
            T t = default(T);
            Thread thread = new Thread(() =>
            {
                t = func.Invoke();
            });
            thread.Start();
            return () =>
            {
                thread.Join();
                return t;
            };
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

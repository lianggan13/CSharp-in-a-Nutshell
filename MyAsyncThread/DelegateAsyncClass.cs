using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyAsyncThread
{
    /// <summary>
    /// 委托异步
    /// </summary>
    public class DelegateAsyncClass
    {
         public DelegateAsyncClass()
        {
            Console.WriteLine($"****************btnAsyncAdvanced_Click Start {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
            Action<string> action = this.DoSomethingLong;

            IAsyncResult asyncResult = null;

            AsyncCallback callback = ia =>
            {
                Console.WriteLine(object.ReferenceEquals(asyncResult, ia));
                Console.WriteLine(ia.AsyncState);
                Console.WriteLine($"到这里计算已经完成了。{Thread.CurrentThread.ManagedThreadId.ToString("00")}。");
            };
            asyncResult = action.BeginInvoke("btnAsyncAdvanced_Click", callback, "hao");
            Console.WriteLine($"到这里计算已经完成了。{Thread.CurrentThread.ManagedThreadId.ToString("00")}。");

            int i = 0;
            while (!asyncResult.IsCompleted)//1 卡界面：主线程忙于等待
            {   //可以等待，边等待边做其他操作
                //可能最多200ms的延迟
                if (i < 10)
                {
                    Console.WriteLine($"文件上传完成{i++ * 10}%..");//File.ReadSize
                }
                else
                {
                    Console.WriteLine($"文件上传完成99.9%..");
                }
                Thread.Sleep(200);
            }
            Console.WriteLine($"上传成功了。。。..");

            Thread.Sleep(200);
            Console.WriteLine("Do Something Else......");
            Console.WriteLine("Do Something Else......");
            Console.WriteLine("Do Something Else......");

            asyncResult.AsyncWaitHandle.WaitOne();//等待任务的完成
            asyncResult.AsyncWaitHandle.WaitOne(-1);//等待任务的完成
            asyncResult.AsyncWaitHandle.WaitOne(1000);//等待；但是最多等待1000ms

            action.EndInvoke(asyncResult);//可以等待
            {
                Func<int> func = () =>
                {
                    Thread.Sleep(2000);
                    return DateTime.Now.Day;
                };
                Console.WriteLine($"func.Invoke()={func.Invoke()}");

                 asyncResult = func.BeginInvoke(r =>
                {
                    func.EndInvoke(r);
                    Console.WriteLine(r.AsyncState);
                }, "冰封的心");

                Console.WriteLine($"func.EndInvoke(asyncResult)={func.EndInvoke(asyncResult)}");
            }

            Console.WriteLine($"全部计算真的都完成了，然后给用户返回{Thread.CurrentThread.ManagedThreadId.ToString("00")}");
            Console.WriteLine($"****************btnAsyncAdvanced_Click End   {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
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

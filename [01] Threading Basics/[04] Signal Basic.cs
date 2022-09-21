using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _01__Threading_Basics
{
   public class _04__Signal_Basic
    {
        public static void Show()
        {
            var signal = new ManualResetEvent(false);
            new Thread(() =>
            {
                Console.WriteLine("Waiting for signal...");
                signal.WaitOne();   // 阻止当前线程，直到当前WaitHandle信号
                signal.Dispose();
                Console.WriteLine("Got signal!");

            }).Start();
            Thread.Sleep(500);
            signal.Set();           // “Open” the signal 将事件状态设置为有信号，从而允许一个或多个等待线程继续执行
        }
    }
}

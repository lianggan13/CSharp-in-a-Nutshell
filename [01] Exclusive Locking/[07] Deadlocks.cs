using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _01__Exclusive_Locking
{
    /// <summary>
    /// 死锁
    /// </summary>
    public  class _07__Deadlocks
    {
        object locker1 = new object();
        object locker2 = new object();

        public  void Show()
        {
            // 先创建线程开始执行
            ThreadProgram();
            // Thread.Sleep(500);   // 中间加个延时，先让线程执行完，即先让线程 把 locker1 locker2 释放掉
            // 线程与主程序并发执行
            MainProgram();  
          
        }

        private void ThreadProgram()
        {
            new Thread(() =>
            {
                lock (locker1)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("Thread wait for locker2");
                    lock (locker2) { Console.WriteLine("Thread quit!"); }
                }
            }).Start();
        }
        private void MainProgram()
        {
            lock (locker2)
            {
                Thread.Sleep(1000);
                Console.WriteLine("MainProgram wait for locker1");
                lock (locker1) { Console.WriteLine("MainProgram quit!"); }
            }
        }

    }
}

using System;

// 排他锁
namespace _01__Exclusive_Locking
{
    class Program
    {
        static void Main(string[] args)
        {
            /* 
              同步
              协调并发操作，得到可以预测结果的行为。
              同步工具：
              1.延续（continuation）+_任务组合器，将并发程序构造为异步操作，减少对🔒和✳发送的依赖
              2.排它🔒：lock,Metux,SpinLock,每次只允许一个线程执行特定的活动或一段代码

              3.非排它🔒：Semaphore,ReaderWriterLock,ReaderWriterLockSlim 实现有限的并发性（有并发容量限制）
              4.信号✳发送：ManualResetEvent,AutoResetEvent,CountdownEvent,Barrier,允许线程在接到一个
              或者多个其他线程通知之前保持阻塞状态
            */
            /*
             线程安全
                应用程序或方法在任意多线程的场景下正确执行。
             如何确保线程安全？
                · 在同一时刻需要访问可写的共享字段时，加🔒(lock,Metux,SpinLock)或者发送信号✳
                · 降低线程间的交互性，减少数据共享
                · 对于客户端应用程序，将访问的共享状态放在UI线程上
              */


            _05__Nested_locking.Show();


            //_07__Deadlocks deadlock = new _07__Deadlocks();
            //deadlock.Show();

            //_10__Mutex.Show();

            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
        }
    }
}

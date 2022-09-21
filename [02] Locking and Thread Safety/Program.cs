using System;

namespace _02__Locking_and_Thread_Safety
{
    /// <summary>
    /// 锁和安全性
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //_05__Thread_safety_in_application_servers.Show();

            _10__Semaphore.Show();

            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
        }
    }
}

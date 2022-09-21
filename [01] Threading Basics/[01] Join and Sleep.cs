using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _01__Threading_Basics
{
    public class _01__Join_and_Sleep
    {
        public static void Show()
        {
            CreateThread();
        }
        public static void CreateThread()
        {
            Thread t = new Thread(WriteY);
            t.Start();
            t.Join();   
            for (int i = 0; i < 1000; i++) Console.Write("x");
            Thread.Sleep(500);   // Sleep the current thread for 500 milliseconds
            Console.WriteLine("Thread t has ended!");
        }

        private static void WriteY()
        {
            for (int i = 0; i < 1000; i++) Console.Write("y");
        }
    }
}

using System;
using System.Threading;

namespace _40__EXTRA___Wait_and_Pulse
{
    public class _01__Signaling_with_Wait_and_Pulse
    {
        // See http://www.albahari.com/threading/part4.aspx ("Signaling with Wait and Pulse") for the accompanying text.
        /* lock(obj){} = Monitor.Enter(obj) + Monitor.Exit(obj) */

        static readonly object _locker = new object();
        static bool _go;

        public static void Show()
        {                                // The new thread will block because _go==false.
            new Thread(Work).Start();
            Console.WriteLine("Press Enter to signal");
            Console.ReadLine();          // Wait for user to hit Enter

            lock (_locker)                 // Let's now wake up the thread by
            {                              // setting _go=true and pulsing.
                Console.WriteLine("Countinue...");
                _go = true;
                Monitor.Pulse(_locker);
            }
        }

        static void Work()
        {
            lock (_locker)
            {
                while (!_go)
                {
                    Monitor.Wait(_locker);    // Lock is released while we’re waiting
                    Thread.Sleep(3000);
                }
            }
            Console.WriteLine("Woken!!!");
        }
    }
}

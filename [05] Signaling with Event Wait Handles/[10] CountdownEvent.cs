using System;
using System.Threading;

namespace _05__Signaling_with_Event_Wait_Handles
{
    /// <summary>
    /// 等待多个线程
    /// </summary>
    public class _10__CountdownEvent
    {
        static CountdownEvent _countdown = new CountdownEvent(3);

        public static void Show()
        {
            new Thread(SaySomething).Start("I am thread 1");
            new Thread(SaySomething).Start("I am thread 2");
            new Thread(SaySomething).Start("I am thread 3");
            _countdown.Wait();   // Blocks until Signal has been called 3 times
            Console.WriteLine("All threads have finished speaking!");
        }

        static void SaySomething(object thing)
        {
            Thread.Sleep(1000);
            Console.WriteLine(thing);
            _countdown.Signal();
            //_countdown.Reset();
        }
    }
}

using System;
using System.Threading;

namespace _05__Signaling_with_Event_Wait_Handles
{
    /// <summary>
    /// 信号发送
    /// </summary>
    public class _01__AutoResetEvent
    {
        static EventWaitHandle _waitHandle = new AutoResetEvent(false);// true 终止 Set() 句柄为打开状态  false 非终止

        public static void Show()
        {
            new Thread(Waiter).Start();
            Thread.Sleep(3000);                  // Pause for a second...
            _waitHandle.Set();                    // Wake up the Waiter.
        }

        static void Waiter()
        {
            Console.WriteLine("Waiting...");
            _waitHandle.WaitOne();                // Wait for notification
            Console.WriteLine("Notified");
        }
    }
}

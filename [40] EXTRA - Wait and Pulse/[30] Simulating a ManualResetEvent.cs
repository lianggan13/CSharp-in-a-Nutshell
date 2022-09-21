using System;
using System.Threading;

namespace _40__EXTRA___Wait_and_Pulse
{
    public class _30__Simulating_a_ManualResetEvent
    {
        public static void Show()
        {
            CustomManualResetEvent e = new CustomManualResetEvent();
            new Thread(() => { Thread.Sleep(2000); e.Set(); }).Start();
            Console.WriteLine("Waiting...");
            e.WaitOne();
            Console.WriteLine("Signaled");
        }
    }

    public class CustomManualResetEvent
    {
        private readonly object _locker = new object();
        bool _signal;

        public void WaitOne()
        {
            lock (_locker)
            {
                while (!_signal) Monitor.Wait(_locker);
                //Reset();
            }
        }
        public void Set()
        {
            lock (_locker)
            {
                _signal = true;
                Monitor.PulseAll(_locker);
            }
        }
        public void Reset()
        {
            lock (_locker)
            {
                _signal = false;
            }
        }
    }
}

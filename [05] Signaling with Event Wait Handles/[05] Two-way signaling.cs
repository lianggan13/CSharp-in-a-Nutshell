using System;
using System.Threading;

namespace _05__Signaling_with_Event_Wait_Handles
{
    /// <summary>
    /// 双向信号
    /// </summary>
    public class _05__Two_way_signaling
    {
        static EventWaitHandle _ready = new AutoResetEvent(false);
        static EventWaitHandle _go = new AutoResetEvent(false);
        static readonly object _locker = new object();
        static string _message;

        public static void Show()
        {
            new Thread(Work).Start();

            _ready.WaitOne();
            lock (_locker) _message = "Tom kicks the ball to Bob";
            _go.Set();

            _ready.WaitOne();
            lock (_locker) _message = "Bob kicks the ball to Tom";
            _go.Set();

            _ready.WaitOne();
            lock (_locker) _message = "Tom kicks the ball away...";
            _go.Set();

            _ready.WaitOne();
            lock (_locker) _message = null;
            _go.Set();
        }

        static void Work()
        {
            while (true)
            {
                _ready.Set();                          // Indicate that we're ready
                _go.WaitOne();                         // Wait to be kicked off...
                lock (_locker)
                {
                    if (_message == null) return;        // Gracefully exit
                    Console.WriteLine(_message);
                }
            }
        }
    }
}

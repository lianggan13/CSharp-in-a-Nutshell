using System;
using System.Threading;

namespace _40__EXTRA___Wait_and_Pulse
{
    public class _40__Thread_rendezvous
    {
        static object _locker = new object();

        static CountdownEvent _countdown = new CountdownEvent(2);

        public static void Show()
        {
            // Get each thread to sleep a random amount of time.
            Random r = new Random();
            new Thread(Mate).Start(r.Next(10000));
            Thread.Sleep(r.Next(10000));

            _countdown.Signal();
            _countdown.Wait();

            Console.WriteLine("Show");
        }

        static void Mate(object delay)
        {
            Thread.Sleep((int)delay);

            _countdown.Signal();
            _countdown.Wait();

            Console.WriteLine("Mate! ");
        }
    }
}

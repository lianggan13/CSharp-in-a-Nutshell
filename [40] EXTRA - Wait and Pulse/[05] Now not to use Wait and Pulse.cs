using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _40__EXTRA___Wait_and_Pulse
{
    public class _05__Now_not_to_use_Wait_and_Pulse
    {
        // Non-deterministic!

        static readonly object _locker = new object();

       public static  void Show()
        {
            new Thread(Work).Start();
            lock (_locker) Monitor.Pulse(_locker);
        }

        static void Work()
        {
            lock (_locker) Monitor.Wait(_locker);
            Console.WriteLine("Woken!!!");
        }
    }
}

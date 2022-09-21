using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _20__Thread_local_Storage
{
   public class _05__ThreadLocal
    {
        static ThreadLocal<int> _x = new ThreadLocal<int>(() => 3);

       public static  void Show()
        {
            new Thread(() => { Thread.Sleep(1000); _x.Value++; _x.Dump(); }).Start();
            new Thread(() => { Thread.Sleep(2000); _x.Value++; _x.Dump(); }).Start();
            new Thread(() => { Thread.Sleep(3000); _x.Value++; _x.Dump(); }).Start();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace _20__Thread_local_Storage
{
    public class _01__ThreadStatic
    {

        [ThreadStatic] 
        static int _x;

         void Show()
        {
            new Thread(() => { Thread.Sleep(1000); _x++; _x.Dump(); }).Start();
            new Thread(() => { Thread.Sleep(2000); _x++; _x.Dump(); }).Start();
            new Thread(() => { Thread.Sleep(3000); _x++; _x.Dump(); }).Start();
        }
    }
}

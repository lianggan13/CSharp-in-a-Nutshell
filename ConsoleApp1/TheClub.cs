using System;
using System.Threading;

namespace ConsoleApp1
{
    /// <summary>
    /// 俱乐部
    /// </summary>
    public class TheClub
    {
        //static SemaphoreSlim _sem = new SemaphoreSlim(3, 5);
        static SemaphoreSlim _sem = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        public void Enter(object id)
        {
            Console.WriteLine(id + " wants to enter");
            _sem.Wait();
            //_sem.Wait(timeout: TimeSpan.FromSeconds(5));
            Console.WriteLine(id + " is in!");
            Thread.Sleep(1000 * (int)id);
            Console.WriteLine(id + " is leaving!");
            _sem.Release();
            _sem.Release();
        }
    }
}

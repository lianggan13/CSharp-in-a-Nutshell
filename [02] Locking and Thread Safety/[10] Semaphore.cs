using System;
using System.Threading;
using System.Threading.Tasks;

namespace _02__Locking_and_Thread_Safety
{
    /*
       *  信号量可用于限制并发数，防止太多的线程同时执行特定的代码
          如同俱乐部，有特定的容量，还有门卫保护，一旦满员之后，
       不允许他人进入，人们只能在外面排队，当有人离开时，才准许
       另外一个人进入
       */
    /// <summary>
    /// 信号量(非排它锁)
    /// </summary>
    public static class _10__Semaphore
    {
        public static void Show()
        {
            //Club.Show();

            Downloader.Show();
        }

    }

    public static class Club
    {
        public static void Show()
        {
            // 5个线程试图进入聚乐部，但最多只允许3个线程同时进入
            for (int i = 1; i <= 5; i++)
            {
                int id = i;
                new Thread(Enter).Start(id);
            }
        }

        static SemaphoreSlim _sem = new SemaphoreSlim(3);    // Capacity of 3

        static void Enter(object id)
        {
            Console.WriteLine(id + " wants to enter");
            _sem.Wait();
            Console.WriteLine(id + " is in!");           // Only three threads
            Thread.Sleep(1000 * (int)id);               // can be here at
            Console.WriteLine(id + " is leaving");       // a time.
            _sem.Release();
        }
    }

    public static class Downloader
    {
        public static void Show()
        {
            for (int i = 0; i < 13; i++)
            {
                Download(i).ContinueWith(c => Console.WriteLine(c.Result[0]));
            }

        }

        static SemaphoreSlim _sem = new SemaphoreSlim(3);

        static async Task<byte[]> Download(int i)
        {
            await _sem.WaitAsync().ConfigureAwait(false);
            var result = await Task.Run<byte[]>(() =>
            {
                Thread.Sleep(1000 * 5);
                return new byte[] { (byte)i };
            });
            _sem.Release();
            return result;
        }
    }


}

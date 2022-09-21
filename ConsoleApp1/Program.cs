using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        private static object locker1 = new object();
        private static object locker2 = new object();
        static void Main(string[] args)
        {
            //var signal = new ManualResetEvent(false);
            //int x = 0;
            //new Thread(() =>
            //{
            //    { x++; Thread.Sleep(5000); signal.Set(); }
            //}).Start();
            //signal.WaitOne();
            //Console.WriteLine(x);

            // 两个线程互相等待对方占用的资源就会导致双方无法继续执行，从而产生死锁
            #region 死锁
            //new Thread(() =>
            // {
            //     lock (locker1)
            //     {
            //         Console.WriteLine("Thread Id:" + Thread.CurrentThread.ManagedThreadId.ToString("00") + " 已获取 locker1");
            //         Thread.Sleep(2000); x++;
            //         Console.WriteLine("Thread Id:" + Thread.CurrentThread.ManagedThreadId + " 等待 locker2 释放");
            //         lock (locker2)
            //         {
            //             Console.WriteLine("Thread Id:" + Thread.CurrentThread.ManagedThreadId + " 已获取 locker2");
            //         }
            //     }
            // }).Start();

            //lock (locker2)
            //{
            //    Console.WriteLine("Thread Id:" + Thread.CurrentThread.ManagedThreadId + " 已获取 locker2");
            //    Thread.Sleep(2000); x++;
            //    Console.WriteLine("Thread Id:" + Thread.CurrentThread.ManagedThreadId + " 等待 locker1 释放");
            //    lock (locker1)
            //    {
            //        Console.WriteLine("Thread Id:" + Thread.CurrentThread.ManagedThreadId + " 已获取 locker1");
            //    }
            //} 
            #endregion


            // Mutex 可以跨进程
            //using (var mutex = new Mutex(true, "I'm mutex lock"))
            //{
            //    if (mutex.WaitOne(TimeSpan.FromSeconds(20), false))
            //    {
            //        Console.WriteLine("Ohoh,wait time out,I gotta to go ,bye!"); return;
            //    }
            //    try { RunProgram(); }
            //    finally { mutex.ReleaseMutex(); }
            //};

            //private static void RunProgram()
            //{
            //    Console.WriteLine("Running.Press Enter to exit");
            //    Console.ReadKey();
            //}

            // 信号 俱乐部
            //TheClub club = new TheClub();
            //for (int i = 0; i < 5; i++) new Thread(club.Enter).Start(i);
            // 读写锁
            //ReadWriteLockDemo rwLockDemo = new ReadWriteLockDemo();
            //new Thread(rwLockDemo.Read).Start();
            //new Thread(rwLockDemo.Read).Start();
            //new Thread(rwLockDemo.Read).Start();

            //new Thread(rwLockDemo.Write).Start();
            //new Thread(rwLockDemo.Write).Start();

            //WaitHandleDelegate waitHandle = new WaitHandleDelegate();
            //WaitHandleDelegate.Show();

            BaeeierDemo.Show();// baeeier = new BaeeierDemo();
          
            Console.ReadKey();
        }

        


    }

    public class UserCache
    {
        static Dictionary<int, User> _users = new Dictionary<int, User>();
        /// <summary>
        /// 频繁调用 缓存，提高性能
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User GetUser(int id)
        {
            User u = null;
            lock (_users) if (_users.TryGetValue(id, out u)) return u;

            // 并发查询数据库
            u = RetrieveUser(id);

            lock (_users) _users.Add(id, u);
            return u;
        }

        private User RetrieveUser(int id)
        {
            return new User() { id = 1, Name = "lianggan#" + id };
        }
    }

    public class User
    {
        public string Name { get; internal set; }
        public int id { get; internal set; }
    }

    /// <summary>
    /// 俱乐部
    /// </summary>
    public class TheClub
    {
        static SemaphoreSlim _sem = new SemaphoreSlim(3, 5);
        public void Enter(object id)
        {
            Console.WriteLine(id + " wants to enter");
            _sem.Wait();
            Console.WriteLine(id + " is in!");
            Thread.Sleep(1000 * (int)id);
            Console.WriteLine(id + " is leaving!");
            _sem.Release();
        }
    }

    /// <summary>
    /// 读写并发
    /// </summary>
    public class ReadWriteLockDemo
    {
        static ReaderWriterLockSlim _rw = new ReaderWriterLockSlim();
        static List<int> _items = new List<int>();
        static Random _random = new Random();
        public void Read()
        {
            while (true)
            {
                // 支持并发【读】操作
                _rw.EnterReadLock();               
                StringBuilder sb = new StringBuilder();
                foreach (var item in _items)
                {
                    sb.Append(item + ",");
                    Thread.Sleep(100);
                }
                Console.WriteLine(string.Format("Thread[{0}] Read data:{1}", Thread.CurrentThread.ManagedThreadId,sb.ToString()));
                _rw.ExitReadLock();
            }
        } 
        public void Write()
        {
            while (true)
            {               
                Console.WriteLine(string.Format("当前共 {0} readers", _rw.CurrentReadCount));
                int newNum = GetRandNum(100);
                _rw.EnterUpgradeableReadLock();   // 调用 【可升级锁🔒】
                if (!_items.Contains(newNum))       // 判断该元素是否存在列表中
                {
                    _rw.EnterWriteLock();           // 【可升级🔒转化为 写锁🔒】
                    _items.Add(newNum);
                    _rw.ExitWriteLock();
                    Console.WriteLine(string.Format("Thread[{0}] Add item:{1}", Thread.CurrentThread.ManagedThreadId, newNum));
                }
                 _rw.ExitUpgradeableReadLock();
                Thread.Sleep(100);
            }
           
        }
        private int GetRandNum(int maxNum) { lock (_random) return _random.Next(maxNum); }
    }

    /// <summary>
    /// 事件等待句柄 双向句柄
    /// </summary>
    public class TwoWaySingaling
    {
        static EventWaitHandle _ready = new AutoResetEvent(false);
        static EventWaitHandle _go = new AutoResetEvent(false);
        static readonly object _locker = new object();
        static string _cmd;

        public  void Leader()
        {
            new Thread(Worker).Start();
            _ready.WaitOne();
            _cmd = "Let the desk away.";
            _go.Set();

            _ready.WaitOne();
            _cmd = "Check report.";
            _go.Set();

            _ready.WaitOne();
            _cmd = null;
            _go.Set();

        }
        
        public void Worker()
        {
            while (true)
            {
                _ready.Set();
                _go.WaitOne();
                lock (_cmd)
                {
                    if (string.IsNullOrEmpty(_cmd)) return;
                    Console.WriteLine(_cmd+" [Done]");
                }
            }
        }
    }

    /// <summary>
    /// 不等待一个句柄从而阻塞线程 
    /// </summary>
    public static class WaitHandleDelegate
    {
        private static ManualResetEvent _starter = new ManualResetEvent(false);
        public static void Show()
        {
            RegisteredWaitHandle reg = ThreadPool.RegisterWaitForSingleObject(_starter, Go, "Some data", -1, true);
            Thread.Sleep(5000);
            Console.WriteLine("Signaling worker...");
            //_starter.Set();
            //Console.ReadLine();
            //reg.Unregister(_starter);
            // 在等待句柄上附加延续
            _starter.ToTask().ContinueWith(o =>
            {
                Console.WriteLine("Done!");
            });
        }

        private static void Go(object state, bool timedOut)
        {
            Console.WriteLine("Started - " + state);
        }
        /// <summary>
        /// 将等待句柄转化为任务
        /// </summary>
        /// <param name="waitHandle"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static Task<bool> ToTask(this WaitHandle waitHandle, int timeout = -1)
        {
            var tsc = new TaskCompletionSource<bool>();       
            var tokenReady = new ManualResetEventSlim();
            RegisteredWaitHandle token = null; 
              token  = ThreadPool.RegisterWaitForSingleObject(waitHandle,(state,timeOut)
                =>
            {
                tokenReady.Wait();tokenReady.Dispose();
                token.Unregister(waitHandle);
                tsc.SetResult(!timeOut);
            },null,timeout,true);
            tokenReady.Set();
            return tsc.Task;
        }
        
        
    
    }

    /// <summary>
    /// 线程屏障 允许多个线程在同一时刻汇合
    /// </summary>
    public class BaeeierDemo
    {
        static Barrier _barrier = new Barrier(3, barrier => Console.WriteLine("------------"));
        public static void Show()
        {
            new Thread(Speak).Start();
            new Thread(Speak).Start();
            new Thread(Speak).Start();

        }
        
        static void Speak()
        {
            for (int i = 0; i < 13; i++)
            {
                Console.Write(i + " ");
                _barrier.SignalAndWait();
            }
        }
    }
}

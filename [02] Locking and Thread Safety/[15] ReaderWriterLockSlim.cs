using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _02__Locking_and_Thread_Safety
{
    /// <summary>
    /// 读写锁
    /// </summary>
     public  class _15__ReaderWriterLockSlim
    {
		/*          
         在大量的读操作和少量写操作环境下减少🔒竞争
	        · 【写锁】全局排它锁
	        · 【读锁】兼容其它的读锁
	        · 【可升级锁】在有必要写操作时，读锁转化为写锁时间
	     应用：System.InvalidOperationException:“集合已修改；可能无法执行枚举操作。”
         List 是线程不安全 ，即使枚举它，也是不安全的
         因为在枚举它时，若被其它线程修改，就不能进行枚举操作
         解决：使用读/写锁🔒
         */

		static ReaderWriterLockSlim _rw = new ReaderWriterLockSlim();
		static List<int> _items = new List<int>();
		static Random _rand = new Random();

		static void Show()
		{
			new Thread(Read).Start();
			new Thread(Read).Start();
			new Thread(Read).Start();

			new Thread(Write).Start("A");
			new Thread(Write).Start("B");

			new Thread(WriteWithUpgradeableReadLock).Start("C");
			new Thread(WriteWithUpgradeableReadLock).Start("D");
			new Thread(WriteWithUpgradeableReadLock).Start("E");
		}

		static void Read()
		{
			while (true)
			{
				_rw.EnterReadLock();	// 读锁
				foreach (int i in _items) Thread.Sleep(10);
				_rw.ExitReadLock();
			}
		}

		static void Write(object threadID)
		{
			while (true)
			{
				int newNumber = GetRandNum(100);
				_rw.EnterWriteLock();	// 写锁
				_items.Add(newNumber);
				_rw.ExitWriteLock();
				Console.WriteLine(_rw.CurrentReadCount + " concurrent readers");// 有多个并发读锁
				Console.WriteLine("Thread " + threadID + " added " + newNumber);
				Thread.Sleep(100);
			}
		}

		static void WriteWithUpgradeableReadLock(object threadID)
		{
			while (true)
			{
				int newNumber = GetRandNum(100);
				_rw.EnterUpgradeableReadLock();	// 可升级锁
				if (!_items.Contains(newNumber))
				{
					_rw.EnterWriteLock();		// 可升级锁 转化 为死锁
					_items.Add(newNumber);
					_rw.ExitWriteLock();
				Console.WriteLine("Thread " + threadID + " added " + newNumber);
				}
				_rw.ExitUpgradeableReadLock();
				Thread.Sleep(100);


			}
		}

		static int GetRandNum(int max) { lock (_rand) return _rand.Next(max); }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _02__Locking_and_Thread_Safety
{
    /// <summary>
    /// 读写锁递归
    /// </summary>
 public    class _25__ReaderWriterLockSlim___lock_recursion
    {
        public static void Show()
        {
            var rw = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            rw.EnterReadLock();
            rw.EnterReadLock();
            rw.ExitReadLock();
            rw.ExitReadLock();

            rw.EnterWriteLock();
            rw.EnterReadLock();
            Console.WriteLine(rw.IsReadLockHeld);     // True
            Console.WriteLine(rw.IsWriteLockHeld);    // True
            rw.ExitReadLock();
            rw.ExitWriteLock();
        }
    }
}

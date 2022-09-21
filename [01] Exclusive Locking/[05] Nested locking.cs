using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01__Exclusive_Locking
{
    /// <summary>
    /// 嵌套锁
    /// </summary>
   public class _05__Nested_locking
    {
        static readonly object _locker = new object();
        public static void Show()
        {
            lock (_locker)
            {
                AnotherMethod();
                // We still have the lock - because locks are reentrant(重入).
            }
        }

        private static void AnotherMethod()
        {
            lock (_locker) Console.WriteLine("Another method");
        }
    }
}

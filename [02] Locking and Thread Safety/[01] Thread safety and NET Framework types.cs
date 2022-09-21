using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _02__Locking_and_Thread_Safety
{
    /// <summary>
    /// 线程安全与.Net类型
    /// </summary>
    public  class _01__Thread_safety_and_NET_Framework_types
    {
        /* 
         .Net中对集合的枚举（如：List<T>）不是线程安全的。
         在枚举的过程中修改列表则枚举操作会产生异常
          静态成员（如：DateTime）是线程安全的，而实例化
         成员不具备线程安全性   
          只读线程安全性：集合的并发读是线程安全的
          .Net引入了线程安全的队列、
栈和字典
          注意：在只读的过程中，进行任何写✍操作时要确保线程的安全性，
         如Random.Next()方法在内部实现需要更新私有种子字段的值

             */
        static List<string> m_list = new List<string>();
        public static void Show()
        {
            new Thread(AddItem).Start();
            new Thread(AddItem).Start();
        }

        private static void AddItem()
        {
            lock (m_list) m_list.Add("Item" + m_list.Count());

            string[] items;
            lock (m_list) items = m_list.ToArray();
            foreach (string s in items) Console.WriteLine(s);
        }
    }
}

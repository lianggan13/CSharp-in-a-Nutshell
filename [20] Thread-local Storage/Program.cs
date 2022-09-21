using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20__Thread_local_Storage
{
    /// <summary>
    /// 线程本地存储
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            /*
                优化并行代码
                允许每一个线程无须使用锁独立访问属于该线程的对象
                线程本地存储并不适合在异步代码中使用，因为一些延
                续可能会运行在之前的线程上
                 · [ThreadStatic] 特性
                 · ThreadLocal<T> 类
                 · GetData SetData 方法
             
             
             */
            _10__GetData_and_SetData.Show();

            Console.ReadKey();
        }
    }
}

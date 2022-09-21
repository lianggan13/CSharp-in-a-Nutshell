using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _03__Task_Parallelism
{
   public class _06__Working_with_AggregateException
    {
        public static void Show()
        {
            // AggregateException
            {
                // 并行化 异常会在一个独立的线程抛出 跳过 Catch 代码块导致 程序崩溃
                // AggregateException 捕获全部异常并重新抛给调用者
                try
                {
                    var query = from i in ParallelEnumerable.Range(0, 1000000)
                                select 100 / i;
                    // Enumerate query
                    query.Dump();
                }
                catch (AggregateException aex)
                {
                    foreach (Exception ex in aex.InnerExceptions)
                        Console.WriteLine(ex.Message);
                }
            }
            // Flatten
            {
                // 消除任意层级的嵌套 展平内部异常列表
                try
                {
                    var query = from i in ParallelEnumerable.Range(0, 1000000)
                                select 100 / i;
                    // Enumerate query
                    query.Dump();
                }
                catch (AggregateException aex)
                {
                    foreach (Exception ex in aex.Flatten().InnerExceptions)
                        ex.Dump();
                }
            }
            // Handle
            {
                // 捕获特定异常 重新抛出异常类型
                var parent = Task.Factory.StartNew(() =>
                {
                    // We’ll throw 3 exceptions at once using 3 child tasks:

                    int[] numbers = { 0 };

                    var childFactory = new TaskFactory
                    (TaskCreationOptions.AttachedToParent, TaskContinuationOptions.None);

                    childFactory.StartNew(() => 5 / numbers[0]);   // Division by zero
                    childFactory.StartNew(() => numbers[1]);      // Index out of range
                    childFactory.StartNew(() => { throw null; });  // Null reference
                });

                try { parent.Wait(); }
                catch (AggregateException aex)
                {
                    aex.Flatten().Handle(ex =>   // Note that we still need to call Flatten
                    {
                        if (ex is DivideByZeroException)
                        {
                            Console.WriteLine("Divide by zero");
                            return true;                           // This exception is "handled"
                        }
                        if (ex is IndexOutOfRangeException)
                        {
                            Console.WriteLine("Index out of range");
                            return true;                           // This exception is "handled"   
                        }
                        return false;    // All other exceptions will get rethrown
                    });
                }

            }
        }
    }
}

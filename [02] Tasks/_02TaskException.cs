using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02__Tasks
{
   public class _02TaskException
    {
        public async static void Show()
        {
            // Start a Task that throws a NullReferenceException:
            // TaskAggregateException
            {
                TaskAggregateException();
            }

            {
              // await TaskNotSupportedException();
            }
        }
        private static void TaskAggregateException()
        {
            Task task = Task.Run(() => throw new NullReferenceException("null"));
            try
            {
                task.Wait();
            }
            catch (AggregateException aex)
            {
                if (aex.InnerException is NullReferenceException)
                {
                    Console.WriteLine("NullReferenceException 异常:" + aex.Message);
                    Console.WriteLine(aex.Flatten().InnerException);    // 平展异常(异常的嵌套层次) 
                    aex.Handle(ex =>
                    {
                        Console.WriteLine("NullReferenceException 异常已处理");
                        return true;
                    });
                }
                  
                else throw;
            }
        }

        private async static Task TaskNotSupportedException()
        {
            Task task = Task.Run(() => throw new NotSupportedException("NotSupportedException"));
            try
            {
                await task;
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

        }
    }
}

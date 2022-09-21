using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _02__Tasks
{
  public  class _01__Wait
    {
        public static void Show()
        {
            // Wait
            {
                Task task = Task.Run(() =>
                {
                    Console.WriteLine("Task started");
                    Thread.Sleep(2000);
                    Console.WriteLine("Foo");
                });
                Console.WriteLine(task.IsCompleted);  // False
                task.Wait();  // Blocks until task is complete
            }
            // Long-running
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Task started");
                    Thread.Sleep(2000);
                    Console.WriteLine("Foo");
                }, TaskCreationOptions.LongRunning);

                task.Wait();  // Blocks until task is complete
            }
            // Return
            {
                Task<int> task = Task.Run(() => { Console.WriteLine("Foo"); return 3; });
                int result = task.Result;
                Console.WriteLine(result);

                Task<int> primeNumberTask = Task.Run(() =>
                    Enumerable.Range(2, 3000000).Count(n =>
                         Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));

                Console.WriteLine("Task running...");
                Console.WriteLine("The answer is " + primeNumberTask.Result);
            }
           
        }
    }
}

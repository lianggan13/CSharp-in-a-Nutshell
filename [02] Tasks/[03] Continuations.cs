using System;
using System.Linq;
using System.Threading.Tasks;

namespace _02__Tasks
{
    public class _03__Continuations
    {
        public static void Show()
        {
            // Continuations - GetAwaiter
            {
                Task<int> primeNumberTask = Task.Run(() =>
                    Enumerable.Range(2, 3000000).Count(n => Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));
                var awaiter = primeNumberTask.GetAwaiter();
                awaiter.OnCompleted(() =>
                {
                    int result = awaiter.GetResult();
                    Console.WriteLine(result);
                    Console.WriteLine("Continuations - GetAwaiter");
                });
                Console.WriteLine("Main Thread Point");
            }
            // Continuations - ContinueWith
            {
                Task<int> primeNumberTask = Task.Run(() =>
                     Enumerable.Range(2, 3000000).Count(n => Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));
                primeNumberTask.ContinueWith(asyncResult =>
                {
                    int result = asyncResult.Result;
                    Console.WriteLine(result);
                    Console.WriteLine("Continuations - ContinueWith");
                });
                Console.WriteLine("Main Thread Point");
            }
        }
    }
}

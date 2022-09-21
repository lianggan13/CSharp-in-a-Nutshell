using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04__Asynchronous_Function
{
   public class _01__Awaiting
    {
        public static void Show()
        {
            DisplayPrimesCount();
        }

       public static async void DisplayPrimesCount()
        {
            int result = await GetPrimesCountAsync(2, 1000000);
            Console.WriteLine(result);
        }

      public static  Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run(() =>
               ParallelEnumerable.Range(start, count).Count(n =>
                 Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));
        }
    }
}

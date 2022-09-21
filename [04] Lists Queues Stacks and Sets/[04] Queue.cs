using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04__Lists_Queues_Stacks_and_Sets
{
   public  class _04__Queue
    {
        public static void Show()
        {
            var q = new Queue<int>();
            q.Enqueue(10);
            q.Enqueue(20);
            int[] data = q.ToArray();         // Exports to an array
            Console.WriteLine(q.Count);      // "2"
            Console.WriteLine(q.Peek());     // "10"
            Console.WriteLine(q.Dequeue());  // "10"
            Console.WriteLine(q.Dequeue());  // "20"
            Console.WriteLine(q.Dequeue());  // throws an exception (queue empty)
        }
    }
}

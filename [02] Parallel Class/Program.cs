using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02__Parallel_Class
{
    class Program
    {
        static void Main(string[] args)
        {

            // Break
            Parallel.ForEach("Hello, world", (c, loopState) =>
            {
                if (c == ',')
                    loopState.Break();
                else
                    Console.Write(c);
            });
            Console.WriteLine();
         
            Console.ReadKey();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04__Lists_Queues_Stacks_and_Sets
{
    public  class _05__BitArray
    {
        public static void Show()
        {
            var bits = new BitArray(2);
            bits[1] = true;
            bits.Xor(bits);               // Bitwise exclusive-OR bits with itself
            Console.WriteLine(bits[1]);   // False
        }
    }
}

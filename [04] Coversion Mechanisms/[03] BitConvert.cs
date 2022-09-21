using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04__Coversion_Mechanisms
{
   public  class _03__BitConvert
    {
        public static void Show()
        {
            foreach (byte b in BitConverter.GetBytes(3.5))
                Console.Write(b + " ");                          // 0 0 0 0 0 0 12 64
        }

    }
}

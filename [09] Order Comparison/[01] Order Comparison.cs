using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _09__Order_Comparison
{
   public  class _01__Order_Comparison
    {
        public static void Show()
        {
            string[] colors = { "Green", "Red", "Blue" };
            Array.Sort(colors);
            foreach (string c in colors) Console.Write(c + " ");   // Blue Green Red 
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _09__Order_Comparison
{
   public  class _02__Order_Interface
    {
        public static void Show()
        {
            // IComparable
            {
                Console.WriteLine("Beck".CompareTo("Anne"));       // 1
                Console.WriteLine("Beck".CompareTo("Beck"));       // 0
                Console.WriteLine("Beck".CompareTo("Chris"));      // -1
            }
            
        }
    }
}

using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _05__Dictionaries
{
   public  class _02__SortedDictionary
    {
        public static  void Show()
        {
            // MethodInfo is in the System.Reflection namespace

            var sorted = new SortedList<string, MethodInfo>();

            foreach (MethodInfo m in typeof(object).GetMethods())
                sorted[m.Name] = m;

            sorted.Keys.Dump("keys");
            sorted.Values.Dump("values");

            foreach (MethodInfo m in sorted.Values)
                Console.WriteLine(m.Name + " returns a " + m.ReturnType);

            Console.WriteLine(sorted["GetHashCode"]);      // Int32 GetHashCode()

            Console.WriteLine(sorted.Keys[sorted.Count - 1]);            // ToString
            Console.WriteLine(sorted.Values[sorted.Count - 1].IsVirtual);  // True
        }
    }
}

using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01__Enumeration
{
    /// <summary>
    /// 集合 与 枚举器,前者 拥有 后者
    /// </summary>
    public class _01__IEnumerable_and_IEnumerator
    {
        public static void Show()
        {
            // GetEnumerator and MoveNext
            {
                string s = "Hello";
                // Because string implements IEnumerable, we can call GetEnumerator():
                IEnumerator rator = s.GetEnumerator();
                while (rator.MoveNext())
                {
                    char c = (char)rator.Current;
                    Console.Write(c + ".");
                }
                Console.WriteLine();
                // Equivalent to:
                foreach (char c in s)
                    Console.Write(c + ".");
            }
            // Disposing
            {
                IEnumerable<char> s = "Hello";
                using (var rator = s.GetEnumerator())
                    while (rator.MoveNext())
                    {
                        Console.Write((char)rator.Current + ".");
                    }
            }
            // IEnumerable 非泛型集合 接口,保证 所有元素类型统一
            {
                Count("the quick brown fix".Split()).Dump();
            }
        }
        public static int Count(IEnumerable e)
        {
            int count = 0;
            foreach (object element in e)
            {
                var subCollection = element as IEnumerable;
                if (subCollection != null)
                    count += Count(subCollection);
                else
                    count++;
            }
            return count;
        }
    }
}

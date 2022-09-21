using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace _08__Equality_Comparison
{
    public class _02__Equals_Interface_and_Method
    {
        public static void Show()
        {
            // public virtual bool Equals(Object obj);
            {
                object x = 5;
                object y = 5;
                Console.WriteLine(x.Equals(y));      // True
            }
            // public static bool Equals(Object objA, Object objB);
            {
                object x = 3, y = 3;
                Console.WriteLine(object.Equals(x, y));   // True
                x = null;
                Console.WriteLine(object.Equals(x, y));   // False
                y = null;
                Console.WriteLine(object.Equals(x, y));   // True
            }
            // ReferenceEquals
            {
                Widget w1 = new Widget();
                Widget w2 = new Widget();
                Console.WriteLine(object.ReferenceEquals(w1, w2));     // False
            }
            // IEquatable
            {
                new Test<int>().IsEqual(3, 3).Dump();
            }

           
        }

        // Here's an example of how we can leverage the virtual Equals mehtod:
        public static bool AreEqual(object obj1, object obj2)
        {
            if (obj1 == null) return obj2 == null;
            return obj1.Equals(obj2);
            // What we've written is in fact equivalent to the static object.Equals method!
        }
        class Widget
        {
            // Let's suppose Widget overrides its Equals method and overloads its == operator such
            // that w1.Equals (w2) would return true if w1 and w2 were different objects.
            /*...*/
        }
        class Test<T> where T : IEquatable<T>
        {
            public bool IsEqual(T a, T b) => a.Equals(b);     // No boxing with generic T
        }
    }
   
}

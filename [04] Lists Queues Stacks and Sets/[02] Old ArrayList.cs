using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04__Lists_Queues_Stacks_and_Sets
{
   public class _02__Old_ArrayList
    {
        public static void Show()
        {
            ArrayList al = new ArrayList();
            al.Add("hello");
            string first = (string)al[0];
            string[] strArr = (string[])al.ToArray(typeof(string));

            al = new ArrayList();
            al.Add("hello");
            first = (string)al[0];

            // We need a clumsy cast to retrieve elements:
            strArr = (string[])al.ToArray(typeof(string));

            // which fails at *runtime* if we get it wrong:
            var runtimeFail = (int)al[0];    // Runtime exception
        }
    }
}

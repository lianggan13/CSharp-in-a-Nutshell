using Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _07__Comparer
{
    public class _03__StringComparer
    {
        public static void Show()
        {
            {
                var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                dict["joe"] = 12345;
                dict["JOE"].Dump();

                string[] names = { "Tom", "HARRY", "sheila" };
                CultureInfo ci = new CultureInfo("en-AU");
                Array.Sort<string>(names, StringComparer.Create(ci, false));
                names.Dump();
            }
        }


    }

}

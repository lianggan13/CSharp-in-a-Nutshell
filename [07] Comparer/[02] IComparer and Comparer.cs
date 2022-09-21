using Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace _07__Comparer
{
    public class _02__IComparer_and_Comparer
    {
        public static void Show()
        {
            {
                var wishList = new List<Wish>();
                wishList.Add(new Wish("Peace", 2));
                wishList.Add(new Wish("Wealth", 3));
                wishList.Add(new Wish("Love", 2));
                wishList.Add(new Wish("3 more wishes", 1));

                wishList.Sort(new PriorityComparer());
                wishList.Dump();
            }
            {
                var dic = new SortedDictionary<string, string>(new SurnameComparerWithCulture(CultureInfo.GetCultureInfo("de-DE")));
                dic.Add("MacPhail", "second!");
                dic.Add("MacWilliam", "third!");
                dic.Add("McDonald", "first!");
                dic.Dump();
            }
        }
    }

    public class PriorityComparer : Comparer<Wish>
    {
        public override int Compare(Wish x, Wish y)
        {
            if (object.Equals(x, y)) return 0;
            return x.Priority.CompareTo(y.Priority);
        }
    }

    public class SurnameComparer : Comparer<string>
    {
        public override int Compare(string x, string y)
       => Normalize(x).CompareTo(Normalize(y));

        string Normalize(string s)
        {
            s = s.Trim().ToUpper();
            if (s.StartsWith("MC")) s = "MAC" + s.Substring(2);
            return s;
        }
    }

    public class SurnameComparerWithCulture : Comparer<string>
    {
        StringComparer strCmp;

        public SurnameComparerWithCulture(CultureInfo ci)
        {
            // Create a case-sensitive, culture-sensitive string comparer
            strCmp = StringComparer.Create(ci, false);
        }

        string Normalize(string s)
        {
            s = s.Trim();
            if (s.ToUpper().StartsWith("MC")) s = "MAC" + s.Substring(2);
            return s;
        }

        public override int Compare(string x, string y)
            => strCmp.Compare(Normalize(x), Normalize(y));
    }



    public class Wish
    {
        public string Name;
        public int Priority;

        public Wish(string name, int priority)
        {
            Name = name;
            Priority = priority;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04__Lists_Queues_Stacks_and_Sets
{
    public  class _06__HashSet
    {
        public static void Show()
        {
			{
				var letters = new HashSet<char>("the quick brown fox");

				Console.WriteLine(letters.Contains('t'));      // true
				Console.WriteLine(letters.Contains('j'));      // false

				foreach (char c in letters) Console.Write(c);   // the quickbrownfx
			}
			Console.WriteLine();
			{
				var letters = new SortedSet<char>("the quick brown fox");

				foreach (char c in letters)
					Console.Write(c);                                    //  bcefhiknoqrtuwx

				Console.WriteLine();

				foreach (char c in letters.GetViewBetween('f', 'j'))
					Console.Write(c);                                    //  fhk
			}

			// IntersectWith 交集
			{
				var letters = new HashSet<char>("the quick brown fox");
				letters.IntersectWith("aeiou");
				foreach (char c in letters) Console.Write(c);     // euio
			}
			// ExceptWith 差集
			{
				var letters = new HashSet<char>("the quick brown fox");
				letters.ExceptWith("aeiou");
				foreach (char c in letters) Console.Write(c);     // th qckbrwnfx
			}
			// SymmetricExceptWith
			{
				var letters = new HashSet<char>("the quick brown fox");
				letters.SymmetricExceptWith("the lazy brown fox");
				foreach (char c in letters) Console.Write(c);     // quicklazy
			}
		}
    }
}

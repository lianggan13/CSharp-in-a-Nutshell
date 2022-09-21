using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _06__Customizable_Collections
{
   public class _03__ReadOnlyCollection
    {
        public static void Show()
        {
			Test t = new Test();

			Console.WriteLine(t.Names.Count);       // 0
			t.AddInternally();
			Console.WriteLine(t.Names.Count);       // 1

			t.Names.Append("test");                    // Compiler error
			((IList<string>)t.Names).Add("test");  // NotSupportedException
		}
    }

	public class Test
	{
		List<string> names;
		public ReadOnlyCollection<string> Names { get; private set; }

		public Test()
		{
			names = new List<string>();
			Names = new ReadOnlyCollection<string>(names);
		}

		public void AddInternally() { names.Add("test"); }
	}

}

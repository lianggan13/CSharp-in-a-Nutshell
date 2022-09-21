using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _15__Lazy_Initialization
{
   public class _05__Intro___with_lock
    {
		 void Show()
		{
			new Foo().Expensive.Dump();
		}

		class Foo
		{
			Expensive _expensive;
			readonly object _expenseLock = new object();

			public Expensive Expensive
			{
				get
				{
					lock (_expenseLock)
					{
						if (_expensive == null) _expensive = new Expensive();
						return _expensive;
					}
				}
			}
		}

		class Expensive {  /* Suppose this is expensive to construct */  }
	}
}

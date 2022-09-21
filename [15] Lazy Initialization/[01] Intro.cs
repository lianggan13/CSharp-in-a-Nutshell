using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _15__Lazy_Initialization
{
   public class _01__Intro
    {
		public static void Show()
		{
			new Foo().Expensive.Dump();
		}

		class Foo
		{
			Expensive _expensive;
			public Expensive Expensive         // Lazily instantiate Expensive
			{
				get
				{
					// 多个线程 同时 做 断言，则会创建不同实例
					if (_expensive == null) _expensive = new Expensive();
					return _expensive;
				}
			}
		}

		class Expensive {  /* Suppose this is expensive to construct */  }
	}
}

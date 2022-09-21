using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _15__Lazy_Initialization
{
  public  class _10__Lazy_of_T
    {
       public static  void Show()
        {
            new Foo().Expensive.Dump();
        }

        class Foo
        {
            // 双检锁 执行一次volatitle读操作，避免在对象初始化后进行锁操作
            Lazy<Expensive> _expensive = new Lazy<Expensive>(() => new Expensive(), true);
            public Expensive Expensive { get { return _expensive.Value; } }
        }

        class Expensive {  /* Suppose this is expensive to construct */  }
    }
}

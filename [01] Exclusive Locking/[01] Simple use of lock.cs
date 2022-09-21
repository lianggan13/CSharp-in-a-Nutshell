using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _01__Exclusive_Locking
{
    class _01__Simple_use_of_lock
    {
		static readonly object _locker = new object();
		static int _val1, _val2;

		public	static void Show()
		{
			new Thread(Go).Start();
			new Thread(Go).Start();
			new Thread(Go).Start();
		}

		static void Go()
		{
			lock (_locker)  // Threadsafe: will never get DivideByZeroException
			{
				if (_val2 != 0) Console.WriteLine(_val1 / _val2);
				_val2 = 0;
			}
		}
	}
}

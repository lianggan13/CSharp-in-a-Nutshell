using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace _20__Thread_local_Storage
{
  public  class _10__GetData_and_SetData
    {
		public static void Show()
		{
			var test = new Test();
			new Thread(() => { Thread.Sleep(1000); test.SecurityLevel++; test.SecurityLevel.Dump(); }).Start();
			new Thread(() => { Thread.Sleep(2000); test.SecurityLevel++; test.SecurityLevel.Dump(); }).Start();
			new Thread(() => { Thread.Sleep(3000); test.SecurityLevel++; test.SecurityLevel.Dump(); }).Start();
		}

		class Test
		{
			// The same LocalDataStoreSlot object can be used across all threads.
			// 数据插槽
			LocalDataStoreSlot _secSlot = Thread.GetNamedDataSlot("securityLevel");

			// This property has a separate value on each thread.
			public int SecurityLevel
			{
				get
				{
					// 从插槽中获取数据
					object data = Thread.GetData(_secSlot);	
					return data == null ? 0 : (int)data;    // null == uninitialized
				}
				set
				{
					// 将数据设置在插槽中
					Thread.SetData(_secSlot, value);
				}
			}
		}
	}
}

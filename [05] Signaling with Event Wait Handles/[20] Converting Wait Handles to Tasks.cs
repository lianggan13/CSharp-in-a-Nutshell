using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _05__Signaling_with_Event_Wait_Handles
{
	/// <summary>
	/// 将等待句柄转为任务
	/// </summary>
  public  class _20__Converting_Wait_Handles_to_Tasks
    {
		async void Show()
		{
			var myWaitHandle = new AutoResetEvent(true);
			await myWaitHandle.ToTask();
			"Done".Dump();
		}

	
	}
}

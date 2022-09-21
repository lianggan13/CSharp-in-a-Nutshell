using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _25__Suspend_and_Resume
{
   public class _01__Obtaining_stack_trace
    {
		public  void Show()
		{
			var thread = new Thread(Foo);
			thread.Start();
			Thread.Sleep(100);
			var trace = GetTraceOnThread(thread);
			if (trace == null)
				"Bad luck, deadlocked!".Dump();
			else
				trace.GetFrames().Dump();
		}

		StackTrace GetTraceOnThread(Thread targetThread)
		{
			StackTrace stackTrace = null;
			var ready = new ManualResetEventSlim();

			new Thread(() =>
			{
				// Backstop to release thread in case of deadlock:
				ready.Set();
				Thread.Sleep(200);
				try { targetThread.Resume(); } catch { }
			}).Start();

			// 挂起一个线程，诊断其线程信息
			ready.Wait();			// 等待接收信号
			targetThread.Suspend();	// 冻结或挂起线程
			try { stackTrace = new StackTrace(targetThread, true); }
			catch { /* Deadlock */ }
			finally
			{
				try { targetThread.Resume(); } // 解冻
				catch { stackTrace = null;  /* Deadlock */  }
			}
			return stackTrace;
		}

		void Foo()
		{
			Thread.Sleep(5000);
		}
	}
}

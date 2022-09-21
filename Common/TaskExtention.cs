using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
  public static class TaskExtention
    {
		public static Task<bool> ToTask(this WaitHandle waitHandle, int timeout = -1)
		{
			var tcs = new TaskCompletionSource<bool>();
			RegisteredWaitHandle token = null;
			var tokenReady = new ManualResetEventSlim();

			token = ThreadPool.RegisterWaitForSingleObject(
				waitHandle,
				(state, timedOut) =>
				{
					tokenReady.Wait();
					tokenReady.Dispose();
					token.Unregister(waitHandle);
					tcs.SetResult(!timedOut);
				},
				null,
				timeout,
				true);

			tokenReady.Set();
			return tcs.Task;
		}
	}
}

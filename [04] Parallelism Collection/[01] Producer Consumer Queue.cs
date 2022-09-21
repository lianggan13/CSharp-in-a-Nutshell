using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _04__Parallelism_Collection
{
   public class _01__Producer_Consumer_Queue
    {
        public static void Show()
        {
            // With Action
            {
				using (var q = new PCQueueWithAction(1))
				{
					q.EnqueueTask(() => "Foo".Dump());
					q.EnqueueTask(() => "Far".Dump());
				}
			}
            // With Tasks
            {
				// 任务的优势：异常传播、返回值、取消操作、调度
				using (var pcQ = new PCQueueWithTask(1))
				{
					Task task1 = pcQ.Enqueue(() => Console.WriteLine("Too"));
					Task task2 = pcQ.Enqueue(() => Console.WriteLine("Easy!"));

					task1.ContinueWith(_ => "Task 1 complete".Dump());
					task2.ContinueWith(_ => "Task 2 complete".Dump());
				}
			}
        }
    }

	public class PCQueueWithAction : IDisposable
	{
		BlockingCollection<Action> _taskQ = new BlockingCollection<Action>();

		public PCQueueWithAction(int workerCount)
		{
			// Create and start a separate Task for each consumer:
			for (int i = 0; i < workerCount; i++)
				Task.Factory.StartNew(Consume);
		}

		public void Dispose() { _taskQ.CompleteAdding(); }

		public void EnqueueTask(Action action) { _taskQ.Add(action); }

		void Consume()
		{
			// This sequence that we’re enumerating will block when no elements
			// are available and will end when CompleteAdding is called.

			foreach (Action action in _taskQ.GetConsumingEnumerable())
				action();     // Perform task.
		}
	}

	public class PCQueueWithTask : IDisposable
	{
		BlockingCollection<Task> _taskQ = new BlockingCollection<Task>();

		public PCQueueWithTask(int workerCount)
		{
			// Create and start a separate Task for each consumer:
			for (int i = 0; i < workerCount; i++)
				Task.Factory.StartNew(Consume);
		}

		public Task Enqueue(Action action, CancellationToken cancelToken = default(CancellationToken))
		{
			var task = new Task(action, cancelToken);
			_taskQ.Add(task);
			return task;
		}

		public Task<TResult> Enqueue<TResult>(Func<TResult> func,
			CancellationToken cancelToken = default(CancellationToken))
		{
			var task = new Task<TResult>(func, cancelToken);
			_taskQ.Add(task);
			return task;
		}

		void Consume()
		{
			foreach (var task in _taskQ.GetConsumingEnumerable())
				try
				{
					if (!task.IsCanceled) task.RunSynchronously();
				}
				catch (InvalidOperationException) { }  // Race condition
		}

		public void Dispose() { _taskQ.CompleteAdding(); }
	}
}

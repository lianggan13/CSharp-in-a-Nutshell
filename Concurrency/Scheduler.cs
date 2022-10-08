using System.Threading.Tasks.Dataflow;
using Utilities;

namespace Concurrency
{
    public class Scheduler
    {
        public static void Show()
        {
            //ConcurrentExclusiveSchedulerPair();

            SynchronizationContext2();
        }

        public static void ConcurrentExclusiveSchedulerPair()
        {
            var schedulerPair = new ConcurrentExclusiveSchedulerPair(
                TaskScheduler.Default, maxConcurrencyLevel: 8);
            TaskScheduler concurrent = schedulerPair.ConcurrentScheduler;
            TaskScheduler exclusive = schedulerPair.ExclusiveScheduler;

            var collections = Enumerable.Range(0, 999);

            ParallelOptions options = new ParallelOptions { TaskScheduler = concurrent };
            Parallel.ForEach(collections, options,
                    matrices => LogHelper.Info($"{matrices}"));
        }

        public static void SynchronizationContext2()
        {
            // fake ui context
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var options = new ExecutionDataflowBlockOptions
            {
                //TaskScheduler = TaskScheduler.Default,
                //TaskScheduler = TaskScheduler.Current,
                TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext(), // UI Thread
            };

            var multiplyBlock = new TransformBlock<int, int>(item => item * 2);
            var displayBlock = new ActionBlock<int>(
                result => LogHelper.Info($"{result}"), options);
            multiplyBlock.LinkTo(displayBlock);

            foreach (var item in Enumerable.Range(0, 3))
            {
                multiplyBlock.Post(item);
            }

            multiplyBlock.Complete();
        }
    }
}

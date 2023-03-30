using System.Diagnostics;

namespace Concurrency
{
    public class Asynchronous
    {
        public static async Task<string> AwaitWithTimeout(HttpClient client, string uri)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            Task<string> downloadTask = client.GetStringAsync(uri);
            Task timeoutTask = Task.Delay(Timeout.InfiniteTimeSpan, cts.Token);

            Task completedTask = await Task.WhenAny(downloadTask, timeoutTask); // task winner 
            if (completedTask == timeoutTask)
                return null;
            return await downloadTask;
        }

        static async Task<int> DelayAndReturnAsync(int value)
        {
            await Task.Delay(TimeSpan.FromSeconds(value));
            return value;
        }

        // Currently, this method prints "2", "3", and "1".
        // The desired behavior is for this method to print "1", "2", and "3".
        public static async Task ProcessTasksAsync()
        {
            // Create a sequence of tasks.
            Task<int> taskA = DelayAndReturnAsync(2);
            Task<int> taskB = DelayAndReturnAsync(3);
            Task<int> taskC = DelayAndReturnAsync(1);
            Task<int>[] tasks = new[] { taskA, taskB, taskC };

            // Await each task in order.
            //foreach (Task<int> task in tasks)
            //{
            //    var result = await task;
            //    Trace.WriteLine(result);
            //}

            var processingTasks = tasks.Select(async t =>
            {
                var result = await t;
                Trace.WriteLine(result);
            });

            await Task.WhenAll(processingTasks);
        }

        public static async Task ResumeOnContextAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));

            // This method resumes within the same context.
            // ui、asp.net's requset/response
        }

        public async Task ResumeWithoutContextAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            // This method discards its context when it resumes.
            // handle data...
        }



    }

}

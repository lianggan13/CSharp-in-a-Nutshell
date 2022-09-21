using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _03__Task_Parallelism
{
    public class _03__Cancelling_Task
    {
        public static void Show()
        {
            var cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            cts.CancelAfter(500);

            Task task = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                token.ThrowIfCancellationRequested();  // Check for cancellation request
            }, token);

            try { task.Wait(); }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.InnerException is TaskCanceledException);  // True
                Console.WriteLine(task.IsCanceled);                             // True
                Console.WriteLine(task.Status);                             // Canceled
            }
        }
    }
}

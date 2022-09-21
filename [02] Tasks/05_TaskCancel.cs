using System;
using System.Threading;
using System.Threading.Tasks;

namespace _02__Tasks
{
    public class _05_TaskCancel
    {
        public static void Show()
        {
            _05_TaskCancel t = new _05_TaskCancel();
            //t.CancelableMethod();
            t.IssueCancelRequestAsync();
        }

        public async Task<int> CancelableMethodAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            return 42;
        }

        private async void IssueCancelRequestAsync()
        {
            var cts = new CancellationTokenSource();
            //var task = CancelableMethodAsync(cts.Token);
            var task1 = Task.Run<int>(async () =>
            {
                await Task.Delay(5000, cts.Token);
                return 1;
            }, cts.Token);

            var task2 = Task.Run<int>(async () =>
            {
                await Task.Delay(5000);
                return 2;
            }, cts.Token);


            // At this point, the operation is happily running.

            // Issue the cancellation request.
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                cts.Cancel();
            });

            // (Asynchronously) wait for the operation to finish.
            try
            {
                //await task1;
                await Task.WhenAll(task1, task2);
                // If we get here, the operation completed successfully
                //  before the cancellation took effect.
            }
            catch (OperationCanceledException ocex)
            {
                // If we get here, the operation was canceled before it completed.
                Console.WriteLine(ocex.Message);
            }
            catch (Exception ex)
            {
                // If we get here, the operation completed with an error
                //  before the cancellation took effect.
                Console.WriteLine(ex.Message);
            }
        }

        public async void CancelableMethod()
        {
            var cts = new CancellationTokenSource();
            //var task = CancelableMethodAsync(cts.Token);
            var task1 = Task.Run(async () =>
            {
                for (int i = 0; i != 100; ++i)
                {
                    Thread.Sleep(1000); // Some calculation goes here.
                    cts.Token.ThrowIfCancellationRequested();
                }
            });


            cts.Cancel();
            cts.CancelAfter(5000);

            try
            {
                await task1;
                // If we get here, the operation completed successfully
                //  before the cancellation took effect.
            }
            catch (OperationCanceledException ocex)
            {
                // If we get here, the operation was canceled before it completed.
                Console.WriteLine(ocex.Message);
            }
            catch (Exception ex)
            {
                // If we get here, the operation completed with an error
                //  before the cancellation took effect.
                Console.WriteLine(ex.Message);
            }
        }
    }

}

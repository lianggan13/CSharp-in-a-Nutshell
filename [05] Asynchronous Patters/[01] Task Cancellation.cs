using System;
using System.Threading;
using System.Threading.Tasks;

namespace _05__Asynchronous_Patters
{
    public class _01__Task_Cancellation
    {
        public async static void Show()
        {
            // 自定义取消令牌
            {
                var myToken = new MyCancellationToken();
                Task.Delay(5000).ContinueWith(ant => myToken.Cancel()); // 异步延时5s(非阻塞)后，执行延续 --- 启动取消令牌
                await new MyTask().Foo(myToken);
            }
            // .Net CancellationTokenSource 与 CancellationToken
            {
                var cancelSource = new CancellationTokenSource();
                Task.Delay(5000).ContinueWith(ant => cancelSource.Cancel());
                await new MyTask().Foo(cancelSource.Token);
            }
            // 
            {
                var cancelSource = new CancellationTokenSource(2000); // // Tell it to cancel in two seconds.
                try { await new MyTask().Foo(cancelSource.Token); }
                catch (OperationCanceledException ocex)
                { Console.WriteLine("Canceled after 2 seconds"); }
            }
        }
    }

    class MyCancellationToken
    {
        public bool IsCancellationRequested { get; private set; }
        public void Cancel() { IsCancellationRequested = true; }
        public void ThrowIfCancellationRequested()
        {
            if (IsCancellationRequested) throw new OperationCanceledException();
        }
    }

    class MyTask
    {
        public async Task Foo(MyCancellationToken myCancellationToken)
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
                await Task.Delay(1000);
                myCancellationToken.ThrowIfCancellationRequested();
            }
        }

        public async Task Foo(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
                await Task.Delay(1000);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }


}

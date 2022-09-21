using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    /// <summary>
    /// 异步取消
    /// </summary>
  public class AsyncCancelOperation
    {
        public void Show()
        {
            var cancelSource = new CancellationTokenSource();
            //Task foo = CancelTask(cancelSource.Token);
            // 立即取消 
            // cancelSource.Cancel();
            // 在另一个任务中取消
            //Task.Delay(3000).ContinueWith(ant => cancelSource.Cancel());
           
            Task task =  CancelTaskFactory(cancelSource.Token);
            Task.Delay(3000).ContinueWith(ant => cancelSource.Cancel());
        }
        public async Task CancelTask(CancellationToken cancellationToken)
        {
            
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
                await Task.Delay(1000, cancellationToken); // 向一个Task任务中传入 【取消令牌】
            }
        }
        public async Task CancelTaskFactory(CancellationToken cancellationToken)
        {
            TaskFactory taskFactory = new TaskFactory();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                await taskFactory.StartNew(() =>
                {
                    Console.WriteLine(i);
                    Thread.Sleep(2000);

                }, cancellationToken);
                //tasks.Add(await taskFactory.StartNew(() =>
                //{
                //    Console.WriteLine(i);
                //    Thread.Sleep(2000);

                //}, cancellationToken));
                //Console.WriteLine(i);
               // await Task.Delay(1000, cancellationToken); // 向一个Task任务中传入 【取消令牌】
            }
        }
    }
}

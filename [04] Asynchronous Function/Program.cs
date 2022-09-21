using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Forms;
using System.Windows.Media;

namespace _04__Asynchronous_Function
{
    class Program
    {
        static void Main(string[] args)
        {
            //_02__Awaiting_in_a_UI.Show();

            //new _03__Returning_Task_of_TResult().Show();

            //AsyncBasic asyncBasic = new AsyncBasic();
            //asyncBasic.Show();

            //ParallelBasic parallelBasic = new ParallelBasic();
            //parallelBasic.Show();

            //DataFlowBasic dataFlow = new DataFlowBasic();
            //dataFlow.Show();

            _07__Async_Stream.Show();

            Console.WriteLine("Main Thread End.");
            Console.ReadKey();
        }
    }

    #region 2.异步编程基础
    // 2.异步编程基础
    public class AsyncBasic
    {
        public async void Show()
        {
            Action action = () => Console.WriteLine("Action result");
            Action result = await DelayResult<Action>(action, TimeSpan.FromSeconds(5));
            result.BeginInvoke(null, null);


            //await ProcessTasksAsync();
            Console.WriteLine("Main" + Thread.CurrentThread.ManagedThreadId);
            await ResumeOnContextAsync();

            await ResumeWithoutContextAsync();

        }
        // 2.1 异步暂停一段时间 场景：简单的超时
        public async Task<T> DelayResult<T>(T result, TimeSpan delay)
        {
            await Task.Delay(delay);
            return result;
        }
        public async Task<string> DownloadStringWithRetries(string uri)
        {
            using (var client = new HttpClient())
            {
                TimeSpan nextDelay = TimeSpan.FromSeconds(1);
                // 重试策略: 重试的延迟时间会逐次增加 防止服务器被太多的重试阻塞
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        return await client.GetStringAsync(uri);
                    }
                    catch
                    {

                    }
                    await Task.Delay(nextDelay);
                    nextDelay += TimeSpan.FromSeconds(i);
                }
                // 最后重试一次 以便让调用知道出错信息
                return await client.GetStringAsync(uri);
            }
        }
        // 2.4 等待一组任务完成 场景：执行几个任务 等待它们全部完成
        public async Task AwaitAllTask()
        {
            Task task1 = Task.FromResult(3);
            Task task2 = Task.FromResult(5);
            Task task3 = Task.FromResult(7);
            await Task.WhenAll(task1, task2, task3);
        }
        public async Task<string> DownAllAsync(IEnumerable<string> urls)
        {
            var httpClient = new HttpClient();
            // 创建任务组 此时任务均未启动
            var downloads = urls.Select(url => httpClient.GetStringAsync(url));

            // 所有任务同步开始
            Task<string>[] downloadTasks = downloads.ToArray();

            // 所有任务已经开始执行  

            // 异步等待所有任务完成 并返回一个 结果任务 completedTask 捕获的异常存储在 completedTask 中
            Task<string[]> completedTask = Task.WhenAll(downloadTasks);
            string[] htmlPages = null;
            try
            {
                // Task 对象被Await调用 异常再次被 引发
                htmlPages = await completedTask;
            }
            catch
            {
                // 捕获任务组异常
                AggregateException ae = completedTask.Exception;
                throw ae;
            }

            return string.Concat(htmlPages);
        }
        // 2.5 等待任意一个任务完成 场景：同时向多个Web服务器请求股票价格 只获取第一个响应的
        public async Task<int> FirstRespondingUrlAsync(string urlA, string urlB)
        {
            var httpClient = new HttpClient();

            // 并发地开始两个下载任务
            Task<byte[]> downloadTaskA = httpClient.GetByteArrayAsync(urlA);
            Task<byte[]> downloadTaskB = httpClient.GetByteArrayAsync(urlB);

            // 等待任意一个任务完成
            Task<byte[]> completedTask = await Task.WhenAny(downloadTaskA, downloadTaskB);

            // 返回从URL得到的长度
            byte[] data = await completedTask;
            return data.Length;
        }
        // 2.6 任务完成时的处理 场景：任务一完成就进行处理 不需要等待其它任务 而不是等待所有任务完成才进行处理
        private async Task<int> DelayAndReturnAsync(int val)
        {
            await Task.Delay(TimeSpan.FromSeconds(val));
            return val;
        }
        public async Task ProcessTasksAsync()
        {
            // 创建任务队列 
            Task<int> taskA = DelayAndReturnAsync(2);
            Task<int> taskB = DelayAndReturnAsync(15);
            Task<int> taskC = DelayAndReturnAsync(1);
            var tasks = new[] { taskA, taskB, taskC };

            // 执行任务队列 期望输出 1 2 15

            // 实际输出 2 15 1
            //foreach (var task in tasks)
            //{
            //    var result = await task;
            //    Console.WriteLine(result);
            //}

            // 实际输出 2 15 1
            //int[] results = await Task.WhenAll(tasks);
            //results.ToList().ForEach(s => Console.WriteLine(s));

            // 实际输出 1 2 15
            var processingTasks = tasks.Select(async t =>
            {
                var result = await t;
                Console.WriteLine(result);  // 相当于任务t 的延续
            });
            await Task.WhenAll(processingTasks.ToArray());

        }
        // 2.7 避免上下文切换
        // 场景：UI线程，拥有大量的Async方法，这些Async方法会在被Await调用后恢复运行时，
        //      切换到UI上下文中，引起性能上的问题
        //      Async方法需要上下文：处理UI元素或ASP.NET 请求/响应
        //      Async方法摆脱上下文：执行后台指令
        //      Async方法部分需要上下，部分摆脱上下文：拆分更多的Async方法，组织不同层次代码
        public async Task ResumeOnContextAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));

            // Async 方法在同一个上下文中恢复运行
            //Console.WriteLine("ResumeOnContextAsync" + Thread.CurrentThread.ManagedThreadId);
        }
        public async Task ResumeWithoutContextAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            // Async 方法恢复运行时，会丢弃上下文
            //Console.WriteLine("ResumeWithoutContextAsync" + Thread.CurrentThread.ManagedThreadId);
        }





    }
    #endregion

    #region 3.并行开发基础
    // 3.并行开发基础
    public class ParallelBasic
    {
        // 并行编程:CPU计算密集型 任务片段
        // 异步编程:IO密集型 长时间等待、网页下载
        public void Show()
        {
            ParallelSum(new List<int>() { 1, 3, 2, 4, 4, 3, 2, 1 });
        }

        // 3.1 数据的并行处理
        // 场景：对一批数据中的每个元素进行相同的操作，CPU密集型，耗费时间
        //      反转集合中的每个矩阵，发现无效矩阵，中断并行
        public void InvertMatrices(IEnumerable<Matrix> matrices, CancellationToken token)
        {
            Parallel.ForEach(matrices, new ParallelOptions { CancellationToken = token }, (matrix, state) =>
              {
                  if (matrix.HasInverse)
                  {
                      matrix.Invert();
                  }
                  else
                  {
                      state.Stop();   // 停止并行运行(内部) 另:CancellationToken(外部)
                  }
              });
        }

        // 3.2 并行聚合
        // 场景：在并行操作结束时，需要聚合结果，包括累加和、平均值等
        //      并行累加求和(对每一个输入的数据调用一个操作)
        public int ParallelSum(IEnumerable<int> values)
        {
            object mutex = new object();
            int result = 0;
            Parallel.ForEach(
                source: values,
                localInit: () => 0,
                body: (item, state, localValue) => localValue + item,
                localFinally: localValue =>
                {
                    lock (mutex)
                        result += localValue;
                });
            return result;
            // 并行处理 数据流 另一种方式：PLINQ
            return values.AsParallel().Sum();
        }

        // 3.3 并行调用 Invoke Action
        // 场景：并行调用一批方法，且这些方法相互独立
        public void ProcessActions(CancellationToken token, params Action[] actions)
        {
            Parallel.Invoke(new ParallelOptions { CancellationToken = token }, actions);
        }

        // 3.4 动态并行
        // 场景：并行的任务和数量在运行时才能确定
        //      对树的每个节点进行处理
        // Task类 作为并行任务，使用阻塞成员：Task.Wait、Task.Result、Task.WaitAll、Task.WaitAny
        //        作为异步任务，使用非阻塞成员：Await、Task.WhenAll、Task.WhenAny
        public void ProcessTree(TreeNode node)
        {
            var task = Task.Factory.StartNew(() => Traverse(node),
                            CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            task.Wait();
        }

        private void Traverse(TreeNode node)
        {
            DoExpensiveActionOnNode(node);
            if (node.Nodes.Count > 0)
            {
                var tasks = node.Nodes.OfType<TreeNode>().Select(n =>
                        Task.Factory.StartNew(() => Traverse(n),
                            CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default));
                Task.WaitAll();
            }
        }

        private void DoExpensiveActionOnNode(TreeNode node)
        {
            Console.WriteLine(node.Name + ": " + node.Text);
        }

        // 3.5 并行LINQ
        // 场景：需要对一批数据进行并行处理，生成另外一批数据，或者对数据进行统计
        //      数据流操作：输入 一个 数据队列，输出 一个 数据队列，对序列中的每个元素乘以2
        public IEnumerable<int> MultiplyBy2(IEnumerable<int> values)
        {
            // 输出数据队列 次序 不固定 
            return values.AsParallel().Select(item => item * 2);
            // 数据数据队列 爆出 原有次序
            return values.AsParallel().AsOrdered().Select(item => item * 2);
        }
    }
    #endregion

    #region  4.数据流基础
    // 4.数据流基础
    // TPL DataFlow(TDF) ActionBlock，BufferBlock，TransformBlock，BroadcastBlock
    // https://my.oschina.net/u/4365520/blog/3908742
    public class DataFlowBasic
    {
        public void Show()
        {
            Task task = LinkToDataBlock();
            task.Wait();
        }
        // 4.1 链接 数据流块
        // 场景：创建网格时 链接 数据流块
        public async Task LinkToDataBlock()
        {

            var multiplyBlock = new TransformBlock<int, int>(item => item * 2);
            var subtractBlock = new TransformBlock<int, int>(item => item - 2);

            // 建立链接后，从 multiplyBlock 输出的数据 输入至 subtractBlock
            multiplyBlock.LinkTo(subtractBlock, new DataflowLinkOptions() { PropagateCompletion = true });

            multiplyBlock.Post(3);

            // 第一个数据块的完成情况或出错信息 自动传递给 第二个数据块

            await subtractBlock.Completion; // 这里会卡死
                                            // int result =  subtractBlock.Receive();
        }

        // 4.4 限制流量
        // 场景：分叉的负载均衡
        //      在数据流网格中进行分叉，并且在数据流量能在各分支之间平衡
        public void LimitBoundedCapacity()
        {
            var sourceBlock = new BufferBlock<int>();
            var options = new DataflowBlockOptions { BoundedCapacity = 1 };
            var targetBlockA = new BufferBlock<int>(options);
            var targetBlockB = new BufferBlock<int>(options);
            sourceBlock.LinkTo(targetBlockA);
            sourceBlock.LinkTo(targetBlockB);
        }

        // 4.7 数据流块的并行处理
        // 场景：对数据网格进行并行处理，是数据流块在处理输入的数据时采用并行的方式
        public void ParallelDataflowBlock()
        {
            var multiplyBlock = new TransformBlock<int, int>(
                 item => item * 2,
                 new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded }
                );
            var subtractBlock = new TransformBlock<int, int>(item => item * 2);
            multiplyBlock.LinkTo(subtractBlock);
        }
    }
    #endregion

    // 5.Rx基础




}

using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Concurrency
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ReactiveExtensions.SubEventStream();

            //// ch05
            //{
            //    Ch06();
            //}

            ch12r02A.Show();

            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();
        }

        public static void RxSimples()
        {
            // Rx = Observables + LINQ + Schedulers
            // Reactive Programming 反应式编程，使用 LINQ 风格编写基于观察者模式的异步编程模型
            // Install-Package Rx-main
            // Install-Package Rx-WPF

            var greet = Observable.Return("Hello world!");
            greet.Subscribe(Console.WriteLine);
            // IObservable  greet,
            // IObserve     Console.WriteLine

            Observable.Range(1, 10)
                .Subscribe(x => Console.WriteLine(x));

            var range = Observable.Generate(0,
                x =>
                {
                    return x < 10;
                },
                x =>
                {
                    return x + 1;
                },
                x =>
                {
                    //if (x > 3)
                    //    throw new Exception("more than 3");
                    return x;
                });
            //range.Subscribe(x => Console.WriteLine(x));
            range.Subscribe(x => Console.WriteLine(x.ToString()), e => Console.WriteLine("Error" + e.Message), () => Console.WriteLine("Completed"));


            Observable.Range(1, 10)
                .Subscribe(x => Console.WriteLine(x.ToString()), e => Console.WriteLine("Error" + e.Message), () => Console.WriteLine("Completed"));

            UsingScheduler();

            DifferenceBetweenSubscribeOnAndObserveOn();
        }

        public static void UsingScheduler()
        {
            Console.WriteLine("Starting on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
            var source = Observable.Create<int>(
            o =>
            {
                //  Action<T> onNext, Action<Exception> onError, Action onCompleted
                Console.WriteLine("Invoked on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
                o.OnNext(1); // --> Action<T> onNext
                o.OnNext(2); // --> Action<T> onNext
                o.OnNext(3); // --> Action<T> onNext
                o.OnCompleted(); // --> Action onCompleted
                //o.OnError(new Exception());
                Console.WriteLine("Finished on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
                return Disposable.Empty;
            });
            source
            .SubscribeOn(NewThreadScheduler.Default) // 调度 长时间操作
            //.SubscribeOn(ThreadPoolScheduler.Instance) /// 短时间操作
            .Subscribe(
            o =>
            {
                Console.WriteLine("Received {1} on threadId:{0}", Thread.CurrentThread.ManagedThreadId, o);
            },
            () =>
            {
                Console.WriteLine("OnCompleted on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
            });
            Console.WriteLine("Subscribed on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
        }

        public static void DifferenceBetweenSubscribeOnAndObserveOn()
        {
            Thread.CurrentThread.Name = "Main";

            IScheduler thread1 = new NewThreadScheduler(x => new Thread(x) { Name = "Thread1" });
            IScheduler thread2 = new NewThreadScheduler(x => new Thread(x) { Name = "Thread2" });

            Observable.Create<int>(o =>
            {
                Console.WriteLine("Subscribing on " + Thread.CurrentThread.Name);
                o.OnNext(1);
                return Disposable.Create(() => { });
            })
            .SubscribeOn(thread1)   // 调度 可观察者 异步
            .ObserveOn(thread2)     // 调度 观察者 异步
            .Subscribe(x => Console.WriteLine("Observing '" + x + "' on " + Thread.CurrentThread.Name));
        }

        public static void Ch05()
        {
            {
                // TransformBlock
                var multiplyBlock = new TransformBlock<int, int>(item =>
                {
                    int result = item * 2;
                    Console.WriteLine($"item * 2:{result}");

                    return result;
                });
                var subtractBlock = new TransformBlock<int, int>(item =>
                {
                    int result = item - 2;
                    Console.WriteLine($"item - 2:{item - 2}");

                    return result;
                });


                // After linking, values that exit multiplyBlock will enter subtractBlock.
                //multiplyBlock.LinkTo(subtractBlock);


                var options = new DataflowLinkOptions { PropagateCompletion = true };
                IDisposable link = multiplyBlock.LinkTo(subtractBlock, options);

                multiplyBlock.Post(2);

                // The first block's completion is automatically propagated to the second block.
                multiplyBlock.Complete();


                link.Dispose();


                IPropagatorBlock<int, int> CreateMyCustomBlock()
                {
                    var multiplyBlock2 = new TransformBlock<int, int>(item => item * 2);
                    var addBlock = new TransformBlock<int, int>(item => item + 2);
                    var divideBlock = new TransformBlock<int, int>(item => item / 2);

                    var flowCompletion = new DataflowLinkOptions { PropagateCompletion = true };
                    multiplyBlock2.LinkTo(addBlock, flowCompletion);
                    addBlock.LinkTo(divideBlock, flowCompletion);

                    return DataflowBlock.Encapsulate(multiplyBlock2, divideBlock);
                }
            }

            {
                // BufferBlock
                var sourceBlock = new BufferBlock<int>();
                var options = new DataflowBlockOptions { BoundedCapacity = 1 };
                var targetBlockA = new BufferBlock<int>(options);
                var targetBlockB = new BufferBlock<int>(options);

                sourceBlock.LinkTo(targetBlockA);
                sourceBlock.LinkTo(targetBlockB);

                sourceBlock.Post(2);
            }

        }

        /// <summary>
        /// Cancel
        /// </summary>
        public static async void Ch09()
        {
            {
                CancellationToken cancellationToken = default; // ...
                IObservable<int> observable = Observable.Range(0, 10); // ...
                int lastElement = await observable.TakeLast(1).ToTask(cancellationToken);
            }

            {
                CancellationToken cancellationToken = default; // ...
                IObservable<int> observable = Observable.Range(0, 10); // ...
                IList<int> allElements = await observable.ToList().ToTask(cancellationToken);
            }

            {
                CancellationToken cancellationToken = default; // ...
                var blockOptions = new ExecutionDataflowBlockOptions
                {
                    CancellationToken = cancellationToken
                };
                var multiplyBlock = new TransformBlock<int, int>(item => item * 2,
                    blockOptions);
                var addBlock = new TransformBlock<int, int>(item => item + 2,
                    blockOptions);
                var divideBlock = new TransformBlock<int, int>(item => item / 2,
                    blockOptions);

                var flowCompletion = new DataflowLinkOptions
                {
                    PropagateCompletion = true
                };
                multiplyBlock.LinkTo(addBlock, flowCompletion);
                addBlock.LinkTo(divideBlock, flowCompletion);

                var result = DataflowBlock.Encapsulate(multiplyBlock, divideBlock);
            }

            {
                string url = "";    //...
                HttpClient client = new HttpClient();

                CancellationToken cancellationToken = default; // ...
                using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))  // 取消标记
                {
                    cts.CancelAfter(TimeSpan.FromSeconds(2));
                    CancellationToken combinedToken = cts.Token;    // 组合标记
                    var result = await client.GetAsync(url, combinedToken);
                }

            }

            {
                string hostNameOrAddress = "127.0.0.1";
                CancellationToken cancellationToken = default; // ...

                var ping = new Ping();

                using (CancellationTokenRegistration _ = cancellationToken.Register(() => ping.SendAsyncCancel()))
                {
                    Task<PingReply> task = ping.SendPingAsync(hostNameOrAddress);
                    var result = await task;
                }

            }
        }


        class ch11r05
        {
            public static async void Test()
            {
                ch11r05 c = new ch11r05();

                c.MyEvent -= c.AsyncHandler;
                c.MyEvent += c.AsyncHandler;

                await c.RaiseMyEventAsync();
                Console.WriteLine("done.");
            }

            public class MyEventArgs : EventArgs, IDeferralSource
            {
                private readonly DeferralManager _deferrals = new DeferralManager();

                // ... // Your own constructors and properties.

                public IDisposable GetDeferral()
                {
                    Console.WriteLine(nameof(GetDeferral));
                    return _deferrals.DeferralSource.GetDeferral();
                }

                internal Task WaitForDeferralsAsync()
                {
                    Console.WriteLine(nameof(WaitForDeferralsAsync));
                    return _deferrals.WaitForDeferralsAsync();
                }
            }

            public event EventHandler<MyEventArgs> MyEvent;

            public async Task RaiseMyEventAsync()
            {
                EventHandler<MyEventArgs> handler = MyEvent;

                if (handler == null)
                    return;

                var args = new MyEventArgs();
                handler(this, args);
                await args.WaitForDeferralsAsync();
            }


            public async void AsyncHandler(object sender, MyEventArgs args)
            {
                using (IDisposable deferral = args.GetDeferral())
                {
                    Console.WriteLine("start delay");
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    Console.WriteLine("end delay");
                }
            }
        }

        class ch12r00
        {
            public async Task MyMethodAsync()
            {
                int value = 10;
                await Task.Delay(TimeSpan.FromSeconds(1));
                value = value + 1;
                await Task.Delay(TimeSpan.FromSeconds(1));
                value = value - 1;
                await Task.Delay(TimeSpan.FromSeconds(1));
                Console.WriteLine(value);
            }
        }

        class ch12r00B
        {
            public static async void Show()
            {
                var ch = new ch12r00B();
                int v = await ch.ModifyValueConcurrentlyAsync();
                Console.Write(v);
            }

            class SharedData
            {
                public int Value { get; set; }
            }

            async Task ModifyValueAsync(SharedData data)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                data.Value = data.Value + 1;
            }

            // WARNING: may require synchronization; see discussion below.
            async Task<int> ModifyValueConcurrentlyAsync()
            {
                var data = new SharedData();

                // Start three concurrent modifications.
                Task task1 = ModifyValueAsync(data);
                Task task2 = ModifyValueAsync(data);
                Task task3 = ModifyValueAsync(data);

                await Task.WhenAll(task1, task2, task3);
                return data.Value;
            }
        }

        class ch12r00C
        {
            public static async void Show()
            {
                var ch = new ch12r00C();
                int v = await ch.ModifyValueConcurrentlyAsync();
                Console.Write(v);
            }

            private int value;

            async Task ModifyValueAsync()
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                value = value + 1;
            }

            // WARNING: may require synchronization; see discussion below.
            async Task<int> ModifyValueConcurrentlyAsync()
            {
                // Start three concurrent modifications.
                Task task1 = ModifyValueAsync();
                Task task2 = ModifyValueAsync();
                Task task3 = ModifyValueAsync();

                await Task.WhenAll(task1, task2, task3);

                return value;
            }
        }

        class ch12r00D
        {
            public static async void Show()
            {
                var ch = new ch12r00D();
                int v = await ch.SimpleParallelismAsync();
                Console.Write(v);
            }

            // BAD CODE!!
            async Task<int> SimpleParallelismAsync()
            {
                int value = 0;
                Task task1 = Task.Run(() => { value = value++; });
                Task task2 = Task.Run(() => { value = value++; });
                Task task3 = Task.Run(() => { value = value++; });
                await Task.WhenAll(task1, task2, task3);
                return value;
            }
        }

        class ch12r00F
        {
            public static async void Show()
            {
                var ch = new ch12r00F();
                int v = ch.ParallelSum(new List<int>() { 1, 2, 3 });
                Console.Write(v);
            }

            // BAD CODE!!
            int ParallelSum(IEnumerable<int> values)
            {
                int result = 0;
                Parallel.ForEach(source: values,
                    localInit: () => 0,
                    body: (item, state, localValue) =>
                    {
                        int s = localValue + item;
                        return s;
                    },
                    localFinally: localValue =>
                    {
                        result += localValue;
                    });
                return result;
            }
        }

        // 阻塞锁 Mutex Monitor SpinLock ReaderWriterLockSlim
        class ch12r01
        {
            class MyClass
            {
                // This lock protects the _value field.
                private readonly object _mutex = new object();

                private int _value;

                public void Increment()
                {
                    lock (_mutex)
                    {
                        _value = _value + 1;
                    }
                }
            }
        }

        // 异步锁 SemaphoreSlim
        class ch12r02A
        {
            public static async void Show()
            {
                var ch = new ch12r02A();
                int v = await ch.ModifyValueConcurrentlyAsync();
                Console.Write(v);
            }


            // This lock protects the _value field.
            private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1);

            private int _value;

            public async Task DelayAndIncrementAsync()
            {
                await _mutex.WaitAsync();
                try
                {
                    int oldValue = _value;
                    await Task.Delay(TimeSpan.FromSeconds(oldValue));
                    _value = oldValue + 1;
                }
                finally
                {
                    _mutex.Release();
                }
            }

            async Task<int> ModifyValueConcurrentlyAsync()
            {
                // Start three concurrent modifications.
                Task task1 = DelayAndIncrementAsync();
                Task task2 = DelayAndIncrementAsync();
                Task task3 = DelayAndIncrementAsync();

                await Task.WhenAll(task1, task2, task3);

                return _value;
            }
        }

        // 阻塞信号 ManualResetEventSlim AutoResetEvent CountdownEvent Barrier
        class ch12r03A
        {
            class MyClass
            {
                private readonly ManualResetEventSlim _initialized =
                    new ManualResetEventSlim();

                private int _value;

                public int WaitForInitialization()
                {
                    _initialized.Wait();
                    return _value;
                }

                public void InitializeFromAnotherThread()
                {
                    _value = 13;
                    _initialized.Set();
                }
            }
        }

        // 异步信号 TaskCompletionSource AsyncManualResetEvent
        class ch12r04A
        {
            class MyClass
            {
                private readonly TaskCompletionSource<object> _initialized =
                    new TaskCompletionSource<object>();

                private int _value1;
                private int _value2;

                public async Task<int> WaitForInitializationAsync()
                {
                    await _initialized.Task; // wait for signal
                    return _value1 + _value2;
                }

                public void Initialize()
                {
                    _value1 = 13;
                    _value2 = 17;
                    _initialized.TrySetResult(null); // send signal
                }
            }
        }

        class ch12r04B
        {
            class MyClass
            {
                private readonly AsyncManualResetEvent _connected =
                    new AsyncManualResetEvent();

                public async Task WaitForConnectedAsync()
                {
                    await _connected.WaitAsync(); // wait for signal
                }

                public void ConnectedChanged(bool connected)
                {
                    if (connected)
                        _connected.Set(); // send signal
                    else
                        _connected.Reset();
                }
            }
        }

        // 限流
        class ch12r05
        {
            abstract class Matrix
            {
                public abstract void Rotate(float degrees);
            }

            IPropagatorBlock<int, int> DataflowMultiplyBy2()
            {
                var options = new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 10
                };

                return new TransformBlock<int, int>(data => data * 2, options);
            }

            // Using Parallel LINQ (PLINQ)
            IEnumerable<int> ParallelMultiplyBy2(IEnumerable<int> values)
            {
                return values.AsParallel()
                    .WithDegreeOfParallelism(10)
                    .Select(item => item * 2);
            }

            // Using the Parallel class
            void ParallelRotateMatrices(IEnumerable<Matrix> matrices, float degrees)
            {
                var options = new ParallelOptions
                {
                    MaxDegreeOfParallelism = 10
                };
                Parallel.ForEach(matrices, options, matrix => matrix.Rotate(degrees));
            }


            async Task<string[]> DownloadUrlsAsync(HttpClient client, IEnumerable<string> urls)
            {
                using (var semaphore = new SemaphoreSlim(10))
                {
                    Task<string>[] tasks = urls.Select(async url =>
                    {
                        await semaphore.WaitAsync();
                        try
                        {
                            return await client.GetStringAsync(url);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }).ToArray();
                    return await Task.WhenAll(tasks);
                }
            }
        }

        #region 调度



        #endregion

    }
}

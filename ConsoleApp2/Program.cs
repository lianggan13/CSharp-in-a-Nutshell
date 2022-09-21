using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
 

            //PLINQ 示例：并行拼写检查器
            {
                List<string> words = new List<string>();
                string[] chars = new string[] { "A", "B", "C", "D", "E", "F", "G" };
                Random random = new Random();
                words.AddRange(Enumerable.Range(0, 150000).Select(i => chars[random.Next(chars.Length)]));


                var wordLookup = new HashSet<string>(words, StringComparer.InvariantCultureIgnoreCase);
                string[] wordList = wordLookup.ToArray();

                //string[] wordsToTest = Enumerable.Range(0, 100000)
                //    .Select(i => wordList[random.Next(0, wordList.Length)]).ToArray();
                // 使用【并行化】的方式生成词汇列表  由于PLINQ会在并行线程上执行，因此必须注意保证操作的线程安全性
                var localRandom = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));  // 线程本地存储，为每个线程创建一个Random对象
                string[] wordsToTest = Enumerable.Range(0, 1000000)
                    .Select(i => wordList[localRandom.Value.Next(0, wordList.Length)]).ToArray();

                wordsToTest[1234] = "Error Word1";
                wordsToTest[1235] = "Error Word2";

                var query = wordsToTest.AsParallel()    // 启用查询并行化
                                        .Select((word, index) => new IndexeWord { Word = word, Index = index })
                                        .Where(iWord => !wordLookup.Contains(iWord.Word))
                                        .OrderBy(iWord => iWord.Index);
                foreach (var mistake in query)
                {
                    Console.WriteLine(mistake.Word + " - index = " + mistake.Index);
                }
            }
            // 设置和更改并行级别 和 取消操作
            {
                var msg = "The Quick Brown Fox"
               .AsParallel().WithDegreeOfParallelism(2)
               .Where(c => !char.IsWhiteSpace(c))
               .AsParallel().WithDegreeOfParallelism(3)
               .Select(c => char.ToUpper(c));
                Console.WriteLine(string.Join("", msg));

                //  取消操作
                var cancelSource = new CancellationTokenSource();
                IEnumerable<int> million = Enumerable.Range(3, 1000000);
                var primeNumQuery =
                    from n in million.AsParallel().WithCancellation(cancelSource.Token)
                    where Enumerable.Range(2, (int)Math.Sqrt(n)).All(i => n % i > 0)
                    select n;

                new Thread(() =>
                {
                    Thread.Sleep(100);
                    cancelSource.Cancel();
                }).Start();
                try
                {
                    // start query running;
                    int[] primes = primeNumQuery.ToArray();

                }
                catch (OperationCanceledException opEx)
                {

                    Console.WriteLine(opEx.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            // PLINQ优化{
            {
                // 输出端优化 ForAll
                "abcdef".AsParallel().Select(c => char.ToUpper(c)).ForAll(Console.Write);
                // 输入端优化 【范围】和【块】划分策略

                // 块划分：让各个线程保持同等忙碌的状态，但从共享的输入序列中获取元素需要进行同步，同步会带来竞争和开销
                int[] nums = { 3, 4, 5, 6, 7, 8, 9 };
                var parallelQuery =
                    Partitioner.Create(nums, true).AsParallel()
                    .Select(i => i + 1);
                parallelQuery.ForAll(Console.Write);
                // 范围划分
                // 预先给每一个工作线程分配同等数量的元素，避免输入序列上竞争
                // 相同的数字（序列）操作 --- 范围划分
                ParallelEnumerable.Range(1, 100000000).Sum(i => Math.Sqrt(i));

                // 聚合优化
                string text = "Let's suppose this is a really long string";
                int[] result =
                    text.Aggregate(
                        new int[26], (accumulator, c) =>
                        {
                            int index = char.ToUpper(c) - 'A';
                            if (index >= 0 && index <= 26) accumulator[index]++;
                            return accumulator;
                        });
                int[] resultAsync =
                    text.AsParallel().Aggregate(
                          () => new int[26],

                        (accumulator, c) =>
                        {
                            int index = char.ToUpper(c) - 'A';
                            if (index >= 0 && index <= 26) accumulator[index]++;
                            return accumulator;
                        },

                        (mainAccumulator, localAccumulator)=>
                            mainAccumulator.Zip(localAccumulator,(f1,f2)=>f1+f2).ToArray(),
                        finalResult => finalResult
                    );

                

                int[] resultAsync2 = 
                    text.AsParallel().Aggregate(
                               () => new int[26],
                               (localFreqs, c) =>
                               {
                                   int index = char.ToUpper(c) - 'A';
                                   if (index >= 0 && index <= 26) localFreqs[index]++;
                                   return localFreqs;
                               },
                               (mainFreqs, localFreqs) =>
                                   mainFreqs.Zip(localFreqs, (f1, f2) => f1 + f2).ToArray(),
                               finalResult => finalResult

                      );

            }

            // Parallel 类
            {
                // Parallel.Invoke 并行执行一组委托 params Action[] actions
                Parallel.Invoke(() => Console.WriteLine("par1"),
                    () => Console.WriteLine("par2"));
                // Parallel.For .Foreach 迭代并非顺序执行 完成的顺序是随机的
                Parallel.For(0, 100, i => Console.WriteLine(i));
                Parallel.ForEach("Hello,world", c => Console.WriteLine(c));
                // Parllel 与 PLINQ 对比
                var keyPairs =  new string[6];
                Parallel.For(0, keyPairs.Length, i => keyPairs[i] = "value" + i);
                keyPairs = ParallelEnumerable.Range(0, keyPairs.Length)
                            .Select(i => keyPairs[i] = "value" + i).ToArray();

                // Prallel 版 拼写检查器
                List<string> words = new List<string>();
                string[] chars = new string[] { "A", "B", "C", "D", "E", "F", "G" };
                Random random = new Random();
                words.AddRange(Enumerable.Range(0, 150000).Select(i => chars[random.Next(chars.Length)]));


                var wordLookup = new HashSet<string>(words, StringComparer.InvariantCultureIgnoreCase);
                string[] wordList = wordLookup.ToArray();

                string[] wordsToTest = Enumerable.Range(0, 100000)
                    .Select(i => wordList[random.Next(0, wordList.Length)]).ToArray();

                wordsToTest[1234] = "Error Word1";
                wordsToTest[1235] = "Error Word2";

                var misspellings = new ConcurrentBag<Tuple<int, string>>();// 线程安全的 无序 集合
                Parallel.ForEach(wordsToTest, (word, state, i) =>
                {
                    if (!wordLookup.Contains(word))
                    {
                        misspellings.Add(Tuple.Create((int)i, word));
                    }
                    if (wordsToTest.Length > 1)
                        state.Break();
                });

                // 使用本地值TLocal进行优化，将本地聚合值和主要结果值进行合并
                object locker = new object();
                double total = 0;
                Parallel.For(1, 10000000, // 并行化的效果大部分被一千万个🔒操作和因此带来的阻塞抵消了
                    i => { lock (locker) total += Math.Sqrt(i); });
                Console.WriteLine("total: " + total.ToString());
                total = 0;
                Parallel.For(1, 10000000,
                    () => 0.0,              // 初始化一个本地值
                    (i, state, localTotal) =>
                        localTotal + Math.Sqrt(i),  // 对本地值进行聚合
                    localTotal =>
                        { lock (locker) total += localTotal; }  // 将本地值和最终结果值进行合并
                    );
                Console.WriteLine("total: " + total.ToString());
                // 对比 PLINQ
               double total2 = 0;
                total2 = ParallelEnumerable.Range(1, 10000000)
                    .Sum(i => Math.Sqrt(i));
                Console.WriteLine("total2: " + total2.ToString());

            }

            // 并行任务
            {
                // 父子任务
                Task parent = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("I am parent");
                    Task.Factory.StartNew(() =>
                    {
                        Console.WriteLine("I am detached and free,you can't 管我");
                    });
                    Task.Factory.StartNew(() =>
                    {
                        Console.WriteLine("I am a child");
                    }, TaskCreationOptions.AttachedToParent);
                });
                TaskCreationOptions atp = TaskCreationOptions.AttachedToParent;
                var expParent = Task.Factory.StartNew(() =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        Task.Factory.StartNew(() => { throw new Exception("exp"); }, atp);
                    });
                });

                // 延续任务 异常 子任务
                Task.Factory.StartNew<int>(() => 8)         // 8
                    .ContinueWith(ant => ant.Result * 2)    // 16
                    .ContinueWith(ant => Math.Sqrt(ant.Result))    // 4
                   .ContinueWith(ant => Console.WriteLine("Continue Task Result: " + ant.Result));

                Task task1 = Task.Factory.StartNew(() => throw new Exception("exp") );
                Task error = task1.ContinueWith(ant => Console.WriteLine(ant.Exception),TaskContinuationOptions.OnlyOnFaulted);
                Task ok = task1.ContinueWith(ant => Console.WriteLine("Success!"), TaskContinuationOptions.NotOnFaulted);
                // 忽略异常 Task.Factory.StartNew(() => throw new Exception("exp")).IgnoreExceptions();
                TaskCreationOptions atp2 = TaskCreationOptions.AttachedToParent;
                Task.Factory.StartNew(() =>
                {
                    Task.Factory.StartNew(() => throw new Exception("exp from child1"), atp2);
                    Task.Factory.StartNew(() => throw new Exception("exp from child2"), atp2);
                    Task.Factory.StartNew(() => throw new Exception("exp from child3"), atp2);             
                }).ContinueWith(p=>Console.WriteLine(p.Exception),TaskContinuationOptions.OnlyOnFaulted);

                // N个前导任务 --- 1个后导（延续）任务
                // ContinueWhenAll  WhenAll
                // 1个前导任务 --- N个后导（延续）任务 注：后续任务同时开时执行，不会顺序执行 
                var t = Task.Factory.StartNew(() => Thread.Sleep(1000));
                t.ContinueWith(ant => Console.Write("A"));
                    t.ContinueWith(ant => Console.Write("B"));
                    t.ContinueWith(ant => Console.Write("C"));
                    t.ContinueWith(ant => Console.Write("D"));
                    t.ContinueWith(ant => Console.Write("E"));
                    t.ContinueWith(ant => Console.Write("F"));
                    t.ContinueWith(ant => Console.Write("G"));

                // 任务调度器：与CLR的线程池协同工作的默认调度器，和同步上下文调度器
                var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                Task.Run(() => { Thread.Sleep(100); return "content"; })
                    .ContinueWith(ant => { string uiText = ant.Result; }, uiScheduler);
                // 异步捕获和处理
                var mParent = Task.Factory.StartNew(() =>
                {
                    int[] numbers = { 0 };
                    var childFactory = new TaskFactory(TaskCreationOptions.AttachedToParent,
                                    TaskContinuationOptions.None);
                    childFactory.StartNew(() => 5 / numbers[0]);
                    childFactory.StartNew(() => numbers[1]);
                    childFactory.StartNew(() => throw new Exception("exp"));
                });
                try
                {
                    mParent.Wait();
                }
                catch (AggregateException aex)
                {
                    aex.Flatten().Handle(ex =>
                    {
                       if(ex is DivideByZeroException)
                        {
                            Console.WriteLine(ex.Message);
                            return true;
                        }
                        if (ex is IndexOutOfRangeException)
                        {
                            Console.WriteLine("Index out of range");
                            return true;
                        }
                        return false;
                    });
                }
                    

            } 
            Console.ReadKey();
        }
    }

    internal class IndexeWord
    {
        public string Word { get; set; }
        public int Index { get; set; }
    }
}

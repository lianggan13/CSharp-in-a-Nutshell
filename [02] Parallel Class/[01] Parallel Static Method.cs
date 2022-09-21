using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _02__Parallel_Class
{
  public  class _01__Parallel_Static_Method
    {
        public static void Show()
        {
            // Parallel.Invoke .For .Foreach 阻塞线程知道所有工作完成为止
            {
                // Parallel.Invoke 并行执行一组委托 params Action[] actions
                {
                    Parallel.Invoke(
                       () => new WebClient().DownloadFile("http://www.linqpad.net", "lp.html"),
                       () => new WebClient().DownloadFile("http://www.jaoo.dk", "jaoo.html"));
                    Parallel.Invoke(() => Console.WriteLine("par1"),
                        () => Console.WriteLine("par2"));
                }
                // Parallel.For 迭代并非顺序执行 完成的顺序是随机的
                {
                    Parallel.For(0, 100, i => Console.WriteLine(i));
                    Parallel.ForEach("Hello,world", c => Console.WriteLine(c));
                    var keyPairs = new string[6];
                    Parallel.For(0, keyPairs.Length, i => keyPairs[i] = "value" + i);
                    keyPairs = ParallelEnumerable.Range(0, keyPairs.Length)
                                .Select(i => keyPairs[i] = "value" + i).ToArray();
                }
                // Parallel.Foreach 迭代并非顺序执行 完成的顺序是随机的
                {
                    Parallel.ForEach("Hello, world", (c, state, i) =>
                    {
                        Console.WriteLine(c.ToString() + i);
                    });

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

                    var misspellings = new ConcurrentBag<Tuple<int, string>>(); // 线程安全的 无序 集合
                    Parallel.ForEach(wordsToTest, (word, state, i) =>           // i 输入序列集合 索引 
                    {
                        if (!wordLookup.Contains(word))
                        {
                            misspellings.Add(Tuple.Create((int)i, word));       // 并行任务 向线程安全集合中 添加数据
                        }
                        if (wordsToTest.Length > 1)
                            state.Break();
                    });

                    // Break
                    Parallel.ForEach("Hello, world", (c, loopState) =>
                    {
                        if (c == ',')
                            loopState.Break();
                        else
                            Console.Write(c);
                    });

                    // Stop
                    Parallel.ForEach("Hello, world", (c, loopState) =>
                    {
                        if (c == ',')
                            loopState.Stop();
                        else
                            Console.Write(c);
                    });

                }


            }

            // 结果整理优化
            {
                // 并行化的效果被 10000000个 锁操作和因此带来的阻塞抵消
                object locker = new object();
                double total = 0;
                Parallel.For(0,10000000,i=> { lock (locker) total += Math.Sqrt(i); });

                // 并行化中每个工作线程拥有属于本地值，并将本地聚合值与总结果值合并
                double grandTotal = 0;
                Parallel.For(0, 10000000,
                    () => 0.0,
                    (i, state, localTotal) =>
                        localTotal += Math.Sqrt(i),
                    localTotal => { lock (locker) grandTotal += localTotal; }
                    );

                // PINQ 不需要 手动整理结果
                ParallelEnumerable.Range(0, 10000000).Sum(i => Math.Sqrt(i));  
            }
        }
    }
}

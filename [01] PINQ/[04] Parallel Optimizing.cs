using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01__PINQ
{
    /// <summary>
    /// 并行优化
    /// </summary>
    public class _04__Parallel_Optimizing
    {
        public static void Show()
        {
            // OutputSide Optimization 输出端优化 
            {
                // ForAll 不考虑输出元素顺序，并在每个输出元素上执行委托
                "abcdef".AsParallel().Select(c => char.ToUpper(c)).ForAll(Console.Write);
            }
            // InputSide Optimization 输入端优化 
            // 输入端三种划分策略： 块划分、范围划分、散列划分
            {
                // 块划分：输入序列五索引无序集合
                // 让各个线程保持同等忙碌的状态，但从共享的输入序列中获取元素需要进行同步，同步会带来竞争和开销
                {
                    int[] numbers = { 3, 4, 5, 6, 7, 8, 9 };

                    var parallelQuery =
                        Partitioner.Create(numbers, true).AsParallel()
                        .Where(n => n % 2 == 0);
                    parallelQuery.Dump();
                }
                // 范围划分：输入序列有索引，输入序列长，元素执行时间大致相等,如IList<T>
                // 预先给每一个工作线程分配同等数量的元素，避免输入序列上竞争
                {
                    ParallelEnumerable.Range(1, 100000000).Sum(i => Math.Sqrt(i));
                }

            }
            // Aggregations Optimization 聚合优化
            // 为输入序列生成多个种子，从而可以从多个分块序列中聚合
            {
                // 简单使用 Sum求和
                {
                    int[] numbers = { 3, 4, 5, 6, 7, 8, 9 };
                    int sum = numbers.Aggregate(0, (total, n) => total + n);
                }
                // 种子工厂
                {
                    {
                        int[] numbers = { 3, 4, 5, 6, 7, 8, 9 };
                        numbers.AsParallel().Aggregate(
                            () => 0,        // SeedFactory 返回一个新的本地累加器 localTotal = 0
                            (localTotal, n) => localTotal + n,  // UpdateAccumulatorFunc 将聚合到本地累加器
                            (mainTotal, localTotal) => mainTotal + localTotal,  // CombineAccumulatorFunc 将本地累加器与主累加器结合
                            finalResult => finalResult);    // ResultSelector 在最终的结果上应用任意的转换
                    }
                    // 统计字母出现频率
                    {
                        // Foreach 版本
                        string text = "Let’s suppose this is a really long string";
                        var letterFrequencies = new int[26];
                        foreach (char c in text)
                        {
                            int index = char.ToUpper(c) - 'A';
                            if (index >= 0 && index <= 26) letterFrequencies[index]++;
                        };
                        letterFrequencies.Dump();

                        // Aggregate 顺序版本
                        int[] result = text.Aggregate(
                                new int[26],
                                (letterFreqs, c) =>
                                {
                                    int index = char.ToUpper(c) - 'A';
                                    if (index >= 0 && index <= 26) letterFreqs[index]++;
                                    return letterFreqs;
                                }
                            );
                        result.Dump();

                        // Aggregate 并行版本
                        int[] resultAsync = text.AsParallel().
                            Aggregate(
                            () => new int[26],
                            (localFreqs, c) =>
                            {
                                int index = char.ToUpper(c) - 'A';
                                if (index >= 0 && index <= 26) localFreqs[index]++;
                                return localFreqs;
                            },
                            (mainFreqs, localFreqs) => mainFreqs.Zip(localFreqs, (f1, f2) => f1 + f2).ToArray(),
                            finalResult => finalResult
                            );



                    }


                }


            }
        }
    }
}

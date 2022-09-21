using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyAsyncThread
{
    public class ParallelClass
    {
        public ParallelClass()


        {
            Console.WriteLine($"****************btnTask_Click Start {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
            {
                //用Task完成这个  你们会吗  不在意那个主线程参与计算的事儿
                Parallel.Invoke(() => CommoncClass.Coding("爱书客", "Client")
                    , () => CommoncClass.Coding("风动寂野", "Portal")
                    , () => CommoncClass.Coding("笑看风云", "Service"));
            }
            {
                Parallel.For(0, 5, i => CommoncClass.Coding("爱书客", "Client" + i));
            }
            {
                Parallel.ForEach(new string[] { "0", "1", "2", "3", "4" }, i => CommoncClass.Coding("爱书客", "Client" + i));
            }
            {
                //parallelOptions 可以控制并发数量
                ParallelOptions parallelOptions = new ParallelOptions();
                parallelOptions.MaxDegreeOfParallelism = 3;
                Parallel.For(0, 10, parallelOptions, i => CommoncClass.Coding("爱书客", "Client" + i));
            }
            {
                //parallelOptions 可以控制并发数量
                Task.Run(() =>
                {
                    ParallelOptions parallelOptions = new ParallelOptions();
                    parallelOptions.MaxDegreeOfParallelism = 3;
                    Parallel.For(0, 10, parallelOptions, i => CommoncClass.Coding("爱书客", "Client" + i));
                });
            }
            {
                //Break  Stop  都不推荐用
                ParallelOptions parallelOptions = new ParallelOptions();
                parallelOptions.MaxDegreeOfParallelism = 3;
                Parallel.For(0, 40, parallelOptions, (i, state) =>
                {
                    if (i == 2)
                    {
                        Console.WriteLine($"线程Break，当前任务结束 i={i}  {Thread.CurrentThread.ManagedThreadId.ToString("00")}");
                        state.Break();//结束Parallel当次操作  等于continue 
                            return;//必须带上
                        }
                    if (i == 20)
                    {
                        Console.WriteLine($"线程Stop，Parallel结束 i={i} {Thread.CurrentThread.ManagedThreadId.ToString("00")}");
                        state.Stop();//结束Parallel全部操作   等于break
                        return;//必须带上
                    }
                    CommoncClass.Coding("爱书客", "Client" + i);
                });
                //Break 实际上结束了当前这个线程；如果是主线程，等于Parallel都结束了
                //多线程的终止本身就不靠谱
            }
            Console.WriteLine($"****************btnTask_Click End   {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
        }

    }
}

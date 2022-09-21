using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyAsyncThread
{
    public class TaskClass
    {
       public TaskClass()
        {

            {
                Console.WriteLine($"****************btnTask_Click Start {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");

                {
                    Task.Delay(1000);//延迟  不会卡
                    Thread.Sleep(1000);//等待   卡

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    Thread.Sleep(2000);
                    stopwatch.Stop();
                    Console.WriteLine(stopwatch.ElapsedMilliseconds);
                }
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    Task.Delay(2000).ContinueWith(t =>
                    {
                        stopwatch.Stop();
                        Console.WriteLine(stopwatch.ElapsedMilliseconds);
                    });
                }
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    Task.Run(() =>
                    {
                        Thread.Sleep(2000);
                        stopwatch.Stop();
                        Console.WriteLine(stopwatch.ElapsedMilliseconds);
                    });
                }


                {
                    ThreadPool.SetMaxThreads(8, 8);//SetMaxThreads去控制最大的线程并发数量  
                    //这种方法不好 ThreadPool是全局的
                    for (int i = 0; i < 100; i++)
                    {
                        Task.Run(() =>
                        {
                            Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString("00"));
                            Thread.Sleep(2000);
                        });
                    }
                }
                {
                    List<int> list = new List<int>();
                    for (int i = 0; i < 10000; i++)
                    {
                        list.Add(i);
                    }
                    //完成10000个任务  但是只要11个线程  
                    Action<int> action = i =>
                    {
                        Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString("00"));
                        Thread.Sleep(new Random(i).Next(100, 300));
                    };
                    List<Task> taskList = new List<Task>();
                    foreach (var i in list)
                    {
                        int k = i;
                        taskList.Add(Task.Run(() => action.Invoke(k)));
                        while (taskList.Count > 10) // 循环: 当任务📕 小于或等于 10 时，才退出 （保证启动11个任务 并发 控制并发数）
                        {
                            Task.WaitAny(taskList.ToArray());
                            taskList = taskList.Where(t => t.Status != TaskStatus.RanToCompletion).ToList();
                        }
                    }
                    Task.WhenAll(taskList.ToArray());
                }

                {
                    // 识别是哪个任务完成
                    Task task = new Task(t => this.Coding("爱书客", "Client"), "爱书客");
                    Console.WriteLine(task.AsyncState);               
                }


                {
                    TaskFactory ts = new TaskFactory();
                    List<Task> taskList = new List<Task>();
                    taskList.Add(ts.StartNew(o => this.Coding("爱书客", "Client"), "爱书客"));
                    taskList.Add(ts.StartNew(o => this.Coding("风动寂野", "Portal"), "风动寂野"));
                    taskList.Add(ts.StartNew(o => this.Coding("笑看风云", "Service"), "笑看风云"));
                    ts.ContinueWhenAny(taskList.ToArray(), t =>
                    {
                        Console.WriteLine(t.AsyncState);
                        Console.WriteLine($"部署环境，联调测试。。。【{Thread.CurrentThread.ManagedThreadId.ToString("00")}】");
                    });
                    ts.ContinueWhenAll(taskList.ToArray(), tList =>
                    {
                        Console.WriteLine(tList[0].AsyncState);
                        Console.WriteLine($"部署环境，联调测试。。。【{Thread.CurrentThread.ManagedThreadId.ToString("00")}】");
                    });
                }
                Task.Run(() => this.DoSomethingLong("btnTask_Click1"));
                Task.Run(() => this.DoSomethingLong("btnTask_Click2"));
                TaskFactory taskFactory = Task.Factory;//4.0
                taskFactory.StartNew(() => this.DoSomethingLong("btnTask_Click3"));
                new Task(() => this.DoSomethingLong("btnTask_Click4")).Start();

                {
                    Task.Run(() => this.Coding("爱书客", "Client")).ContinueWith(t => { });
                }


                {
                    ////什么时候用多线程？ 任务能并发运行；提升速度；优化体验
                    List<Task> taskList = new List<Task>();
                    Console.WriteLine($"项目经理启动一个项目。。【{Thread.CurrentThread.ManagedThreadId.ToString("00")}】");
                    Console.WriteLine($"前置的准备工作。。。【{Thread.CurrentThread.ManagedThreadId.ToString("00")}】");
                    Console.WriteLine($"开始编程。。。【{Thread.CurrentThread.ManagedThreadId.ToString("00")}】");
                    taskList.Add(Task.Run(() => this.Coding("爱书客", "Client")));//task--hash--
                    taskList.Add(Task.Run(() => this.Coding("风动寂野", "Portal")));
                    taskList.Add(Task.Run(() => this.Coding("笑看风云", "Service")));
                    //做个子类 子类里面包含了一个属性  绿叶种子写个例子
                    taskList.Add(Task.Run(() => this.Coding("Jack", "Jump")));
                    taskList.Add(Task.Run(() => this.Coding("胡萝卜", "Monitor")));


                     taskFactory = new TaskFactory();
                    taskFactory.ContinueWhenAll(taskList.ToArray(), tList =>
                    {
                        Console.WriteLine($"部署环境，联调测试。。。【{Thread.CurrentThread.ManagedThreadId.ToString("00")}】");
                    });

                    taskFactory.ContinueWhenAny(taskList.ToArray(), t =>
                    {
                        Console.WriteLine($"部署环境，联调测试。。。【{Thread.CurrentThread.ManagedThreadId.ToString("00")}】");
                    });

                    taskList.Add(Task.WhenAny(taskList.ToArray()).ContinueWith(t =>
                    {
                        Console.WriteLine(taskList.ToArray().FirstOrDefault(s => s.Status == TaskStatus.RanToCompletion));
                        Console.WriteLine($"得意的笑。。。【{Thread.CurrentThread.ManagedThreadId.ToString("00")}】");
                    }));

                    taskList.Add(Task.WhenAll(taskList.ToArray()).ContinueWith(t =>
                    {
                        Console.WriteLine($"部署环境，联调测试。。。【{Thread.CurrentThread.ManagedThreadId.ToString("00")}】");
                    }));

                    Task.Run(
                        () =>
                        {
                            Task.WaitAny(taskList.ToArray());//会阻塞当前线程，等着某个任务完成后，才进入下一行  卡界面
                                                             //Task.WaitAny(taskList.ToArray(), 1000);
                            Console.WriteLine($"完成里程碑 【{Thread.CurrentThread.ManagedThreadId.ToString("00")}】");

                            //多线程加快速度，但是全部任务完成后，才能执行的操作
                            Task.WaitAll(taskList.ToArray());//会阻塞当前线程，等着全部任务完成后，才进入下一行  卡界面
                            Console.WriteLine($"告诉甲方验收，上线使用【{Thread.CurrentThread.ManagedThreadId.ToString("00")}】");
                        });

                    //一个业务查询操作有多个数据源  首页--多线程并发--拿到全部数据后才能返回  WaitAll
                    //一个商品搜素操作有多个数据源，商品搜索--多个数据源--多线程并发--只需要一个结果即可--WaitAny
                    ////阻塞：需要完成后再继续
                    Task.WaitAny(taskList.ToArray());//会阻塞当前线程，等着某个任务完成后，才进入下一行  卡界面
                    //Task.WaitAny(taskList.ToArray(), 1000);
                    Console.WriteLine($"完成里程碑 【{Thread.CurrentThread.ManagedThreadId.ToString("00")}】");

                    //多线程加快速度，但是全部任务完成后，才能执行的操作
                    Task.WaitAll(taskList.ToArray());//会阻塞当前线程，等着全部任务完成后，才进入下一行  卡界面

                    //Task.WaitAll(taskList.ToArray(), 1000);//限时等待
                    //Console.WriteLine("等待1s之后，执行的动作");
                    Console.WriteLine($"告诉甲方验收，上线使用【{Thread.CurrentThread.ManagedThreadId.ToString("00")}】");
                    Console.WriteLine($"****************btnTask_Click End   {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
                }
            }
           
        }

        /// <summary>
        /// 编码做项目
        /// </summary>
        /// <param name="name"></param>
        /// <param name="project"></param>
        private void Coding(string name, string project)
        {
            Console.WriteLine($"****************Coding {name} Start {project} {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
            long lResult = 0;
            for (int i = 0; i < 1000000000; i++)
            {
                lResult += i;
            }
            //Thread.Sleep(2000);

            Console.WriteLine($"****************Coding {name}   End {project} {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {lResult}***************");
        }

        /// <summary>
        /// 一个比较耗时耗资源的私有方法
        /// </summary>
        /// <param name="name"></param>
        private void DoSomethingLong(string name)
        {
            Console.WriteLine($"****************DoSomethingLong {name} Start {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
            long lResult = 0;
            for (int i = 0; i < 1000000000; i++)
            {
                lResult += i;
            }
            //Thread.Sleep(2000);

            Console.WriteLine($"****************DoSomethingLong {name}   End {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {lResult}***************");
        }
    }
}
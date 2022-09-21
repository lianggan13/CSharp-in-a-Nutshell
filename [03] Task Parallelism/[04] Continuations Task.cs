using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace _03__Task_Parallelism
{
    /// <summary>
    ///  前导任务 延续任务
    /// </summary>
    public class _04__Continuations_Task
    {
        public static void Show()
        {

            // 前导任务 task1  延续任务 task2
            {
                Task task1 = Task.Factory.StartNew(() => Console.Write("antecedant.."));
                Task task2 = task1.ContinueWith(ant => Console.Write("..continuation"));
            }

            // Task<TResult>
            {
                Task.Factory.StartNew<int>(() => 8)
                    .ContinueWith(ant => ant.Result * 2)
                    .ContinueWith(ant => Math.Sqrt(ant.Result))
                    .ContinueWith(ant => Console.WriteLine(ant.Result));   // 4
            }

            // Exception
            {
                Task task1 = Task.Factory.StartNew(() => { throw new Exception("Exception from task1"); });

                // 1.调用延续任务的的 Wait 方法，捕获或传播前导任务中的 异常，否则异常成为未观测异常
                Task task2 = task1.ContinueWith(ant => Console.WriteLine(ant.Exception));
                task2.Wait();   // throws an AggregateException

                // 2.根据前导任务的执行结果指定不同的延续任务 TaskContinuationOptions
                Task error = task1.ContinueWith(ant => Console.WriteLine(ant.Exception), TaskContinuationOptions.OnlyOnFaulted);
                Task ok = task1.ContinueWith(ant => Console.WriteLine("Success"), TaskContinuationOptions.NotOnFaulted);

                // 忽略异常
                Task.Factory.StartNew(() => { throw new Exception("Exception from Task Factory"); }).ContinueWith(t => t.Exception, TaskContinuationOptions.OnlyOnFaulted);

                // 条件执行
                Task t1 = Task.Factory.StartNew(() => Console.WriteLine("nothing awry here"));

                Task fault = t1.ContinueWith(ant => Console.WriteLine("fault"),
                                              TaskContinuationOptions.OnlyOnFaulted);

                Task t3 = fault.ContinueWith(ant => Console.WriteLine("t3"));       // This executes

                Task t4 = fault.ContinueWith(ant => Console.WriteLine("t4"),
                                              TaskContinuationOptions.NotOnCanceled);	// Does not execute

            }


            // Child Task
            {
                // 在父任务中 一次性捕获 所有异常
                TaskCreationOptions atp = TaskCreationOptions.AttachedToParent;
                Task.Factory.StartNew(() =>
                {
                    Task.Factory.StartNew(() => { throw new Exception("Exception from Task Factory#1"); }, atp);
                    Task.Factory.StartNew(() => { throw new Exception("Exception from Task Factory#1"); }, atp);
                    Task.Factory.StartNew(() => { throw new Exception("Exception from Task Factory#1"); }, atp);
                })
                .ContinueWith(p => Console.WriteLine(p.Exception),
                                    TaskContinuationOptions.OnlyOnFaulted)
                .Wait(); // throws AggregateException containing three NullReferenceExceptions
            }

            // Task 组合
            {
                // 具有多个前导任务的延续任务
                // task1 and task2 would call complex functions in real life:
                Task<int> task1 = Task.Factory.StartNew(() => 123);
                Task<int> task2 = Task.Factory.StartNew(() => 456);

                Task<int> task3 = Task<int>.Factory.ContinueWhenAll(
                    new[] { task1, task2 }, tasks => tasks.Sum(t => t.Result));

                Console.WriteLine(task3.Result);           // 579

                // 单一前导任务上的多个延续任务
                var task4 = Task.Factory.StartNew(() => Thread.Sleep(1000));

                var c1 = task4.ContinueWith(ant => Console.Write("X"));
                var c2 = task4.ContinueWith(ant => Console.Write("Y"));

                Task.WaitAll(c1, c2);
            }

            // TaskScheduler 任务调度器
            {
                Thread NetServer = new Thread(() => { new MyWindow().ShowDialog(); });
                NetServer.SetApartmentState(ApartmentState.STA);
                NetServer.IsBackground = true;
                NetServer.Start();
            }

        }

        public partial class MyWindow : Window
        {
            Label lblResult = new Label();
            TaskScheduler _uiScheduler;   // Declare this as a field so we can use
                                          // it throughout our class.
            public MyWindow()
            {
                InitializeComponent();
            }

            protected override void OnActivated(EventArgs e)
            {
                // Get the UI scheduler for the thread that created the form:
                _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

                Task.Factory.StartNew<string>(SomeComplexWebService)
                    .ContinueWith(ant => lblResult.Content = ant.Result, _uiScheduler);
            }

            void InitializeComponent()
            {
                lblResult.FontSize = 20;
                Content = lblResult;
            }

            string SomeComplexWebService() { Thread.Sleep(1000); return "Foo"; }
        }
    }
}

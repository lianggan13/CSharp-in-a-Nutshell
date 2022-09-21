using System;

namespace _02__Tasks
{
    class Program
    {
        static void Main(string[] args)
        {
            //_01__Wait.Show();
            // _02TaskException.Show();
            //_03__Continuations.Show();
            //_04TaskCompletionSource.Show();
            //{
            //    var _04 = new _04TaskCompletionSource();
            //    Func<int> func = () => { Thread.Sleep(3000); return 42; };
            //    Task<int> task = _04.CreateTaskCompletionSourceTask<int>(func);  // 利用 CreateTaskCompletionSourceTask 创建任务
            //    Console.WriteLine(task.Result);     // 42 会阻塞
            //    Console.WriteLine("Main Thread Point #2");

            //}
            _05_TaskCancel.Show();

            Console.WriteLine("Main Thread End.");
            Console.ReadLine();
        }
    }
}

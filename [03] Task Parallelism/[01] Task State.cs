using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _03__Task_Parallelism
{
  public  class _01__Task_State
    {
        public static void Show()
        {
            // public Task StartNew(Action<object> action, object state)
            var task2 = Task.Factory.StartNew(state=>Greet("Hello."), "Task2 State");
            Console.WriteLine(task2.AsyncState);   // Greeting
        }
        static void Greet(string message) { Console.WriteLine(message); }   // Hello

       
    }



}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _03__Task_Parallelism
{
  public  class _02__Child_Task
    {
        public static void Show()
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
           
            var expParent = Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    Task.Factory.StartNew(() => { throw new Exception("exp"); }, TaskCreationOptions.AttachedToParent);
                });
            });

        }
    }
}

using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _03__Task_Parallelism
{
    public  class _05__Custom_TaskFactory
    {
        public static void Show()
        {
            var factory = new TaskFactory(
    TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent,
    TaskContinuationOptions.None);

            Task task1 = factory.StartNew(() => "foo".Dump());
            Task task2 = factory.StartNew(() => "far".Dump());
        }
    }
}

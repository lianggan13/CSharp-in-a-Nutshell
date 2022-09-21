using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    /// <summary>
    /// 进度报告器
    /// </summary>
    public class ProgressReportor
    {
        public async void Show()
        {
            var progress = new Progress<int>(i => Console.WriteLine(i + "%"));
            await Foo(progress);
        }
        public Task Foo(IProgress<int> onProgressPercentChanged)
        {
            return Task.Run(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    if (i % 10 == 0)
                        onProgressPercentChanged.Report(i / 10);
                }
            });
        }
    }
}

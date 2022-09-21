using System;
using System.Threading.Tasks;

namespace _05__Asynchronous_Patters
{
    /// <summary>
    /// 进度报告
    /// </summary>
    public class _02__Progress_Reporting
    {
        public async static void Show()
        {
            // Progress reporting with a delegate
            {
                Action<int> reporter = i => Console.WriteLine(i + " %");
                await new MyTask().Foo(reporter);
            }
            // Progress reporting with a IProgress
            {
                var repporter = new Progress<int>(i => Console.WriteLine(i + " %")
                );
                repporter.ProgressChanged += Repporter_ProgressChanged;
                await new MyTask().Foo(repporter);
            }
        }

        private static void Repporter_ProgressChanged(object sender, int e)
        {

        }

        class MyTask
        {
            public Task Foo(Action<int> onProgressPercentChanged)
            {
                return Task.Run(() =>
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        if (i % 10 == 0) onProgressPercentChanged(i / 10);
                    }
                });
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
}

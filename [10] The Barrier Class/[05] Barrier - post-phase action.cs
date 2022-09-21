using System;
using System.Threading;
using System.Threading.Tasks;

namespace _10__The_Barrier_Class
{
    public class _05__Barrier___post_phase_action
    {
        public static void Show()
        {
            Run();
        }

        public static void Run()
        {
            // 假定有一个4人参加的友谊赛，4人开始跑，有的人跑的快，有的人跑的慢，
            // 但是都会在SignalAndWait处停下来，等4个人都到达SignalAndWait处后，又都开始继续往下执行了
            Barrier barrier = new Barrier(4, it =>
            {
                Console.WriteLine("再次集结，友谊万岁，再次开跑");
            });

            string[] names = { "张三", "李四", "王五", "赵六" };
            Random random = new Random();
            foreach (string name in names)
            {
                Task.Run(() =>
                {
                    Console.WriteLine($"{name}开始跑");
                    int t = random.Next(1, 10);
                    Thread.Sleep(t * 1000);
                    Console.WriteLine($"{name}用时{t}秒，跑到友谊集结点");
                    barrier.SignalAndWait();
                    Console.WriteLine($"友谊万岁，{name}重新开始跑");
                });
            }
        }
    }
}

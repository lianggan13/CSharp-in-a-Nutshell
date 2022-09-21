using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyAsyncThread
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Application.EnableVisualStyles();
            // Application.SetCompatibleTextRenderingDefault(false);
            //  Application.Run(new frmThreads());

            var ss = new string[] { "0", "1", "2", "3", "4" };

            StringBuilder sb = new StringBuilder();

            Parallel.ForEach(ss, i =>
            {
                Console.WriteLine(i);
                sb.Append(i);
                Thread.Sleep((int.Parse(i) * 1000));
                //CommoncClass.Coding("爱书客", "Client" + i);
            });

            if (sb.Length > 0)
                Console.WriteLine(sb.ToString());

            Console.ReadKey();
        }
    }
}

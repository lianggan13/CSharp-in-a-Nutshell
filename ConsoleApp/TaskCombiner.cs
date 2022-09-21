using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
 public   class TaskCombiner
    {
         public async void Show()
        {
            //await CheckAllTaskExpcetion();

            string[] urils = new string[] { "www.baidu.com", "www.goole.com" };
            Console.WriteLine("GetTotalSizeAsync: "+ await GetTotalSizeAsync(urils));
            Console.WriteLine("GetTotalSizeAsyncWithLambda: "+await GetTotalSizeAsyncWithLambda(urils));
        }

        public async Task CheckAllTaskExpcetion()
        {
            Task task1 = Task.Run(() => { throw null; });
            Task task2 = Task.Run(() => { throw null; });
            Task task3 = Task.Run(() => { throw null; });
            Task all =  Task.WhenAll(task1, task2, task3);
            try
            {
                await all;
            }
            catch
            {
                Console.WriteLine(all.Exception.InnerExceptions.Count);

            }
        }
        public async Task<int> GetTotalSizeAsync(string[] urils)
        {
            IEnumerable<Task<byte[]>> downloadTasks = urils.Select(
                uri => DownLoadDataAsync(uri));
            byte[][] contents = await Task.WhenAll(downloadTasks);
            Thread.Sleep(3000);
            return contents.Sum(c => c.Length);  // 在任务完成之后，处理字节数组
        }

        public async Task<int> GetTotalSizeAsyncWithLambda(string[] urils)
        {
            IEnumerable<Task<int>> downloadTasks = urils.Select(async uri =>
                (await DownLoadDataAsync(uri)).Length);
            int[] contentLengths = await Task.WhenAll(downloadTasks);
            return contentLengths.Sum();
        }


        private async Task<byte[]> DownLoadDataAsync(string uri)
        {
            return await Task.Run(() =>
            {
                return DownLoadData(uri);
            });
        }
        private byte[] DownLoadData(string uri)
        {
            List<byte> bytes = new List<byte>();
            foreach (char c in uri.ToArray())
            {            
                bytes.Add(Convert.ToByte(c));
            }
            return bytes.ToArray();
        }
    }
}

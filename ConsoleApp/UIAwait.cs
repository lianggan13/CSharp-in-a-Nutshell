using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class UIAwait
    {
        public async void Show()
        {
            // Go();
            
            //DownLoad();

            //Console.WriteLine(await GetWebPageAsync("www.baidu.com"));
            //var awaiter =  GetWebPageAsync("www.baidu.com").GetAwaiter();
            //if (awaiter.IsCompleted)
            //    Console.WriteLine(awaiter.GetResult());
            //else
            //    awaiter.OnCompleted(() => Console.WriteLine(awaiter.GetResult()));

            string[] urils = new string[] { "www.baidu.com", "www.goole.com" , "www.baidu.com" ,
                "www.baidu.com1" ,"www.baidu.com2", "www.goole.com3" , "www.baidu.com","www.goole.com"};
            foreach (string uri in urils)
            {
                //Console.WriteLine(await GetWebPageAsync(uri));
                Console.WriteLine( await  GetWebPageAsyncWithNoAwait(uri));
            }
            Console.WriteLine("弹入，异步方法执行完成  【执行点】回到主UI线程...");
        }

        public async void Go()
        {
            bool button = false;
            for (int i = 1; i < 5; i++)
            {
                // await 让执行点返回给调用者，这是在调用者线程上同步执行的
               // string text = await GetPrimesCountAsync(i * 100000, 100000) +
               //     " primes between " + (i * 100000) + " and " + ((i + 1) * 100000);
               // 调用异步方法但不等待 可以让异步方法 和 后续代码 并行执行 
                string text =   GetPrimesCountAsync(i * 100000, 100000) +
                    " primes between " + (i * 100000) + " and " + ((i + 1) * 100000);
                Console.WriteLine(text);
               
            }

            button = true;
            Console.WriteLine("【异步方法】执行完成");
        }

        public async void DownLoad()
        {
            bool button = false;
            string[] urils = new string[] { "www.baidu.com", "www.goole.com", "www.baidu.com", "www.goole.com"
            , "www.baidu.com", "www.goole.com", "www.baidu.com", "www.goole.com"};
            int totalLength = 0;
            try
            {
                foreach (string url in urils)
                {
                    byte[] data = await DownLoadDataAsync(url);
                    totalLength += data.Length;
                    Console.WriteLine(totalLength);
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                button = true;
                Console.WriteLine("【异步方法】执行完成");
            }


        }


        public  Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run( () =>
            {
                return ParallelEnumerable.Range(start, count).Count(n =>
                     Enumerable.Range(2, (int)(Math.Sqrt(n) - 1)).All(i => n % i > 0)
                  );
            });
        }
        private async Task<byte[]> DownLoadDataAsync(string uri)
        {
            return await Task.Run(() =>
            {
                List<byte> bytes = new List<byte>();
                foreach (char c in uri.ToArray())
                {

                    bytes.Add(Convert.ToByte(c));
                }
                return bytes.ToArray();
            });
        }

        private static Dictionary<string, string> m_cache = new Dictionary<string, string>();
        private async Task<string> GetWebPageAsync(string uri)
        {
            string html;
            if (m_cache.TryGetValue(uri, out html)) return html;
            return
                m_cache[uri] =
                  await  DownloadStringTaskAsync(uri);
        }
        private static Dictionary<string, Task<string>> m_cacheTask = new Dictionary<string, Task<string>>();

        /// <summary>
        /// 没有等待的异步调用，直接缓存 Task<string> 对象
        /// 防止相同的URL重复调用 DownloadStringTaskAsync
        /// 降低GC负载
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private Task<string> GetWebPageAsyncWithNoAwait(string uri)
        {
           lock (m_cacheTask)   // 即使不在线程上下文，仍具有安全性，并没有在下载网页中添加锁，而是仅仅在检查缓存、开始一个新任务，并更新任务缓存的一小段时间内添加了锁
            {
                Task<string> downloadTask;
                if (m_cacheTask.TryGetValue(uri, out downloadTask)) return downloadTask;
                return
                    m_cacheTask[uri] =
                        DownloadStringTaskAsync(uri);
            }
        }

        private Task<string> DownloadStringTaskAsync(string uri)
        {
            return Task.Run(() =>
            {
                Thread.Sleep(5000);
                return "htttps://" + uri;
            });
          
        }

    }
}

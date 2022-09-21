using Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace _04__Asynchronous_Function
{
    public class _05__Async_Caching
    {
        public static void Show()
        {
            new WebPageDownloader().Show();
        }


    }
    public class WebPageDownloader
    {
        public async void Show()
        {
            // 缓存数据
            {
                string html = await GetWebPageAsync("https://www.baidu.com");
                html.Length.Dump("Characters downloaded");

                // Let's try again. It should be instant this time:
                html = await GetWebPageAsync("https://www.baidu.com");
                html.Length.Dump("Characters downloaded");

                var awaiter = GetWebPageAsync("https://www.baidu.com").GetAwaiter();
                if (awaiter.IsCompleted)
                    Console.WriteLine(awaiter.GetResult());
                else
                    awaiter.OnCompleted(() => Console.WriteLine(awaiter.GetResult()));
            }
            // 缓存任务
            {
                string html = await GetWebPageAsyncWithReturnTask("https://www.baidu.com");
                html.Length.Dump("Characters downloaded");
            }
        }
        static Dictionary<string, string> m_cacheStr = new Dictionary<string, string>();  // 数据字典
        async Task<string> GetWebPageAsync(string uri)
        {
            string html;
            if (m_cacheStr.TryGetValue(uri, out html)) return html; // 标记 IsCompleted = true 返回一个已经结束的任务
            return m_cacheStr[uri] = await new WebClient().DownloadStringTaskAsync(uri);   // 异步执行
        }

        static Dictionary<string, Task<string>> m_cacheTask = new Dictionary<string, Task<string>>(); // 任务字典
        Task<string> GetWebPageAsyncWithReturnTask(string uri)
        {
            lock (m_cacheTask)
            {
                // m_cacheTask 锁对异步方法无效
                // 即 在检查缓存、开启新任务、更新缓存 过程中 锁有效， 在 执行异步任务的 过程中锁无效
                Task<string> downloadTask;
                if (m_cacheTask.TryGetValue(uri, out downloadTask)) return downloadTask;
                return m_cacheTask[uri] = new WebClient().DownloadStringTaskAsync(uri); // 缓存任务
            }

        }
    }

}

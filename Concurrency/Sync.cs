using Nito.AsyncEx;

namespace Concurrency
{
    public class Sync
    {
        public static void Mutex()
        {
            object _mutex = new object();
            int _value = 0;
            lock (_mutex)
            {
                _value = _value + 1;
            }
        }

        public static async void SemaphoreSlim()
        {
            SemaphoreSlim _mutex = new SemaphoreSlim(1);
            int _value = 0;
            await _mutex.WaitAsync();
            try
            {
                int oldValue = _value;
                await Task.Delay(TimeSpan.FromSeconds(oldValue));
                _value = oldValue + 1;
            }
            finally
            {
                _mutex.Release();
            }
        }

        public static async void AsyncLock()
        {
            AsyncLock _mutex = new AsyncLock();
            int _value = 0;
            using (await _mutex.LockAsync())
            {
                int oldValue = _value;
                await Task.Delay(TimeSpan.FromSeconds(oldValue));
                _value = oldValue + 1;
            }
        }

        public static void ManualResetEventSlim()
        {
            ManualResetEventSlim _initialized = new ManualResetEventSlim();
            int _value = 0;

            Task.Run(() =>
            {
                _initialized.Wait();
                return _value;
            });

            Task.Run(() =>
            {
                _value = 13;
                _initialized.Set();
            });
        }

        public static void AsyncManualResetEvent()
        {
            AsyncManualResetEvent _connected =
         new AsyncManualResetEvent();

            Task.Run(async () =>
            {
                await _connected.WaitAsync();
            });

            bool connected = true;
            Task.Run(() =>
            {
                if (connected)
                    _connected.Set();
                else
                    _connected.Reset();
            });
        }

        public static async Task<string[]> SemaphoreSlim(HttpClient client, IEnumerable<string> urls)
        {
            using var semaphore = new SemaphoreSlim(10);
            Task<string>[] tasks = urls.Select(async url =>
            {
                await semaphore.WaitAsync();
                try
                {
                    return await client.GetStringAsync(url);
                }
                finally
                {
                    semaphore.Release();
                }
            }).ToArray();
            return await Task.WhenAll(tasks);
        }
    }
}

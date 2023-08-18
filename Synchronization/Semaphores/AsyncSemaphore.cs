namespace Semaphores
{
    internal class AsyncSemaphore
    {

        public static async void Show()
        {
            string[] messages = { "one", "two", "three", "four", "five", "six" };
            Task[] tasks = new Task[messages.Length];

            for (int i = 0; i < messages.Length; i++)
            {
                string message = messages[i];

                tasks[i] = Task.Run(async () =>
                {
                    await LockWithSemaphore(message);
                });
            }

            await Task.WhenAll(tasks);
        }

        private static SemaphoreSlim s_asyncLock = new SemaphoreSlim(1);
        static async Task LockWithSemaphore(string title)
        {
            Console.WriteLine($"{title} waiting for lock");
            await s_asyncLock.WaitAsync();
            try
            {
                Console.WriteLine($"{title} {nameof(LockWithSemaphore)} started");
                await Task.Delay(500);
                Console.WriteLine($"{title} {nameof(LockWithSemaphore)} ending");
            }
            finally
            {
                s_asyncLock.Release();
            }
        }
    }
}

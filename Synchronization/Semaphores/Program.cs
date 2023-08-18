// See https://aka.ms/new-console-template for more information
using Semaphores;

Console.WriteLine("Hello, World!");

AsyncSemaphore.Show();

int taskCount = 6;
int semaphoreCount = 3;
var semaphore = new SemaphoreSlim(semaphoreCount, semaphoreCount);
var tasks = new Task[taskCount];

for (int i = 0; i < taskCount; i++)
{
    tasks[i] = Task.Run(() => Do(semaphore));
}

Task.WaitAll(tasks);

Console.WriteLine("All tasks finished");

Console.ReadKey();


static void Do(SemaphoreSlim semaphore)
{
    bool isCompleted = false;
    while (!isCompleted)
    {
        if (semaphore.Wait(600))
        {
            try
            {
                Console.WriteLine($"Task {Task.CurrentId} locks the semaphore");
                Task.Delay(2000).Wait();
            }
            finally
            {
                Console.WriteLine($"Task {Task.CurrentId} releases the semaphore");
                semaphore.Release();
                isCompleted = true;
            }
        }
        else
        {
            Console.WriteLine($"Timeout for task {Task.CurrentId}; wait again");
        }
    }
}
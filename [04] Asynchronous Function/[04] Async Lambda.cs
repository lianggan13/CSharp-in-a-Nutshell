using System;
using System.Threading.Tasks;

namespace _04__Asynchronous_Function
{
    public class _04__Async_Lambda
    {
        public async void Show()
        {
            // Unnamed asynchronous method:
            {
                Func<Task> unnamed = async () =>
                {
                    await Task.Delay(1000);
                    Console.WriteLine("Foo");
                };

                // We can call the two in the same way:
                await NamedMethod();
                await unnamed();
            }
            // with return
            {
                Func<Task<int>> unnamed = async () =>
                {
                    await Task.Delay(1000);
                    return 123;
                };

                int answer = await unnamed();
                Console.WriteLine("answer:" + answer);
            }
        }

        async Task NamedMethod()
        {
            await Task.Delay(1000);
            Console.WriteLine("Foo");
        }
    }


}

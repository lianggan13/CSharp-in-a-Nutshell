using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _04__Asynchronous_Function
{
    public class _07__Async_Stream
    {
        public static async void Show()
        {
            Console.WriteLine($"Starting async Task<IEnumerable<int>>. Data arrives in one group.");

            foreach (var d in await RangeTaskAsync(0, 10, 500))
                Console.WriteLine(d);

            //foreach (var d in await RangeNumAsync(0, 10, 500))
            //    Console.WriteLine(d);
        }


        public async static Task<IEnumerable<int>> RangeTaskAsync(int start, int count, int delay)
        {
            List<int> data = new List<int>();
            for (int i = start; i < start + count; i++)
            {
                await Task.Delay(delay);
                data.Add(i);
            }
            return data;
        }


        //public async static IAsyncEnumerable<int> RangeNumAsync(int start, int count, int delay)
        //{
        //    for (int i = start; i < start + count; i++)
        //    {
        //        await Task.Delay(delay);
        //        yield return i;
        //    }
        //}
    }
}

using System;
using System.Threading.Tasks;

namespace _04__Asynchronous_Function
{
    public class _03__Returning_Task_of_TResult
    {
        public void Show()
        {
            Go();
        }
        async Task Go()
        {
            await PrintAnswerToLife();
            Console.WriteLine("Done");
        }
        async Task PrintAnswerToLife()
        {
            int answer = await GetAnswerToLife();
            Console.WriteLine(answer);
        }
        async Task<int> GetAnswerToLife()
        {
            await Task.Delay(5000);
            int answer = 21 * 2;
            return answer;
        }
    }
}

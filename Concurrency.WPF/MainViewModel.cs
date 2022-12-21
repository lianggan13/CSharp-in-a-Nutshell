using Concurrency.WPF.Model;
using System;
using System.Threading.Tasks;

namespace Concurrency.WPF
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            //MyValue = NotifyTask.Create(CalculateMyValueAsync());
            MyValue = new BindableTask<int>(CalculateMyValueAsync());
        }

        public BindableTask<int> MyValue { get; private set; }

        private async Task<int> CalculateMyValueAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            //throw new Exception("a error occured...");
            return 13;
        }
    }
}

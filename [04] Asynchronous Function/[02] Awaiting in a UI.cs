using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace _04__Asynchronous_Function
{
    public class _02__Awaiting_in_a_UI
    {
        public static void Show()
        {
            // sync UI
            {
                Thread NetServer = new Thread(() => { new SyncUI().ShowDialog(); });
                NetServer.SetApartmentState(ApartmentState.STA);
                NetServer.IsBackground = true;
                NetServer.Start();
            }
            // async UI
            {
                Thread NetServer = new Thread(() => { new AsyncUI().ShowDialog(); });
                NetServer.SetApartmentState(ApartmentState.STA);
                NetServer.IsBackground = true;
                NetServer.Start();
            }
            // async UI DownloadData
            {
                Thread NetServer = new Thread(() => { new AsyncUIDownloadData().ShowDialog(); });
                NetServer.SetApartmentState(ApartmentState.STA);
                NetServer.IsBackground = true;
                NetServer.Start();
            }


        }

    }

    class SyncUI : Window       // Notice how the window becomes unresponsive while working
    {
        Button _button = new Button { Content = "Go" };
        TextBlock _results = new TextBlock();

        public SyncUI()
        {
            var panel = new StackPanel();
            panel.Children.Add(_button);
            panel.Children.Add(_results);
            Content = panel;
            _button.Click += (sender, args) => Go();
        }

        void Go()
        {
            for (int i = 1; i < 5; i++)
                _results.Text += GetPrimesCount(i * 1000000, 1000000) +
                    " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1) + Environment.NewLine;
        }

        int GetPrimesCount(int start, int count)
        {
            return ParallelEnumerable.Range(start, count).Count(n =>
          Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0));
        }
    }

    class AsyncUI : Window       // Notice how the window becomes unresponsive while working
    {
        Button _button = new Button { Content = "Go" };
        TextBlock _results = new TextBlock();

        public AsyncUI()
        {
            var panel = new StackPanel();
            panel.Children.Add(_button);
            panel.Children.Add(_results);
            Content = panel;
            _button.Click += (sender, args) => Go();
        }

        async void Go()
        {
            _button.IsEnabled = false;

            for (int i = 1; i < 5; i++)
                _results.Text += await GetPrimesCountAsync(i * 1000000, 1000000) +
                    " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1) + Environment.NewLine;

            _button.IsEnabled = true;
        }

        Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run(() =>
               ParallelEnumerable.Range(start, count).Count(n =>
                 Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));
        }
    }

    class AsyncUIDownloadData : Window       // Notice how the window becomes unresponsive while working
    {
        Button _button = new Button { Content = "Go" };
        TextBlock _results = new TextBlock();

        public AsyncUIDownloadData()
        {
            var panel = new StackPanel();
            panel.Children.Add(_button);
            panel.Children.Add(_results);
            Content = panel;
            _button.Click += (sender, args) => Go();
        }

        async void Go()
        {
            _button.IsEnabled = false;
            string[] urls = "www.albahari.com www.oreilly.com www.linqpad.net".Split();
            int totalLength = 0;
            try
            {
                foreach (string url in urls)
                {
                    var uri = new Uri("http://" + url);
                    byte[] data = await new WebClient().DownloadDataTaskAsync(uri);
                    _results.Text += "Length of " + url + " is " + data.Length + Environment.NewLine;
                    totalLength += data.Length;
                }
                _results.Text += "Total length: " + totalLength;
            }
            catch (WebException ex)
            {
                _results.Text += "Error: " + ex.Message;
            }
            finally { _button.IsEnabled = true; }
        }

        Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run(() =>
               ParallelEnumerable.Range(start, count).Count(n =>
                 Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));
        }
    }
}

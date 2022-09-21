using RestSharp;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Concurrency.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Throttle_Click(object sender, RoutedEventArgs e)
        {
            Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
            handler => (s, a) => handler(s, a),
            handler => MouseMove += handler,
            handler => MouseMove -= handler)
            .Select(x => x.EventArgs.GetPosition(this))
            .Throttle(TimeSpan.FromSeconds(1))
            .Subscribe(x => Trace.WriteLine(
            $"{DateTime.Now.Second}: Saw {x.X + x.Y}"));
        }

        private void Sample_Click(object sender, RoutedEventArgs e)
        {
            Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
              handler => (s, a) => handler(s, a),
              handler => MouseMove += handler,
              handler => MouseMove -= handler)
              .Select(x => x.EventArgs.GetPosition(this))
              .Sample(TimeSpan.FromSeconds(1))
              .Subscribe(x => Trace.WriteLine(
              $"{DateTime.Now.Second}: Saw {x.X + x.Y}"));
        }

        private void TimeoutWithSelector_Click(object sender, RoutedEventArgs e)
        {
            IObservable<Point> clicks =
                   Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
                       handler => (s, a) => handler(s, a),
                       handler => MouseDown += handler,
                       handler => MouseDown -= handler)
                   .Select(x => x.EventArgs.GetPosition(this));

            Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                    handler => (s, a) => handler(s, a),
                    handler => MouseMove += handler,
                    handler => MouseMove -= handler)
                .Select(x => x.EventArgs.GetPosition(this))
                .Timeout(TimeSpan.FromSeconds(10), clicks)
                .Subscribe(
                    x => Trace.WriteLine($"{DateTime.Now.Second}: Saw {x.X},{x.Y}"),
                    ex => Trace.WriteLine(ex));
        }

        private void TaskWithTimeout_Click(object sender, RoutedEventArgs e)
        {

            RequestPlcUri();
            return;

            HttpClient client = new HttpClient();
            client.GetStringAsync("http://www.example.com/").ToObservable()
                   .Timeout(TimeSpan.FromSeconds(5))
                   .Subscribe(
                       x => Trace.WriteLine($"{DateTime.Now.Second}: Saw {x.Length}"),
                       ex => Trace.WriteLine(ex));
            Trace.WriteLine("-------------");
        }

        private void RequestPlcUri()
        {
            RestRequest request = new RestRequest("http://10.6.1.20:81");
            request.Method = Method.Get;
            request.Timeout = 5000;

            RestClient client = new RestClient();

            client.ExecuteAsync(request).ToObservable()
                .Timeout(TimeSpan.FromSeconds(2))
                .Select(t => t.StatusCode)
                   .Subscribe(
                      x => Trace.WriteLine($"{DateTime.Now.Second}: Saw {x}"),
                      ex => Trace.WriteLine(ex));
        }

        private void BufferWithTime_Click(object sender, RoutedEventArgs e)
        {
            Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
               handler => (s, a) => handler(s, a),
               handler => MouseMove += handler,
               handler => MouseMove -= handler)
           .Buffer(TimeSpan.FromSeconds(1))
           .Subscribe(x => Trace.WriteLine(
               $"{DateTime.Now.Second}: Saw {x.Count} items."));
        }

        private void ObserveOn_Click(object sender, RoutedEventArgs e)
        {
            SynchronizationContext uiContext = SynchronizationContext.Current;
            Trace.WriteLine($"UI thread is {Environment.CurrentManagedThreadId}");
            Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                    handler => (s, a) => handler(s, a),
                    handler => MouseMove += handler,
                    handler => MouseMove -= handler)
                .Select(evt => evt.EventArgs.GetPosition(this))
                .ObserveOn(Scheduler.Default)
                .Select(position =>
                {
                    // Complex calculation
                    Thread.Sleep(100);
                    var result = position.X + position.Y;
                    Trace.WriteLine($"Calculated result {result} on thread {Environment.CurrentManagedThreadId}");
                    return result;
                })
                .ObserveOn(uiContext)
                .Subscribe(x => Trace.WriteLine($"Result {x} on thread {Environment.CurrentManagedThreadId}"));
        }

        private void BufferWithCount_Click(object sender, RoutedEventArgs e)
        {
            Observable.Interval(TimeSpan.FromSeconds(1))
             .Buffer(2)
             .Subscribe(x => Trace.WriteLine(
                 $"{DateTime.Now.Second}: Got {x[0]} and {x[1]}"));
        }

        private void WindowWithCount_Click(object sender, RoutedEventArgs e)
        {
            Observable.Interval(TimeSpan.FromSeconds(1))
           .Window(2)
           .Subscribe(group =>
           {
               Trace.WriteLine($"{DateTime.Now.Second}: Starting new group");
               group.Subscribe(
                   x => Trace.WriteLine($"{DateTime.Now.Second}: Saw {x}"),
                   () => Trace.WriteLine($"{DateTime.Now.Second}: Ending group"));
           });
        }

        void Button_Click2(object sender, RoutedEventArgs e)
        {
            SynchronizationContext uiContext = SynchronizationContext.Current;
            Trace.WriteLine($"UI thread is {Environment.CurrentManagedThreadId}");
            Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                    handler => (s, a) => handler(s, a),
                    handler => MouseMove += handler,
                    handler => MouseMove -= handler)
                .Select(evt => evt.EventArgs.GetPosition(this))
                .ObserveOn(Scheduler.Default)
                .Select(position =>
                {
                    // Complex calculation
                    Thread.Sleep(100);
                    var result = position.X + position.Y;
                    Trace.WriteLine($"Calculated result {result} on thread {Environment.CurrentManagedThreadId}");
                    return result;
                })
                .ObserveOn(uiContext)
                .Subscribe(x => Trace.WriteLine($"Result {x} on thread {Environment.CurrentManagedThreadId}"));
        }


        void Button_Click3(object sender, RoutedEventArgs e)
        {
            IObservable<Point> clicks =
                Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
                    handler => (s, a) => handler(s, a),
                    handler => MouseDown += handler,
                    handler => MouseDown -= handler)
                .Select(x => x.EventArgs.GetPosition(this));

            Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                    handler => (s, a) => handler(s, a),
                    handler => MouseMove += handler,
                    handler => MouseMove -= handler)
                .Select(x => x.EventArgs.GetPosition(this))
                .Timeout(TimeSpan.FromSeconds(1), clicks)
                .Subscribe(
                    x => Trace.WriteLine($"{DateTime.Now.Second}: Saw {x.X},{x.Y}"),
                    ex => Trace.WriteLine(ex));
        }


        void Button_Click4(object sender, RoutedEventArgs e)
        {
            // Buffer with count
            Observable.Interval(TimeSpan.FromSeconds(1))
               .Buffer(2)
               .Subscribe(x => Trace.WriteLine(
                   $"{DateTime.Now.Second}: Got {x[0]} and {x[1]}"));

            // Window with count
            Observable.Interval(TimeSpan.FromSeconds(1))
            .Window(2)
            .Subscribe(group =>
            {
                Trace.WriteLine($"{DateTime.Now.Second}: Starting new group");
                group.Subscribe(
                    x => Trace.WriteLine($"{DateTime.Now.Second}: Saw {x}"),
                    () => Trace.WriteLine($"{DateTime.Now.Second}: Ending group"));
            });

            // Buffer with time
            Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                handler => (s, a) => handler(s, a),
                handler => MouseMove += handler,
                handler => MouseMove -= handler)
            .Buffer(TimeSpan.FromSeconds(1))
            .Subscribe(x => Trace.WriteLine(
                $"{DateTime.Now.Second}: Saw {x.Count} items."));

            // Throttle
            Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                handler => (s, a) => handler(s, a),
                handler => MouseMove += handler,
                handler => MouseMove -= handler)
            .Select(x => x.EventArgs.GetPosition(this))
            .Throttle(TimeSpan.FromSeconds(1))
            .Subscribe(x => Trace.WriteLine(
                $"{DateTime.Now.Second}: Saw {x.X + x.Y}"));

            // Sample
            Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
               handler => (s, a) => handler(s, a),
               handler => MouseMove += handler,
               handler => MouseMove -= handler)
           .Select(x => x.EventArgs.GetPosition(this))
           .Sample(TimeSpan.FromSeconds(1))
           .Subscribe(x => Trace.WriteLine(
               $"{DateTime.Now.Second}: Saw {x.X + x.Y}"));

            // Task to Observable with Tiemout 
            void GetWithTimeout(HttpClient client)
            {
                client.GetStringAsync("http://www.example.com/").ToObservable()
                    .Timeout(TimeSpan.FromSeconds(1))
                    .Subscribe(
                        x => Trace.WriteLine($"{DateTime.Now.Second}: Saw {x.Length}"),
                        ex => Trace.WriteLine(ex));
            }
        }
    }
}

using System.Diagnostics;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Timers;
using System.Windows;
using Utilities;

namespace Concurrency
{
    public class Rx
    {
        public static void Show()
        {
            SubscribeWithDefer();
        }

        public static void SubEventStream()
        {
            {
                Progress<int> progress = new Progress<int>();
                IObservable<EventPattern<int>> progressReports =
                    //Observable.FromEventPattern<int>(
                    Observable.FromEventPattern<EventHandler<int>, int>(
                        //handler => (s, a) => handler(s, a),
                        handler => progress.ProgressChanged += handler,
                        handler => progress.ProgressChanged -= handler);

                progressReports.Subscribe(data => Console.WriteLine("OnNext: " + data.EventArgs));

                for (int i = 0; i < 3; i++)
                {
                    (progress as IProgress<int>).Report(i);
                }
            }

            {
                System.Timers.Timer timer = new System.Timers.Timer(interval: 1000) { Enabled = true };
                IObservable<EventPattern<ElapsedEventArgs>> ticks =
                    //public static IObservable<EventPattern<TEventArgs>> FromEventPattern<TDelegate, TEventArgs>(Func<EventHandler<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
                    Observable.FromEventPattern<ElapsedEventHandler, ElapsedEventArgs>(
                        handler => (s, a) => handler(s, a),
                        handler => timer.Elapsed += handler,
                        handler => timer.Elapsed -= handler);
                ticks.Subscribe(data => Console.WriteLine("OnNext: " + data.EventArgs.SignalTime));

                timer.Start();
            }

            {
                var client = new WebClient();
                IObservable<EventPattern<object>> downloadedStrings =
                    Observable.FromEventPattern(client, nameof(WebClient.DownloadStringCompleted));
                downloadedStrings.Subscribe(
                  onNext: data =>
                  {
                      var eventArgs = (DownloadStringCompletedEventArgs)data.EventArgs;
                      if (eventArgs.Error != null)
                          Console.WriteLine("OnNext: (Error) " + eventArgs.Error);
                      else
                          Console.WriteLine("OnNext: " + eventArgs.Result);
                  },
                    onError: ex => Console.WriteLine("OnError: " + ex.ToString()),
                    onCompleted: () => Console.WriteLine("OnCompleted"));

                client.DownloadStringAsync(new Uri("http://invalid.example.com/"));
            }
        }

        public static void ObserveOnUI()
        {
            void Button_Click_1(object sender, RoutedEventArgs e)
            {
                Trace.WriteLine($"UI thread is {Environment.CurrentManagedThreadId}");
                Observable.Interval(TimeSpan.FromSeconds(1))
                    .Subscribe(x => Trace.WriteLine($"Interval {x} on thread {Environment.CurrentManagedThreadId}"));
            }

            void Button_Click_2(object sender, RoutedEventArgs e)
            {
                SynchronizationContext uiContext = SynchronizationContext.Current;
                Trace.WriteLine($"UI thread is {Environment.CurrentManagedThreadId}");
                Observable.Interval(TimeSpan.FromSeconds(1))
                    .ObserveOn(uiContext)
                    .Subscribe(x => Trace.WriteLine($"Interval {x} on thread {Environment.CurrentManagedThreadId}"));
            }

            void Button_Click_3(object sender, RoutedEventArgs e)
            {
                //Window
                //SynchronizationContext uiContext = SynchronizationContext.Current;
                //Trace.WriteLine($"UI thread is {Environment.CurrentManagedThreadId}");
                //Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                //        handler => (s, a) => handler(s, a),
                //        handler => MouseMove += handler,
                //        handler => MouseMove -= handler)
                //    .Select(evt => evt.EventArgs.GetPosition(this))
                //    .ObserveOn(Scheduler.Default)
                //    .Select(position =>
                //    {
                //        // Complex calculation
                //        Thread.Sleep(100);
                //        var result = position.X + position.Y;
                //        Trace.WriteLine($"Calculated result {result} on thread {Environment.CurrentManagedThreadId}");
                //        return result;
                //    })
                //    .ObserveOn(uiContext)
                //    .Subscribe(x => Trace.WriteLine($"Result {x} on thread {Environment.CurrentManagedThreadId}"));
            }

        }

        public static void GroupObservableEvent()
        {
            {
                Observable.Interval(TimeSpan.FromSeconds(1))
                          .Buffer(2)
                          .Subscribe(x => Trace.WriteLine($"{DateTime.Now.Second}: Got {x[0]} and {x[1]}"));
            }

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

            {
                void Button_Click(object sender, RoutedEventArgs e)
                {
                    //Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                    //        handler => (s, a) => handler(s, a),
                    //        handler => MouseMove += handler,
                    //        handler => MouseMove -= handler)
                    //    .Buffer(TimeSpan.FromSeconds(1))
                    //    .Subscribe(x => Trace.WriteLine(
                    //        $"{DateTime.Now.Second}: Saw {x.Count} items."));
                }
            }
        }

        public static void ThrottleAndSample()
        {
            {
                void Button_Click(object sender, RoutedEventArgs e)
                {
                    //Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                    //      handler => (s, a) => handler(s, a),
                    //      handler => MouseMove += handler,
                    //      handler => MouseMove -= handler)
                    //  .Select(x => x.EventArgs.GetPosition(this))
                    //  .Throttle(TimeSpan.FromSeconds(1))
                    //  .Subscribe(x => Trace.WriteLine(
                    //      $"{DateTime.Now.Second}: Saw {x.X + x.Y}"));
                }
            }

            {
                void Button_Click(object sender, RoutedEventArgs e)
                {
                    //Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                    //        handler => (s, a) => handler(s, a),
                    //        handler => MouseMove += handler,
                    //        handler => MouseMove -= handler)
                    //    .Select(x => x.EventArgs.GetPosition(this))
                    //    .Sample(TimeSpan.FromSeconds(1))
                    //    .Subscribe(x => Trace.WriteLine(
                    //        $"{DateTime.Now.Second}: Saw {x.X + x.Y}"));
                }
            }
        }

        public static void ObserveTimeout()
        {
            {
                void GetWithTimeout(HttpClient client)
                {
                    client.GetStringAsync("http://www.example.com/").ToObservable()
                        .Timeout(TimeSpan.FromSeconds(1))
                        .Subscribe(
                            x => Trace.WriteLine($"{DateTime.Now.Second}: Saw {x.Length}"),
                            ex => Trace.WriteLine(ex));
                }
            }

            {
                void Button_Click(object sender, RoutedEventArgs e)
                {
                    //Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                    //        handler => (s, a) => handler(s, a),
                    //        handler => MouseMove += handler,
                    //        handler => MouseMove -= handler)
                    //    .Select(x => x.EventArgs.GetPosition(this))
                    //    .Timeout(TimeSpan.FromSeconds(1))
                    //    .Subscribe(
                    //        x => Trace.WriteLine($"{DateTime.Now.Second}: Saw {x.X + x.Y}"),
                    //        ex => Trace.WriteLine(ex));
                }
            }

            {
                void Button_Click(object sender, RoutedEventArgs e)
                {
                    //IObservable<Point> clicks =
                    //    Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
                    //        handler => (s, a) => handler(s, a),
                    //        handler => MouseDown += handler,
                    //        handler => MouseDown -= handler)
                    //    .Select(x => x.EventArgs.GetPosition(this));

                    //Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                    //        handler => (s, a) => handler(s, a),
                    //        handler => MouseMove += handler,
                    //        handler => MouseMove -= handler)
                    //    .Select(x => x.EventArgs.GetPosition(this))
                    //    .Timeout(TimeSpan.FromSeconds(1), clicks)
                    //    .Subscribe(
                    //        x => Trace.WriteLine($"{DateTime.Now.Second}: Saw {x.X},{x.Y}"),
                    //        ex => Trace.WriteLine(ex));
                }
            }
        }

        public static void SubscribeWithDefer()
        {
            //var invokeServerObservable = GetValueAsync().ToObservable();

            var invokeServerObservable = Observable.Defer(
                () => GetValueAsync().ToObservable());
            invokeServerObservable.Subscribe(result =>
            {
                LogHelper.Info($"{result}");
            });
            invokeServerObservable.Subscribe(result =>
            {
                LogHelper.Info($"{result}");
            });
        }

        static async Task<int> GetValueAsync()
        {
            Console.WriteLine("Calling server...");
            await Task.Delay(TimeSpan.FromSeconds(2));
            Console.WriteLine("Returning result...");
            return 13;
        }
    }
}

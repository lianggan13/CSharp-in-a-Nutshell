using System.Net;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks.Dataflow;
using Utilities;

namespace Concurrency
{
    public class TAP
    {
        public static async Task Show()
        {


            {
                var ss = ObservableAsync(new HttpClient());
                //LogHelper.Info($"{ss.Content.ReadAsStringAsync().Result}");

                var sss = await ss.Publish();
                LogHelper.Info($"{sss.Content.ReadAsStringAsync().Result}");

                //ss.Subscribe(x => LogHelper.Info($"{x.Content.ReadAsStringAsync().Result}"));
            }

            return;

            {
                WebClient webClient = new WebClient();
                Uri uri = new Uri("https://www.baidu.com/");
                string content = await TAP.DownloadStringTaskAsync(webClient, uri);
                LogHelper.Info(content);
            }

            {
                WebRequest request = WebRequest.Create(new Uri("https://www.baidu.com/"));
                WebResponse response = await TAP.GetResponseAsync(request);
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new System.IO.StreamReader(responseStream, Encoding.Default))
                    {
                        string content = reader.ReadToEnd();
                        LogHelper.Info(content);
                    }
                }
            }

            {
                try
                {
                    string result = await TAP.AuthorizeAsync(1);
                    LogHelper.Info(result);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.Message, ex);
                }
            }

            {
                BlockAsObservable();

                BlockAsObserver();
            }
        }

        // EAP(+OperationAsync、+OperationCompleted) --> TAP(+TaskCompletionSource)
        public static Task<string> DownloadStringTaskAsync(WebClient client,
                Uri address)
        {
            var tcs = new TaskCompletionSource<string>();

            // The event handler will complete the task and unregister itself.
            DownloadStringCompletedEventHandler handler = null;
            handler = (_, e) =>
            {
                client.DownloadStringCompleted -= handler;
                if (e.Cancelled)
                    tcs.TrySetCanceled();
                else if (e.Error != null)
                    tcs.TrySetException(e.Error);
                else
                    tcs.TrySetResult(e.Result);
            };

            // Register for the event and *then* start the operation.
            client.DownloadStringCompleted += handler;
            client.DownloadStringAsync(address);

            return tcs.Task;
        }

        // AMP(+BeginOperation、+EndOperation) --> TAP(+FromAsync)
        public static Task<WebResponse> GetResponseAsync(WebRequest client)
        {
            Task<WebResponse> task = Task<WebResponse>.Factory.FromAsync(client.BeginGetResponse,
                client.EndGetResponse, null);
            return task;
        }

        // Method callback --> TAP(+TaskCompletionSource)
        public static Task<string> AuthorizeAsync(int workNo)
        {
            var tcs = new TaskCompletionSource<string>();
            Authorize(workNo, (result, exception) =>
            {
                if (exception != null)
                    tcs.TrySetException(exception);
                else
                    tcs.TrySetResult(result);
            });

            return tcs.Task;
        }

        public static void Authorize(int workNo, Action<string, Exception?> callback)
        {
            string msg;
            Exception? ex = null;
            if (workNo != 13)
            {
                msg = "Authorization failed";
                ex = new Exception("WorkNo error");
            }
            else
            {
                msg = "Authorization Success";
            }

            callback?.Invoke(msg, ex);
        }

        public static async Task ParallelAsync()
        {
            var source = Enumerable.Range(0, 10);
            Action<int> body = x => { Console.Write($"{x} "); };

            await Task.Run(() => Parallel.ForEach(source, body));

            Console.WriteLine();
        }


        public static async Task ObservableAsync()
        {
            IObservable<int> observable = Observable.Range(0, 10);
            int lastElement = await observable.LastAsync();
            int lastElement2 = await observable;
            int nextElement = await observable.FirstAsync();
            IList<int> allElements = await observable.ToList();
            LogHelper.Info(string.Join(" ", allElements));
        }

        public static IObservable<HttpResponseMessage> ObservableAsync(HttpClient client)
        {
            IObservable<HttpResponseMessage> obs = null;

            {
                Task<HttpResponseMessage> task =
                             client.GetAsync("http://www.example.com/");
                obs = task.ToObservable();
            }

            {
                obs = Observable.StartAsync(
                            token => client.GetAsync("http://www.example.com/", token));
            }

            {
                obs = Observable.FromAsync(
                    token => client.GetAsync("http://www.example.com/", token));
            }

            {
                IObservable<string> urls = new string[] { "http://www.example.com/" }.ToObservable();
                obs = urls.SelectMany((url, token) => client.GetAsync(url, token));
            }
            return obs;
        }

        // Block(BufferBlock) --> Observable
        public static void BlockAsObservable()
        {
            var buffer = new BufferBlock<int>();
            IObservable<int> integers = buffer.AsObservable();
            integers.Subscribe(
                data => LogHelper.Info($"{data}"),
                ex => LogHelper.Error(ex.Message, ex),
                () => LogHelper.Info("Done"));

            buffer.Post(13);
            buffer.Complete();
        }

        // Block(ActionBlock) --> Observer
        public static void BlockAsObserver()
        {
            IObservable<DateTimeOffset> ticks =
                    Observable.Interval(TimeSpan.FromSeconds(1))
                    .Timestamp()
                    .Select(x => x.Timestamp)
                    .Take(5);

            var display = new ActionBlock<DateTimeOffset>(x => LogHelper.Info($"{x}"));
            ticks.Subscribe(display.AsObserver()); // observer#1
            ticks.Subscribe(t => LogHelper.Info($"{t.Second}")); // observer#2
        }
    }
}

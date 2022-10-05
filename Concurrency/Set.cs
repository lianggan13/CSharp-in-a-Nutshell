using Nito.AsyncEx;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;
using Utilities;


namespace Concurrency
{
    public class Set
    {
        public static void Show()
        {
            //ConcurrentDictionary();

            BufferBlock();
        }

        public static void ImmutableStack()
        {
            ImmutableStack<int> stack = ImmutableStack<int>.Empty;
            stack = stack.Push(13);
            ImmutableStack<int> biggerStack = stack.Push(7);

            // Displays "7" followed by "13".
            foreach (int item in biggerStack)
                Trace.WriteLine(item);

            // Only displays "13".
            foreach (int item in stack)
                Trace.WriteLine(item);
        }

        public static void ImmutableQueue()
        {
            ImmutableQueue<int> queue = ImmutableQueue<int>.Empty;
            queue = queue.Enqueue(13);
            queue = queue.Enqueue(7);

            // Displays "13" followed by "7".
            foreach (int item in queue)
                Trace.WriteLine(item);

            int nextItem;
            queue = queue.Dequeue(out nextItem);
            // Displays "13"
            Trace.WriteLine(nextItem);
        }

        public static void ImmutableList()
        {
            ImmutableList<int> list = ImmutableList<int>.Empty;
            list = list.Insert(0, 13);
            list = list.Insert(0, 7);

            // Displays "7" followed by "13".
            foreach (int item in list)
                Trace.WriteLine(item);

            list = list.RemoveAt(1);
        }

        public static void ImmutableHashSet()
        {
            ImmutableHashSet<int> hashSet = ImmutableHashSet<int>.Empty;
            hashSet = hashSet.Add(13);
            hashSet = hashSet.Add(7);

            // Displays "7" and "13" in an unpredictable order.
            foreach (int item in hashSet)
                Trace.WriteLine(item);

            hashSet = hashSet.Remove(7);
        }

        public static void ImmutableDictionary()
        {
            ImmutableDictionary<int, string> dictionary = ImmutableDictionary<int, string>.Empty;
            dictionary = dictionary.Add(10, "Ten");
            dictionary = dictionary.Add(21, "Twenty-One");
            dictionary = dictionary.SetItem(10, "Diez");

            // Displays "10Diez" and "21Twenty-One" in an unpredictable order.
            foreach (KeyValuePair<int, string> item in dictionary)
                Trace.WriteLine(item.Key + item.Value);

            string ten = dictionary[10];
            // ten == "Diez"

            dictionary = dictionary.Remove(21);
        }

        public static void ConcurrentDictionary()
        {
            var dictionary = new ConcurrentDictionary<int, string>();
            string newValue = dictionary.AddOrUpdate(0,
               addValueFactory: key =>
               {
                   string value = "Zero";
                   return value;
               },
               updateValueFactory: (key, oldValue) =>
               {
                   string newValue = "Zero";
                   return newValue;
               });


            // Using the same "dictionary" as above.
            // Adds (or updates) key 0 to have the value "Zero".
            dictionary[0] = "Zero1";


            // Using the same "dictionary" as above.
            bool keyExists = dictionary.TryGetValue(0, out string currentValue);


            // Using the same "dictionary" as above.
            bool keyExisted = dictionary.TryRemove(0, out string removedValue);
        }


        public static void BlockingCollection()
        {
            BlockingCollection<int> blockingQueue = new BlockingCollection<int>();

            blockingQueue.Add(7);
            blockingQueue.Add(13);
            blockingQueue.CompleteAdding();

            // Displays "7" followed by "13".
            foreach (int item in blockingQueue.GetConsumingEnumerable())
                Trace.WriteLine(item);
        }

        public static void BufferBlock()
        {
            var _asyncQueue = new BufferBlock<int>(
                new DataflowBlockOptions { BoundedCapacity = 1 });
            // Producer code
            Task.Run(async () =>
            {
                // This Send completes immediately.
                await _asyncQueue.SendAsync(7);

                // This Send (asynchronously) waits for the 7 to be removed
                // before it enqueues the 13.
                await _asyncQueue.SendAsync(13);
                _asyncQueue.Complete();
            });

            // Consumer code
            Task.Run(async () =>
            {
                // Displays "7" followed by "13".
                while (await _asyncQueue.OutputAvailableAsync()) // just for a single consumer
                    LogHelper.Info($"{await _asyncQueue.ReceiveAsync()}");
            });

            Task.Run(async () =>
            {
                while (true)
                {
                    int item;
                    try
                    {
                        item = await _asyncQueue.ReceiveAsync(); // for multiple consumers
                    }
                    catch (InvalidOperationException ex)
                    {
                        LogHelper.Error(ex.Message, ex);
                        break;
                    }
                    LogHelper.Info($"{item}");
                }
            });
        }

        public static async Task AsyncProducerConsumerQueue()
        {
            var _asyncQueue = new AsyncProducerConsumerQueue<int>();

            // Producer code
            await _asyncQueue.EnqueueAsync(7);
            await _asyncQueue.EnqueueAsync(13);
            _asyncQueue.CompleteAdding();

            // Consumer code
            // Displays "7" followed by "13".
            while (await _asyncQueue.OutputAvailableAsync())
                Trace.WriteLine(await _asyncQueue.DequeueAsync());

            while (true)
            {
                int item;
                try
                {
                    item = await _asyncQueue.DequeueAsync();
                }
                catch (InvalidOperationException)
                {
                    break;
                }
                Trace.WriteLine(item);
            }
        }

        public static async Task AsyncCollection()
        {
            var _asyncStack = new AsyncCollection<int>(
                new ConcurrentStack<int>(), maxCount: 1);
            var _asyncBag = new AsyncCollection<int>(
                new ConcurrentBag<int>());

            // Producer code
            await _asyncStack.AddAsync(7);
            await _asyncStack.AddAsync(13);
            _asyncStack.CompleteAdding();

            // Consumer code
            // Displays "13" followed by "7".
            while (await _asyncStack.OutputAvailableAsync())
                Trace.WriteLine(await _asyncStack.TakeAsync());

            while (true)
            {
                int item;
                try
                {
                    item = await _asyncStack.TakeAsync();
                }
                catch (InvalidOperationException)
                {
                    break;
                }
                Trace.WriteLine(item);
            }
        }

        public static void ChannelQueue()
        {
            Channel<int> queue = Channel.CreateBounded<int>(10);

            // Producer code
            ChannelWriter<int> writer = queue.Writer;
            Task.Run(async () =>
            {
                await writer.WriteAsync(7);
                await writer.WriteAsync(13);
                writer.Complete();
            });

            // Consumer code
            ChannelReader<int> reader = queue.Reader;
            Task.Run(async () =>
            {
                while (await reader.WaitToReadAsync())
                    while (reader.TryRead(out int value))
                        Trace.WriteLine(value);
            });
        }
    }
}

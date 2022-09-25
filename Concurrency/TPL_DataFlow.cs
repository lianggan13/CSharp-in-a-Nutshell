using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Concurrency
{
    public class TPL_DataFlow
    {
        public static void SimpleLink()
        {
            var multiplyBlock = new TransformBlock<int, int>(item =>
            {
                int result = item * 2;
                Console.WriteLine($"item * 2:{result}");

                return result;
            });
            var subtractBlock = new TransformBlock<int, int>(item =>
            {
                int result = item - 2;
                Console.WriteLine($"item - 2:{item - 2}");

                return result;
            });

            multiplyBlock.LinkTo(subtractBlock);

            multiplyBlock.Post(3);
        }

        public static async void TransException()
        {
            try
            {
                var multiplyBlock = new TransformBlock<int, int>(item =>
                {
                    if (item == 1)
                        throw new InvalidOperationException("Blech.");
                    return item * 2;
                });
                var subtractBlock = new TransformBlock<int, int>(item =>
                {
                    return item - 2;
                });
                multiplyBlock.LinkTo(subtractBlock,
                    new DataflowLinkOptions { PropagateCompletion = true });
                multiplyBlock.Post(1);
                await subtractBlock.Completion;
            }
            catch (AggregateException aex)
            {
                // The exception is caught here.
            }
        }


        public static void SimpleUnlink()
        {
            var multiplyBlock = new TransformBlock<int, int>(item =>
            {
                int result = item * 2;
                Console.WriteLine($"item * 2:{result}");

                return result;
            });
            var subtractBlock = new TransformBlock<int, int>(item =>
            {
                int result = item - 2;
                Console.WriteLine($"item - 2:{item - 2}");

                return result;
            });

            IDisposable link = multiplyBlock.LinkTo(subtractBlock);
            multiplyBlock.Post(1);
            multiplyBlock.Post(2);

            // Unlink the blocks.
            // The data posted above may or may not have already gone through the link.
            // In real-world code, consider a using block rather than calling Dispose.
            link.Dispose();
        }

        public static void LimitCapacity()
        {
            var sourceBlock = new BufferBlock<int>();
            var options = new DataflowBlockOptions { BoundedCapacity = 1 };
            var targetBlockA = new BufferBlock<int>(options);
            var targetBlockB = new BufferBlock<int>(options);

            sourceBlock.LinkTo(targetBlockA);
            sourceBlock.LinkTo(targetBlockB);


            Task.Run(async () =>
            {
                Random rand = new Random();
                while (true)
                {
                    int n = rand.Next(99);
                    //sourceBlock.Post(n);
                    await sourceBlock.SendAsync(n);
                    Console.WriteLine($"<< {nameof(sourceBlock)}:{n}");
                    await Task.Delay(300);
                }
            });

            Task.Run(async () =>
            {
                Random rand = new Random();
                while (await sourceBlock.OutputAvailableAsync())
                {
                    //int n = sourceBlock.Receive();
                    int n = await sourceBlock.ReceiveAsync();
                    Console.WriteLine($">> {nameof(sourceBlock)}:{n}");
                }
            });

            Task.Run(() =>
            {
                Random rand = new Random();
                while (true)
                {
                    int n = targetBlockA.Receive();
                    Console.WriteLine($">> {nameof(targetBlockA)}:{n}");

                    Thread.Sleep(rand.Next(5000));
                }
            });

            Task.Run(() =>
            {
                Random rand = new Random();
                while (true)
                {
                    int n = targetBlockB.Receive();
                    Console.WriteLine($">> {nameof(targetBlockB)}:{n}");

                    Thread.Sleep(rand.Next(5000));
                }
            });

        }

        public static void ExecuteParallel()
        {
            var multiplyBlock = new TransformBlock<int, int>(
                item =>
                {
                    Thread.Sleep(1000);

                    item = item * 2;
                    Console.WriteLine($"At multiplyBlock {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} Tid:{Thread.CurrentThread.ManagedThreadId}【{item}】");
                    return item;
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 3
                });


            var subtractBlock = new TransformBlock<int, int>(item =>
            {
                item = item - 2;
                Console.WriteLine($"At subtractBlock {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} Tid:{Thread.CurrentThread.ManagedThreadId}【{item}】");
                return item;
            });
            multiplyBlock.LinkTo(subtractBlock);

            Task.Run(() =>
            {
                for (int i = 1; i < 10; i++)
                {
                    multiplyBlock.Post(i);
                }
                Console.WriteLine("Post finished");
            });
        }

        public static void CreateCustomBlock()
        {
            var multiplyBlock = new TransformBlock<int, int>(item =>
            {
                return item * 2;
            });
            var addBlock = new TransformBlock<int, int>(item =>
            {
                return item + 2;
            });
            var divideBlock = new TransformBlock<int, int>(item =>
            {
                return item / 2;
            });

            var flowCompletion = new DataflowLinkOptions { PropagateCompletion = true };
            multiplyBlock.LinkTo(addBlock, flowCompletion);
            addBlock.LinkTo(divideBlock, flowCompletion);

            IPropagatorBlock<int, int> cb = DataflowBlock.Encapsulate(multiplyBlock, divideBlock);
            cb.Post(2);

        }

    }
}

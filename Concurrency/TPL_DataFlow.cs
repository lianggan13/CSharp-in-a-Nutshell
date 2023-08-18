using System.Threading.Tasks.Dataflow;
using Utilities;

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
                    await Task.Delay(2000);
                }
            });
            Task.Run(async () =>
            {
                Random rand = new Random();
                while (await sourceBlock.OutputAvailableAsync())
                {
                    //int n = sourceBlock.Receive();
                    int n = await sourceBlock.ReceiveAsync();
                    LogHelper.Info($">> Local:{n}");
                }
            });

            //Task.Run(async () =>
            //{
            //    Random rand = new Random();
            //    while (await targetBlockA.OutputAvailableAsync())
            //    {
            //        int n = await targetBlockA.ReceiveAsync();
            //        LogHelper.Warn($">> A:{n}");
            //        Thread.Sleep(rand.Next(5000));
            //    }
            //});

            //Task.Run(async () =>
            //{
            //    Random rand = new Random();
            //    while (await targetBlockB.OutputAvailableAsync())
            //    {
            //        int n = await targetBlockB.ReceiveAsync();
            //        LogHelper.Warn($">> B:{n}");
            //        Thread.Sleep(rand.Next(5000));
            //    }
            //});

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

        public static void ReadCSFiles()
        {
            var fileNamesForPath = new TransformBlock<string, IEnumerable<string>>(
              path => GetFileNames(path));

            var lines = new TransformBlock<IEnumerable<string>, IEnumerable<string>>(
              fileNames => LoadLines(fileNames));

            var words = new TransformBlock<IEnumerable<string>, IEnumerable<string>>(
              lines2 => GetWords(lines2));

            var display = new ActionBlock<IEnumerable<string>>(
              coll =>
              {
                  foreach (var s in coll)
                  {
                      Console.WriteLine(s);
                  }
              });


            fileNamesForPath.LinkTo(lines);
            lines.LinkTo(words);
            words.LinkTo(display);
        }


        public static IEnumerable<string> GetFileNames(string path)
        {
            foreach (var fileName in Directory.EnumerateFiles(path, "*.cs"))
            {
                yield return fileName;
            }
        }

        public static IEnumerable<string> LoadLines(IEnumerable<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                using (FileStream stream = File.OpenRead(fileName))
                {
                    var reader = new StreamReader(stream);
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        //WriteLine($"LoadLines {line}");
                        yield return line;
                    }
                }
            }
        }

        public static IEnumerable<string> GetWords(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                string[] words = line.Split(' ', ';', '(', ')', '{', '}', '.', ',');
                foreach (var word in words)
                {
                    if (!string.IsNullOrEmpty(word))
                        yield return word;
                }
            }
        }
    }
}

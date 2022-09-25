using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Concurrency
{
    public static class TPL_Parallel
    {
        public abstract class Matrix
        {
            public bool IsInvertible => true;
            public abstract void Invert();
        }

        public static void StopParallel(IEnumerable<Matrix> matrices)
        {
            Parallel.ForEach(matrices, (matrix, state) =>
            {
                if (!matrix.IsInvertible)
                    state.Stop();
                else
                    matrix.Invert();
            });
        }


        // Note: this is not the most efficient implementation.
        // This is just an example of using a lock to protect shared state.
        public static int ParallelSum(IEnumerable<Matrix> matrices)
        {
            object mutex = new object();
            int nonInvertibleCount = 0;
            Parallel.ForEach(source: matrices,
                body: matrix =>
                {
                    if (matrix.IsInvertible)
                    {
                        matrix.Invert();
                    }
                    else
                    {
                        lock (mutex)
                        {
                            ++nonInvertibleCount;
                        }
                    }
                });
            return nonInvertibleCount;
        }

        // Note: this is not the most efficient implementation.
        // This is just an example of using a lock to protect shared state.
        public static int ParallelSum(IEnumerable<int> values)
        {
            int result = 0;
            {
                object mutex = new object();

                Parallel.ForEach(source: values,
                    localInit: () => 0,
                    body: (item, state, localValue) => localValue + item,
                    localFinally: localValue =>
                    {
                        lock (mutex)
                            result += localValue;
                    });
            }

            {
                result = values.AsParallel().Sum();
            }

            {
                result = values.AsParallel().Aggregate(
                    seed: 0,
                    func: (sum, item) => sum + item);
            }

            return result;
        }

        public static void ParallelInvoke(Action action, CancellationToken token)
        {
            Action[] actions = Enumerable.Repeat(action, 20).ToArray();
            Parallel.Invoke(new ParallelOptions { CancellationToken = token }, actions);
        }

        public class Node
        {
            public Node Left { get; }
            public Node Right { get; }
        }

        static void DoExpensiveActionOnNode(Node node) { }

        static void Traverse(Node current)
        {
            DoExpensiveActionOnNode(current);
            if (current.Left != null)
            {
                Task.Factory.StartNew(
                    () => Traverse(current.Left),
                    CancellationToken.None,
                    TaskCreationOptions.AttachedToParent,
                    TaskScheduler.Default);
            }
            if (current.Right != null)
            {
                Task.Factory.StartNew(
                    () => Traverse(current.Right),
                    CancellationToken.None,
                    TaskCreationOptions.AttachedToParent,
                    TaskScheduler.Default);
            }
        }

        public static void DynamicParallel(Node root)
        {
            Task task = Task.Factory.StartNew(
                () => Traverse(root),
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.Default);
            task.Wait();
        }
    }
}

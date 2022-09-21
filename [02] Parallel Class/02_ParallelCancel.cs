using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace _02__Parallel_Class
{
    public class _02_ParallelCancel
    {
        public static void Show()
        {

        }

        abstract class Matrix
        {
            public abstract void Rotate(float degrees);
        }

        private void RotateMatrices(IEnumerable<Matrix> matrices, float degrees, CancellationToken token)
        {
            Parallel.ForEach(matrices,
                new ParallelOptions { CancellationToken = token },
                matrix => matrix.Rotate(degrees));
        }

        void RotateMatrices2(IEnumerable<Matrix> matrices, float degrees, CancellationToken token)
        {
            // Warning: not recommended; see below.
            Parallel.ForEach(matrices, matrix =>
            {
                matrix.Rotate(degrees);
                token.ThrowIfCancellationRequested();
            });
        }


        IEnumerable<int> MultiplyBy2(IEnumerable<int> values, CancellationToken cancellationToken)
        {
            return values.AsParallel()
                .WithCancellation(cancellationToken)
                .Select(item => item * 2);
        }
    }
}

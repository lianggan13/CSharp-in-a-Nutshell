using System.Collections.Generic;
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

        public static void StopParallelForeach(IEnumerable<Matrix> matrices)
        {
            Parallel.ForEach(matrices, (matrix, state) =>
            {
                if (!matrix.IsInvertible)
                    state.Stop();
                else
                    matrix.Invert();
            });
        }



    }
}

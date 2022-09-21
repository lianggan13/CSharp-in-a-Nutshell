using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLinq.Extension
{
    /// <summary>
    /// 本地查询扩展方法
    /// </summary>
   public static class EnumerableExtension
    {
        /// <summary>
        /// 将符串集合成对组合为另一个集合
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<string> Pair(this IEnumerable<string> source)
        {
            string fisrtHalf = null;
            foreach (string s in source)
            {
                if (string.IsNullOrEmpty(fisrtHalf))
                    fisrtHalf = s;
                else
                {
                    yield return fisrtHalf + ", " + s;
                    fisrtHalf = null;
                }
            }
        }
    }
}

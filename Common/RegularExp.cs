using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common
{
    public class RegularExp
    {
        /// <summary>
        /// 替换 示例：Replace("abcded","[ad]",""uj)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string Replace(string input, string pattern, string replacement)
        {
         return   Regex.Replace(input, pattern, replacement); 
        }

    }
}

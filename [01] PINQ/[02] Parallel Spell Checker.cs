using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _01__PINQ
{
    public   class _02__Parallel_Spell_Checker
    {
        public static void Show()
        {
            
        }

        public static void SpellChecker()
        {
            List<string> words = new List<string>();
            string[] chars = new string[] { "A", "B", "C", "D", "E", "F", "G" };
            Random random = new Random();
            words.AddRange(Enumerable.Range(0, 150000).Select(i => chars[random.Next(chars.Length)]));

            var wordLookup = new HashSet<string>(words, StringComparer.InvariantCultureIgnoreCase);
            string[] wordList = wordLookup.ToArray();

            string[] wordsToTest = Enumerable.Range(0, 100000)
                .Select(i => wordList[random.Next(0, wordList.Length)]).ToArray();
         
            wordsToTest[1234] = "Error Word1";
            wordsToTest[1235] = "Error Word2";

            var query = wordsToTest.AsParallel()    // 启用查询并行化
                                    .Select((word, index) => new IndexeWord { Word = word, Index = index })
                                    .Where(iWord => !wordLookup.Contains(iWord.Word))
                                    .OrderBy(iWord => iWord.Index);
            foreach (var mistake in query)
            {
                Console.WriteLine(mistake.Word + " - index = " + mistake.Index);
            }
        }

        public static void SpellCheckerWithThreadLocal()
        {
            List<string> words = new List<string>();
            string[] chars = new string[] { "A", "B", "C", "D", "E", "F", "G" };
            Random random = new Random();
            words.AddRange(Enumerable.Range(0, 150000).Select(i => chars[random.Next(chars.Length)]));

            var wordLookup = new HashSet<string>(words, StringComparer.InvariantCultureIgnoreCase);
            string[] wordList = wordLookup.ToArray();

            // 使用【并行化】的方式生成词汇列表  由于PLINQ会在并行线程上执行，因此必须注意保证操作的线程安全性
            var localRandom = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));  // 线程本地存储，为每个线程创建一个Random对象
            string[] wordsToTest = Enumerable.Range(0, 1000000)
                .Select(i => wordList[localRandom.Value.Next(0, wordList.Length)]).ToArray();

            wordsToTest[1234] = "Error Word1";
            wordsToTest[1235] = "Error Word2";

            var query = wordsToTest.AsParallel()    // 启用查询并行化
                                    .Select((word, index) => new IndexeWord { Word = word, Index = index })
                                    .Where(iWord => !wordLookup.Contains(iWord.Word))
                                    .OrderBy(iWord => iWord.Index);
            foreach (var mistake in query)
            {
                Console.WriteLine(mistake.Word + " - index = " + mistake.Index);
            }
        }
    }

    public class IndexeWord
    {
        public string Word { get; set; }
        public int Index { get; set; }
    }
}

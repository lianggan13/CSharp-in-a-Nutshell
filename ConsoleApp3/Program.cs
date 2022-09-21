using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<string> files =  Directory.GetFiles(@"C:\Users\Administrator\Desktop\queries", "*.linq", SearchOption.AllDirectories).Select(s=>
            {
                string path = s.Replace(".linq", ".cs");
                File.Copy(s, path, true);
                return path;
            });
            files.ToList().ForEach(f => Console.WriteLine(f));
            Console.ReadLine();
        }
    }
}

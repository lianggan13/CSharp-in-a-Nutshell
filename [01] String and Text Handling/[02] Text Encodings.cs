using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01__String_and_Text_Handling
{
    public class _02__Text_Encodings
    {
        public static void Show()
        {
            // EncodingInfo
            {
                // The easiest way to instantiate a correctly configured encoding class is to 
                // call Encoding.GetEncoding with a standard IANA name:

                Encoding utf8 = Encoding.GetEncoding("utf-8");
                Encoding chinese = Encoding.GetEncoding("GB18030");

                utf8.Dump();
                chinese.Dump();

                // The static GetEncodings method returns a list of all supported encodings:
                foreach (EncodingInfo info in Encoding.GetEncodings())
                    Console.WriteLine(info.Name);
            }
            // Text Bytes Encoding
            {
                byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes("0123456789");
                byte[] utf16Bytes = System.Text.Encoding.Unicode.GetBytes("0123456789");
                byte[] utf32Bytes = System.Text.Encoding.UTF32.GetBytes("0123456789");

                Console.WriteLine(utf8Bytes.Length);    // 10
                Console.WriteLine(utf16Bytes.Length);   // 20
                Console.WriteLine(utf32Bytes.Length);   // 40

                string original1 = System.Text.Encoding.UTF8.GetString(utf8Bytes);
                string original2 = System.Text.Encoding.Unicode.GetString(utf16Bytes);
                string original3 = System.Text.Encoding.UTF32.GetString(utf32Bytes);

                Console.WriteLine(original1);          // 0123456789
                Console.WriteLine(original2);          // 0123456789
                Console.WriteLine(original3);          // 0123456789
            }
            // UTF-16
            {
                int musicalNote = 0x1D161;

                string s = char.ConvertFromUtf32(musicalNote);
                s.Length.Dump();    // 2 (surrogate pair)

                char.ConvertToUtf32(s, 0).ToString("X").Dump();         // Consumes two chars
                char.ConvertToUtf32(s[0], s[1]).ToString("X").Dump();		// Exp
            }
        }
    }
}

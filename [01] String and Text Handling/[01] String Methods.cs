using Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01__String_and_Text_Handling
{
    /// <summary>
    /// 字符串常用方法
    /// </summary>
   public class _01__String_Methods
    {
        public  static void Show()
        {
            // String 是一个引用不可变类型

            // Contains EndsWith IndexOf IndexAny
            {
                // The simplest search methods are Contains, StartsWith, and EndsWith:
                Console.WriteLine("quick brown fox".Contains("brown"));    // True
                Console.WriteLine("quick brown fox".EndsWith("fox"));      // True

                // IndexOf returns the first position of a given character or substring:
                Console.WriteLine("abcde".IndexOf("cd"));   // 2
                Console.WriteLine("abcde".IndexOf("xx"));   // -1

                // IndexOf is overloaded to accept a startPosition StringComparison enum, which enables case-insensitive searches:
                Console.WriteLine("abcde".IndexOf("CD", StringComparison.CurrentCultureIgnoreCase));    // 2

                // LastIndexOf is like IndexOf, but works backward through the string.
                // IndexOfAny returns the first matching position of any one of a set of characters:
                Console.WriteLine("ab,cd ef".IndexOfAny(new char[] { ' ', ',' }));       // 2
                Console.WriteLine("pas5w0rd".IndexOfAny("0123456789".ToCharArray()));  // 3

                // LastIndexOfAny does the same in the reverse direction.
            }

            // Substring Insert Remove PadLeft Trim Replace
            {
                // Because String is immutable, all the methods below return a new string, leaving the original untouched.

                // Substring extracts a portion of a string:
                string left3 = "12345".Substring(0, 3);     // left3 = "123";
                string mid3 = "12345".Substring(1, 3);     // mid3 = "234";

                // If you omit the length, you get the remainder of the string:
                string end3 = "12345".Substring(2);        // end3 = "345";

                // Insert and Remove insert or remove characters at a specified position:
                string s1 = "helloworld".Insert(5, ", ");    // s1 = "hello, world"
                string s2 = s1.Remove(5, 2);                 // s2 = "helloworld";

                // PadLeft and PadRight pad a string to a given length with a specified character (or a space if unspecified):
                Console.WriteLine("12345".PadLeft(9, '*'));  // ****12345
                Console.WriteLine("12345".PadLeft(9));       //     12345

                // TrimStart, TrimEnd and Trim remove specified characters (whitespace, by default) from the string:
                Console.WriteLine("  abc \t\r\n ".Trim().Length);   // 3

                // Replace replaces all occurrences of a particular character or substring:
                Console.WriteLine("to be done".Replace(" ", " | "));  // to | be | done
                Console.WriteLine("to be done".Replace(" ", ""));  // tobedone
            }
            // Split Join Concat
            {
                // Split takes a sentence and returns an array of words (default delimiters = whitespace):
                string[] words = "The quick brown fox".Split();
                words.Dump();

                // The static Join method does the reverse of Split:
                string together = string.Join(" ", words);
                together.Dump();                                // The quick brown fox

                // The static Concat method accepts only a params string array and applies no separator.
                // This is exactly equivalent to the + operator:
                string sentence = string.Concat("The", " quick", " brown", " fox");
                string sameSentence = "The" + " quick" + " brown" + " fox";

                sameSentence.Dump();		// The quick brown fox
            }
            // Format 
            {
                // When calling String.Format, provide a composite format string followed by each of the embedded variables
                string composite = "It's {0} degrees in {1} on this {2} morning";
                string s = string.Format(composite, 35, "Perth", DateTime.Now.DayOfWeek);
                s.Dump();

                // The minimum width in a format string is useful for aligning columns.
                // If the value is negative, the data is left-aligned; otherwise, it’s right-aligned:
                composite = "Name={0,-20} Credit Limit={1,15:C}";

                Console.WriteLine(string.Format(composite, "Mary", 500));
                Console.WriteLine(string.Format(composite, "Elizabeth", 20000));

                // The equivalent without using string.Format:
                s = "Name=" + "Mary".PadRight(20) + " Credit Limit=" + 500.ToString("C").PadLeft(15);
                s.Dump();
            }
            // Compare
            {
                // String comparisons can be ordinal vs culture-sensitive; case-sensitive vs case-insensitive.

                Console.WriteLine(string.Equals("foo", "FOO", StringComparison.OrdinalIgnoreCase));   // True

                // (The following symbols may not be displayed correctly, depending on your font):
                Console.WriteLine("ṻ" == "ǖ");   // False

                // The order comparison methods return a positive number, a negative number, or zero, depending
                // on whether the first value comes after, before, or alongside the second value:
                Console.WriteLine("Boston".CompareTo("Austin"));    // 1
                Console.WriteLine("Boston".CompareTo("Boston"));    // 0
                Console.WriteLine("Boston".CompareTo("Chicago"));   // -1
                Console.WriteLine("ṻ".CompareTo("ǖ"));              // 0
                Console.WriteLine("foo".CompareTo("FOO"));          // -1

                // The following performs a case-insensitive comparison using the current culture:
                Console.WriteLine(string.Compare("foo", "FOO", true));   // 0

                // By supplying a CultureInfo object, you can plug in any alphabet:
                CultureInfo german = CultureInfo.GetCultureInfo("de-DE");
                int i = string.Compare("Müller", "Muller", false, german);
                i.Dump();	// 1
            }
            // StringBuilder
            {
                // Unlike string, StringBuilder is mutable.

                // The following is more efficient than repeatedly concatenating ordinary string types:

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < 50; i++) sb.Append(i + ",");

                // To get the final result, call ToString():
                Console.WriteLine(sb.ToString());

                sb.Remove(0, 60);       // Remove first 50 characters
                sb.Length = 10;         // Truncate to 10 characters
                sb.Replace(",", "+");   // Replace comma with +
                sb.ToString().Dump();

                sb.Length = 0;			// Clear StringBuilder
            }
        }
    }
}

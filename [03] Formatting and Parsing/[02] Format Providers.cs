using Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _03__Formatting_and_Parsing
{
  public   class _02__Format_Providers
    {
        public static void Show()
        {
            // Format Providers
            {
                // The format string provides instructions; the format provider determines how the instructions are translated:
                NumberFormatInfo f = new NumberFormatInfo();
                f.CurrencySymbol = "$$";
                Console.WriteLine(3.ToString("C", f));          // $$ 3.00

                // The default format provider is CultureInfo.CurrentCulture:
                Console.WriteLine(10.3.ToString("C", null));

                // For convenience, most types overload ToString such that you can omit a null provider:
                Console.WriteLine(10.3.ToString("C"));
                Console.WriteLine(10.3.ToString("F4"));     // (Fix to 4 D.P.)

                // Requesting a specific culture (english language in Great Britain):
                CultureInfo uk = CultureInfo.GetCultureInfo("en-GB");
                Console.WriteLine(3.ToString("C", uk));      // £3.00

                // Invariant culture:
                DateTime dt = new DateTime(2000, 1, 2);
                CultureInfo iv = CultureInfo.InvariantCulture;
                Console.WriteLine(dt.ToString(iv));            // 01/02/2000 00:00:00
                Console.WriteLine(dt.ToString("d", iv));       // 01/02/2000
            }

            // NumberFormat
            {
                // Creating a custom NumberFormatInfo:
                NumberFormatInfo f = new NumberFormatInfo();
                f.NumberGroupSeparator = " ";
                Console.WriteLine(12345.6789.ToString("N3", f));   // 12 345.679

                // Cloning:
                NumberFormatInfo f2 = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();

                // Now we can edit f2:
                f2.NumberGroupSeparator = "*";
                Console.WriteLine(12345.6789.ToString("N3", f2));   // 12 345.679
            }

            // Composite Formatting
            {
                string composite = "Credit={0:C}";
                Console.WriteLine(string.Format(composite, 500));   // Credit=$500.00
                Console.WriteLine("Credit={0:C}", 500);   // Credit=$500.00

                object someObject = DateTime.Now;
                string s = string.Format(CultureInfo.InvariantCulture, "{0}", someObject);
                s.Dump();
            }
            // Parsing with Format Providers
            {
                // There’s no standard interface for parsing through a format provider; instead use Parse/TryParse methods
                // on the target types:

                try
                {
                    int error = int.Parse("(2)");   // Exception thrown
                }
                catch (FormatException ex) { ex.Dump(); }

                int minusTwo = int.Parse("(2)", NumberStyles.Integer | NumberStyles.AllowParentheses);   // OK
                minusTwo.Dump();

                decimal fivePointTwo = decimal.Parse("£5.20", NumberStyles.Currency, CultureInfo.GetCultureInfo("en-GB"));
                fivePointTwo.Dump();
            }
            // IFormatProvider and ICustomFormatter
            {
                double n = -123.45;
                IFormatProvider fp = new WordyFormatProvider();
                Console.WriteLine(string.Format(fp, "{0:C} in words is {0:MyWordMark}", n));
            }
            // NumberberStyles
            {

                int thousand = int.Parse("3E8", NumberStyles.HexNumber);
                int minusTwo = int.Parse("(2)", NumberStyles.Integer | NumberStyles.AllowParentheses);

                double.Parse("1,000,000", NumberStyles.Any).Dump("million");
                decimal.Parse("3e6", NumberStyles.Any).Dump("3 million");
                decimal.Parse("$5.20", NumberStyles.Currency).Dump("5.2");

                NumberFormatInfo ni = new NumberFormatInfo();
                ni.CurrencySymbol = "€";
                ni.CurrencyGroupSeparator = " ";
                double.Parse("€1 000 000", NumberStyles.Currency, ni).Dump("million");
            }
            // Paring and misparsing DateTimes
            {
                // Culture-agnostic:
                string s = DateTime.Now.ToString("o");

                // ParseExact demands strict compliance with the specified format string:
                DateTime dt1 = DateTime.ParseExact(s, "o", null);

                // Parse implicitly accepts both the "o" format and the CurrentCulture format:
                DateTime dt2 = DateTime.Parse(s);
                dt1.Dump(); dt2.Dump();
            }
            // Enum Format Strings
            {
                List<string> formats = new List<string>() { "g", "f", "d", "x" };
                foreach (string f in formats)
                   ConsoleColor.Red.ToString(f).Dump("ToString (\"" + f + "\")");
            }

        }
    }

    public class WordyFormatProvider : IFormatProvider, ICustomFormatter
    {
        static readonly string[] _numberWords =
            "zero one two three four five six seven eight nine minus point".Split();

        IFormatProvider _parent;   // Allows consumers to chain format providers

        public WordyFormatProvider() : this(CultureInfo.CurrentCulture) { }
        public WordyFormatProvider(IFormatProvider parent)
        {
            _parent = parent;
        }

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter)) return this;
            return null;
        }

        public string Format(string format, object arg, IFormatProvider prov)
        {
            // If it's not our format string, defer to the parent provider:
            if (arg == null || format != "MyWordMark") 
                return string.Format(_parent, "{0:" + format + "}", arg); // "¥-123.45"
          
            StringBuilder result = new StringBuilder();
            string digitList = string.Format(CultureInfo.InvariantCulture, "{0}", arg);
            foreach (char digit in digitList)
            {
                int i = "0123456789-.".IndexOf(digit);
                if (i == -1) continue;
                if (result.Length > 0) result.Append(' ');
                result.Append(_numberWords[i]);
            }
            return result.ToString();
        }
      
    }
}

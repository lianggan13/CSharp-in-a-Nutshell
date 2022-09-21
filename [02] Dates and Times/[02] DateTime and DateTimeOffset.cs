using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02__Dates_and_Times
{
   public class _02__DateTime_and_DateTimeOffset
    {
        public static void Show()
        {
            // 结构体，如 DateTime DateTimeOffse ，不能为 null
            // 当需要 可设置为 null,如 DateTime? DateTimeOffset?
           

            // Constructing Dateime and DateTimeOffset
            {
                DateTime d1 = new DateTime(2010, 1, 30);    // Midnight, January 30 2010
                                                           //

                d1.Dump("d1");

                DateTime d2 = new DateTime(2010, 1, 30, 12, 0, 0);      // Midday, January 30 2010
                d2.Dump("d2");
                d2.Kind.Dump();

                DateTime d3 = new DateTime(2010, 1, 30, 12, 0, 0, DateTimeKind.Utc);
                d3.Dump("d3");
                d3.Kind.Dump();

                DateTimeOffset d4 = d1;     // Implicit conversion
                d4.Dump("d4");

                DateTimeOffset d5 = new DateTimeOffset(d1, TimeSpan.FromHours(-8)); // -8 hours UTC
                d5.Dump("d5");

                // See "Formatting & Parsing" for constructing a DateTime from a string

                DateTime d = new DateTime(5767, 1, 1, new System.Globalization.HebrewCalendar());   // 系统日历

                Console.WriteLine(d);    // 12/12/2006 12:00:00 AM
            }
            // Now
            {
                Console.WriteLine(DateTime.Now);
                Console.WriteLine(DateTimeOffset.Now);

                Console.WriteLine(DateTime.Today);      // No time portion

                Console.WriteLine(DateTime.UtcNow);
                Console.WriteLine(DateTimeOffset.UtcNow);
            }
            // Day of Working
            {
                DateTime dt = new DateTime(2000, 2, 3,
                            10, 20, 30);

                Console.WriteLine(dt.Year);         // 2000
                Console.WriteLine(dt.Month);        // 2
                Console.WriteLine(dt.Day);          // 3
                Console.WriteLine(dt.DayOfWeek);    // Thursday
                Console.WriteLine(dt.DayOfYear);    // 34

                Console.WriteLine(dt.Hour);         // 10
                Console.WriteLine(dt.Minute);       // 20
                Console.WriteLine(dt.Second);       // 30
                Console.WriteLine(dt.Millisecond);  // 0
                Console.WriteLine(dt.Ticks);        // 630851700300000000
                Console.WriteLine(dt.TimeOfDay);    // 10:20:30  (returns a TimeSpan)

                TimeSpan ts = TimeSpan.FromMinutes(90);
                Console.WriteLine(dt.Add(ts));         // 3/02/2000 11:50:30 AM
                Console.WriteLine(dt + ts);             // 3/02/2000 11:50:30 AM

                DateTime thisYear = new DateTime(2007, 1, 1);
                DateTime nextYear = thisYear.AddYears(1);
                TimeSpan oneYear = nextYear - thisYear;
            }
            // Formatting & Parsing
            {
                // 静态方法 Parse/TryParse 和 ParseExact/TryParseExact 执行和 ToString 相反的操作

                // The following all honor local culture settings:

                DateTime.Now.ToString().Dump("Short date followed by long time");
                DateTimeOffset.Now.ToString().Dump("Short date followed by long time (+ timezone)");

                DateTime.Now.ToShortDateString().Dump("ToShortDateString");
                DateTime.Now.ToShortTimeString().Dump("ToShortTimeString");

                DateTime.Now.ToLongDateString().Dump("ToLongDateString");
                DateTime.Now.ToLongTimeString().Dump("ToLongTimeString");

                // Culture-agnostic methods make for reliable formatting & parsing:

                DateTime dt1 = DateTime.Now;
                string cannotBeMisparsed = dt1.ToString("o");
                DateTime dt2 = DateTime.Parse(cannotBeMisparsed);
                dt2.Dump();
            }
        }
    }
}

using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02__Dates_and_Times
{
    public class _01__TimeSpan
    {
        public static void Show()
        {
            {
                // There are three ways to construct a TimeSpan:
                //  • Through one of the constructors
                //  • By calling one of the static From . . . methods
                //  • By subtracting one DateTime from another

                Console.WriteLine(new TimeSpan(2, 30, 0));     // 02:30:00
                Console.WriteLine(TimeSpan.FromHours(2.5));    // 02:30:00
                Console.WriteLine(TimeSpan.FromHours(-2.5));   // -02:30:00
                Console.WriteLine(DateTime.MaxValue - DateTime.MinValue);

                // TimeSpan overloads the < and > operators, as well as the + and - operators:
      
                (TimeSpan.FromHours(2) + TimeSpan.FromMinutes(30)).Dump("2.5 hours");
                (TimeSpan.FromDays(10) - TimeSpan.FromSeconds(1)).Dump("One second short of 10 days");
            }
            {
                TimeSpan nearlyTenDays = TimeSpan.FromDays(10) - TimeSpan.FromSeconds(1);

                // The following properties are all of type int:

                Console.WriteLine(nearlyTenDays.Days);          // 9
                Console.WriteLine(nearlyTenDays.Hours);         // 23
                Console.WriteLine(nearlyTenDays.Minutes);       // 59
                Console.WriteLine(nearlyTenDays.Seconds);       // 59
                Console.WriteLine(nearlyTenDays.Milliseconds);  // 0

                // In contrast, the Total... properties return values of type double describing the entire time span:

                Console.WriteLine();
                Console.WriteLine(nearlyTenDays.TotalDays);          // 9.99998842592593
                Console.WriteLine(nearlyTenDays.TotalHours);         // 239.999722222222
                Console.WriteLine(nearlyTenDays.TotalMinutes);       // 14399.9833333333
                Console.WriteLine(nearlyTenDays.TotalSeconds);       // 863999
                Console.WriteLine(nearlyTenDays.TotalMilliseconds);  // 863999000

            }
        }
    }
}

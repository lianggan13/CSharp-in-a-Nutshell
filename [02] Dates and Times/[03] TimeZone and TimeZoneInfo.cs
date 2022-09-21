using Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02__Dates_and_Times
{
    public class _03__TimeZone_and_TimeZoneInfo
    {
        public static void Show()
        {
            // DateTime and Time Zones
            {
                // When you compare two DateTime instances, only their ticks values are compared; their DateTimeKinds are ignored:

                DateTime dt1 = new DateTime(2000, 1, 1, 10, 20, 30, DateTimeKind.Local);
                DateTime dt2 = new DateTime(2000, 1, 1, 10, 20, 30, DateTimeKind.Utc);
                Console.WriteLine(dt1 == dt2);          // True

                DateTime local = DateTime.Now;
                DateTime utc = local.ToUniversalTime();
                Console.WriteLine(local == utc);        // False

                // You can construct a DateTime that differs from another only in Kind with the static DateTime.SpecifyKind method:

                DateTime d = new DateTime(2000, 12, 12);  // Unspecified
                DateTime utc2 = DateTime.SpecifyKind(d, DateTimeKind.Utc);
                Console.WriteLine(utc2);            // 12/12/2000 12:00:00 AM
            }

            // DateTimeOffset and Time Zones
            {
                // Comparisons look only at the (UTC) DateTime; the Offset is used primarily for formatting.

                DateTimeOffset local = DateTimeOffset.Now;
                DateTimeOffset utc = local.ToUniversalTime();   // 只会影响 UTC 偏移量

                Console.WriteLine(local.Offset);   // -06:00:00 (in Central America)
                Console.WriteLine(utc.Offset);     // 00:00:00

                Console.WriteLine(local == utc);                 // True

                //To include the Offset in the comparison, you must use the EqualsExact method:

                Console.WriteLine(local.EqualsExact(utc));      // False
            }

            // TimeZone
            {
                // The static TimeZone.CurrentTimeZone method returns a TimeZone object based on the current local settings.
                TimeZone zone = TimeZone.CurrentTimeZone;
                zone.StandardName.Dump("StandardName");
                zone.DaylightName.Dump("DaylightName");

                // The IsDaylightSavingTime and GetUtcOffset methods work as follows:
                DateTime dt1 = new DateTime(2008, 1, 1);
                DateTime dt2 = new DateTime(2008, 6, 1);
                zone.IsDaylightSavingTime(dt1).Dump("IsDaylightSavingTime (January)");
                zone.IsDaylightSavingTime(dt2).Dump("IsDaylightSavingTime (June)");
                zone.GetUtcOffset(dt1).Dump("UTC Offset (January)");
                zone.GetUtcOffset(dt2).Dump("UTC Offset (June)");

                // The GetDaylightChanges method returns specific daylight saving information for a given year:
                DaylightTime day = zone.GetDaylightChanges(2010); // 返回指定年份的夏令时信息
                if (day == null) return;
                day.Start.Dump("day.Start");
                day.End.Dump("day.End");
                day.Delta.Dump("day.Delta");
            }

            //TODO: 夏令时 的概念 暂时不了解
            // Daylight Saving and DateTime
            {
                // The IsDaylightSavingTime tells you whether a given local DateTime is subject to daylight saving.
                Console.WriteLine(DateTime.Now.IsDaylightSavingTime());     // True or False

                // UTC times always return false:
                Console.WriteLine(DateTime.UtcNow.IsDaylightSavingTime());  // Always False

                // The end of daylight saving presents a particular complication for algorithms that use local time.
                // The comments on the right show the results of running this in a daylight-saving-enabled zone:
                DaylightTime changes = TimeZone.CurrentTimeZone.GetDaylightChanges(2010);
                TimeSpan halfDelta = new TimeSpan(changes.Delta.Ticks / 2);
                DateTime utc1 = changes.End.ToUniversalTime() - halfDelta;
                DateTime utc2 = utc1 - changes.Delta;

                // Converting these variables to local times demonstrates why you should use UTC and not local time
                // if your code relies on time moving forward:
                DateTime loc1 = utc1.ToLocalTime();  // (Pacific Standard Time)
                DateTime loc2 = utc2.ToLocalTime();
                Console.WriteLine(loc1);            // 2/11/2010 1:30:00 AM
                Console.WriteLine(loc2);            // 2/11/2010 1:30:00 AM
                Console.WriteLine(loc1 == loc2);    // True

                // Despite loc1 and loc2 reporting as equal, they are different inside:
                Console.WriteLine(loc1.ToString("o"));  // 2010-11-02T02:30:00.0000000-08:00
                Console.WriteLine(loc2.ToString("o"));  // 2010-11-02T02:30:00.0000000-07:00

                // The extra bit ensures correct round-tripping between local and UTC times:
                Console.WriteLine(loc1.ToUniversalTime() == utc1);   // True
                Console.WriteLine(loc2.ToUniversalTime() == utc2);   // True
            }

          
        }
    }
}

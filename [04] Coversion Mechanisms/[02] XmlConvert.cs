using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace _04__Coversion_Mechanisms
{
  public  class _02__XmlConvert
    {
        public static void Show()
        {
            // XmlConvert honors XML formatting rules:

            string s = XmlConvert.ToString(true);
            s.Dump();                                   // true (rather than True)
            XmlConvert.ToBoolean(s).Dump();

            DateTime dt = DateTime.Now;
            XmlConvert.ToString(dt, XmlDateTimeSerializationMode.Local).Dump("local");
            XmlConvert.ToString(dt, XmlDateTimeSerializationMode.Utc).Dump("Utc");
            XmlConvert.ToString(dt, XmlDateTimeSerializationMode.RoundtripKind).Dump("RoundtripKind");

            XmlConvert.ToString(DateTimeOffset.Now).Dump("DateTimeOffset");
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace _04__Coversion_Mechanisms
{
   public class _04__Type_Converter
    {
        public static void Show()
        {
            TypeConverter cc = TypeDescriptor.GetConverter(typeof(Color));

            Color beige = (Color)cc.ConvertFromString("Beige");
            Color purple = (Color)cc.ConvertFromString("#800080");
            Color window = (Color)cc.ConvertFromString("Window");

            beige.Dump();
            purple.Dump();
            window.Dump();
        }
    }
}

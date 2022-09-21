using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class ObjectExtention
    {
        private static Dictionary<Type, Action<object>> m_objTypes;
         static ObjectExtention()
        {
            m_objTypes = new Dictionary<Type, Action<object>>();
            m_objTypes.Add(typeof(int), obj => Console.Write(obj.ToString()));
            m_objTypes.Add(typeof(string), obj => Console.Write(obj.ToString()));
            m_objTypes.Add(typeof(int[]), obj => (obj as int[]).ToList().ForEach(o => o.Dump()));
            m_objTypes.Add(typeof(string[]), obj => (obj as string[]).ToList().ForEach(o => Console.Write(o)));
            m_objTypes.Add(typeof(Encoding), obj => Console.Write((obj as Encoding).EncodingName));
            m_objTypes.Add(typeof(TimeSpan), obj =>Console.Write( obj.ToString()) );
            m_objTypes.Add(typeof(TimeSpan?), obj => Console.Write( (obj as TimeSpan?)) );
            m_objTypes.Add(typeof(bool), obj => Console.Write(obj.ToString()));
            m_objTypes.Add(typeof(ParallelQuery<int>), obj => (obj as  ParallelQuery<int>).ForAll(Console.Write));
        } 
        public static void Dump(this object obj, string data = null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if(data !=null ) Console.WriteLine(data);
            Console.ForegroundColor = ConsoleColor.White;
            Action<object> action = null;
            m_objTypes.TryGetValue(obj.GetType(), out action);
            action?.Invoke(obj);
            Console.WriteLine();
        }
        public static void Dump(this int obj, string data = null)
        {
            if (data != null) Console.WriteLine(data);
            Console.Write(obj.ToString());
            Console.WriteLine();
        }
        //public static void Dump(this TimeSpan obj, string data = null)
        //{
        //    if (data != null) Console.WriteLine(data);
        //    Console.Write(obj.ToString());
        //    Console.WriteLine();
        //}


        public static string ToStringEx(this object value)
        {
            if (value == null) return "<null>";

            StringBuilder sb = new StringBuilder();
            if (value is IList)
                sb.Append("List of " + ((IList)value).Count + "Items: ");

            Type closedIGrouping = value.GetType().GetInterfaces()
                 .Where(t => t.IsGenericType &&
                             t.GetGenericTypeDefinition() == typeof(IGrouping<,>))
                 .FirstOrDefault();
            if (closedIGrouping != null)
            {
                PropertyInfo pi = closedIGrouping.GetProperty("Key");
                object key = pi.GetValue(value, null);
                sb.Append("Group with key " + key + ": ");
            }


            if (value is IEnumerable)
                foreach (object e in (IEnumerable)value)
                    sb.Append(ToStringEx(e) + " ");

            if (sb.Length == 0) sb.Append(value.ToString());

            return "\r\n" + sb.ToString();
        }
    }
}

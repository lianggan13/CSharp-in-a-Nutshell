using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02__ICollection_and_IList
{
    class Program
    {
        static void Main(string[] args)
        {
            // ICollection ICollection<T>
            // IList IList< T >
            // IDictionary IDictionary<T>
            // 泛型接口均没有 实现 非泛型接口，所以集合类通常实现两种版本的接口(泛型与非泛型)

            //例如：若 IList<T> 实现 IList, 当集合类型转换为 IList< T > 接口时，会同时
            //含有Add(T)和Add(object)接口成员，此破坏了静态类型的安全性，因为可将
            //任意类型作为Add方法参数
        }
    }
}

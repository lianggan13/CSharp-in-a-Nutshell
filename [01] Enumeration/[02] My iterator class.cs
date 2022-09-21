using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01__Enumeration
{
    public class _02__My_iterator_class
    {
        public static void Show()
        {
            {
                // Collection
                foreach (int i in new MyCollection(new int[] { 1, 2, 3, 4, 5, 6 }))
                    Console.WriteLine(i);

                // Generic Collection
                foreach (var name in new MyGenCollection<string>(new string[] { "Tom", "Bob", "Jack", "LG" }))
                    Console.WriteLine(name);

                // Iterator method 按需供给
                foreach (int i in MyIterator.GetSomeIntegers())
                    Console.WriteLine(i);
            }
            {
                // MyCurrent MyMoveNext MyReset
                foreach (int i in new MyIntList(new int[] { 1, 2, 3, 4, 5, 6 }))
                    Console.WriteLine(i);

                // T MyCurrent MyMoveNext MyReset
                foreach (string name in new MyGenList<string>(new string[] { "Tom", "Bob", "Jack", "LG" }))
                    Console.WriteLine(name);
            }
        }
    }
    public class MyCollection : IEnumerable
    {
        public int[] data;
        public MyCollection(int[] data)
        {
            this.data = data;
        }
        public IEnumerator GetEnumerator()
        {
            foreach (int item in data)
            {
                yield return item;
            }
        }
    }
    public class MyGenCollection<T> : IEnumerable<T>
    {
        public T[] data;
        public MyGenCollection(T[] data)
        {
            this.data = data;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T item in data)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    public class MyIterator
    {
        public static IEnumerable<int> GetSomeIntegers()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }
    }

    /// <summary>
    /// 列表集合类
    /// </summary>
    public class MyIntList : IEnumerable
    {
        public int[] data;
        public MyIntList(int[] data)
        {
            this.data = data;
        }
        public IEnumerator GetEnumerator() => new MyIntEnumerator(this);
        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns></returns>
        internal class MyIntEnumerator : IEnumerator
        {
            private MyIntList m_MyIntList;
            private int m_CurrentIndex = -1;

            public object Current
            {
                get
                {
                    if (m_CurrentIndex == -1)
                        throw new InvalidOperationException("Enumeration not started!");
                    if (m_CurrentIndex == m_MyIntList.data.Length)
                        throw new InvalidOperationException("Past end of list!");
                    return m_MyIntList.data[m_CurrentIndex];
                }
            }

            public MyIntEnumerator(MyIntList intList)
            {
                m_MyIntList = intList;
            }
            public bool MoveNext()
            {
                if (m_CurrentIndex >= m_MyIntList.data.Length - 1) return false;
                return ++m_CurrentIndex < m_MyIntList.data.Length;
            }

            public void Reset() => m_CurrentIndex = -1;
        }
    }

    /// <summary>
    /// 列表集合类(泛型)
    /// </summary>
    public class MyGenList<T> : IEnumerable<T>
    {
        public T[] data;
        public MyGenList(T[] data)
        {
            this.data = data;
        }
        public IEnumerator<T> GetEnumerator() => new MyGenEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new MyGenEnumerator(this);

        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns></returns>
        internal class MyGenEnumerator: IEnumerator<T>
        {
            private MyGenList<T> m_MyGenList;
            private int m_CurrentIndex = -1;
            
            public MyGenEnumerator(MyGenList<T> genList)
            {
                this.m_MyGenList = genList;
            }

            public T Current =>  m_MyGenList.data[m_CurrentIndex];

            object IEnumerator.Current => m_MyGenList.data[m_CurrentIndex];


            public bool MoveNext()
            {
                if (m_CurrentIndex >= m_MyGenList.data.Length - 1) return false;
                return ++m_CurrentIndex < m_MyGenList.data.Length;
            }

            public void Reset() => m_CurrentIndex = -1;
            public void Dispose()
            {
                this.m_MyGenList = null;
            }

        }
    }
}

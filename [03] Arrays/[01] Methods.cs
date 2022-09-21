using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _03__Arrays
{

    public class _01__Methods
    {
        public static void Show()
        {
            // StructuralEqualityComparer
            {
                object[] a1 = { "string", 123, true };
                object[] a2 = { "string", 123, true };

                Console.WriteLine(a1 == a2);                          // False
                Console.WriteLine(a1.Equals(a2));                    // False

                IStructuralEquatable se1 = a1;

                Console.WriteLine(se1.Equals(a2, StructuralComparisons.StructuralEqualityComparer));   // True
            }
            // Clone
            {
                StringBuilder[] builders = new StringBuilder[5];
                builders[0] = new StringBuilder("builder1");
                builders[1] = new StringBuilder("builder2");
                builders[2] = new StringBuilder("builder3");

                StringBuilder[] builders2 = builders;
                StringBuilder[] shallowClone = (StringBuilder[])builders.Clone();

                builders.Dump();
                builders2.Dump();

                (builders[0] == builders2[0]).Dump("Comparing first element of each array");
            }
            // Create
            {
                // Via C#'s native syntax:

                int[] myArray = { 1, 2, 3 };
                int first = myArray[0];
                int last = myArray[myArray.Length - 1];

                // Using GetValue/SetValue:

                // Create a string array 2 elements in length:
                Array a = Array.CreateInstance(typeof(string), 2);
                a.SetValue("hi", 0);                             //  → a[0] = "hi";
                a.SetValue("there", 1);                          //  → a[1] = "there";
                a.Dump();
                string s = (string)a.GetValue(0);               //  → s = a[0];
                s.Dump();

                // We can also cast to a C# array as follows:
                string[] cSharpArray = (string[])a;
                string s2 = cSharpArray[0];
                s2.Dump();
            }
            // Search
            {
                string[] names = { "Rodney", "Jack", "Jill", "Jane" };

                Array.Find(names, n => n.Contains("a")).Dump(); // Returns first matching element
                Array.FindAll(names, n => n.Contains("a")).Dump();  // Returns all matching elements

                // Equivalent in LINQ:

                names.FirstOrDefault(n => n.Contains("a")).Dump();
                names.Where(n => n.Contains("a")).Dump();
            }
            // Sort
            {
                int[] numbers = { 3, 2, 1 };
                Array.Sort(numbers);
                numbers.Dump("Simple sort");

                numbers = new[] { 3, 2, 1 };
                string[] words = { "three", "two", "one" };
                Array.Sort(numbers, words);
                new { numbers, words }.Dump("Parallel sort");

                // Sort such that odd numbers come first:
                 numbers =new[] { 1, 2, 3, 4, 5 };
                Array.Sort(numbers, (x, y) => x % 2 == y % 2 ? 0 : x % 2 == 1 ? -1 : 1);
                numbers.Dump();
            }
            // ConvertAll
            {
                float[] reals = { 1.3f, 1.5f, 1.8f };
                int[] wholes = Array.ConvertAll(reals, r => Convert.ToInt32(r));

                wholes.Dump();
            }
        }
    }
}

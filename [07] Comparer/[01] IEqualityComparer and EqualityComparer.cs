using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _07__Comparer
{
   public  class _01__IEqualityComparer_and_EqualityComparer
    {
       public static void Show()
        {
            Customer c1 = new Customer("Bloggs", "Joe");
            Customer c2 = new Customer("Bloggs", "Joe");

            Console.WriteLine(c1 == c2);               // False
            Console.WriteLine(c1.Equals(c2));         // False

            var d = new Dictionary<Customer, string>();
            d[c1] = "Joe";
            Console.WriteLine(d.ContainsKey(c2));         // False

            var eqComparer = new LastFirstEqComparer();
            d = new Dictionary<Customer, string>(eqComparer);
            d[c1] = "Joe";
            Console.WriteLine(d.ContainsKey(c2));         // True
        }
    }

    public class Customer
    {
        public string LastName;
        public string FirstName;

        public Customer(string last, string first)
        {
            LastName = last;
            FirstName = first;
        }
    }

    public class LastFirstEqComparer : EqualityComparer<Customer>
    {
        public override bool Equals(Customer x, Customer y)
        => x.LastName == y.LastName && x.FirstName == y.FirstName;

        public override int GetHashCode(Customer obj)
        => (obj.LastName + ";" + obj.FirstName).GetHashCode();
    }
}

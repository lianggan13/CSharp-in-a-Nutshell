using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MyLinq.Model
{
   public  class Customer
    {      
        public int ID { get; set; }
        public string Name { get; set; }
        public Collection<Purchase> Purchases { get; set; }
    }
}

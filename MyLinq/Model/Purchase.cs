using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace MyLinq.Model
{
  public  class Purchase
    {
        public int ID { get; set; }
        public int CustomerID { get; set; }
        public string Desccription { get; set; }
        public decimal Price { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyLinq.Model
{
   public partial class Product
    {
        public int ID;
        public string Description;
        public bool Discontinued;
        public DateTime LastSale;
    }

    public partial class Product
    {
        public static Expression<Func<Product,bool>> IsSelling()
        {
            return p => !p.Discontinued && p.LastSale > DateTime.Now.AddDays(-30);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PsbankTest2.Models
{
    public class ProductVM
    {
        public List<Product> Products { get; set; }
        public Product Product { get; set; }
        public IEnumerable<OrderList> OrderList { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
        public int TotalCost { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task2.Models
{
    public class Products
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? SupplierId { get; set; }
        public int CategoryId { get; set; }
        public Categories Category { get; set; }
        public int QuantityPerUnit { get; set; }
        public int UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public int UnitsOnOrder { get;set; }
        public int ReorderLevel { get; set; }
        public int Discontinued { get; set; }
        public int LastUserId { get; set; }
        public DateTime LastDateUpdated { get; set; }
        public ICollection<OrderDetails> Orders { get; set; }

    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace MarketStore.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderLines = new HashSet<OrderLine>();
            ProductImages = new HashSet<ProductImage>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public byte? Quantitiy { get; set; }
        public long? CategoryId { get; set; }
        public long StoreId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Store Store { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
    }
}

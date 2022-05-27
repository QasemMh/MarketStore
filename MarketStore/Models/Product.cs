using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public byte? Quantitiy { get; set; }
        public long? CategoryId { get; set; }
        [Required]
        public long StoreId { get; set; }
        public decimal? Cost { get; set; }

        public virtual Category Category { get; set; }
        public virtual Store Store { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }



        [NotMapped]
        public List<IFormFile> FormFiles { get; set; }
    }
}

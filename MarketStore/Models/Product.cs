using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

#nullable disable

namespace MarketStore.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderLines = new HashSet<OrderLine>();
            ProductDiscounts = new HashSet<ProductDiscount>();
            ProductImages = new HashSet<ProductImage>();
        }

        public DateTime? CreateAt { get; set; }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public byte? Quantitiy { get; set; }
        public long? CategoryId { get; set; }
        public long StoreId { get; set; }
        public decimal? Cost { get; set; }
        public DateTime? ExpireDate { get; set; }

        [JsonIgnore]
        public virtual Category Category { get; set; }
        [JsonIgnore]
        public virtual Store Store { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderLine> OrderLines { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductDiscount> ProductDiscounts { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductImage> ProductImages { get; set; }


        [JsonIgnore]
        [NotMapped]
        public List<IFormFile> FormFiles { get; set; }
    }
}

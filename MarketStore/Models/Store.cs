using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace MarketStore.Models
{
    public partial class Store
    {
        public Store()
        {
            Products = new HashSet<Product>();
        }

        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public long? CategoryId { get; set; }

        public virtual StoreCategory StoreCategory { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}

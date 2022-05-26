using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace MarketStore.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public long Id { get; set; }
        // [Required]
        public string Name { get; set; }

        public string Image { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}

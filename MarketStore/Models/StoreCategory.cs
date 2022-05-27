using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace MarketStore.Models
{
    public partial class StoreCategory
    {
        public StoreCategory()
        {
            Stores = new HashSet<Store>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public virtual ICollection<Store> Stores { get; set; }

        [NotMapped]
        public IFormFile FormFile { get; set; }
    }
}

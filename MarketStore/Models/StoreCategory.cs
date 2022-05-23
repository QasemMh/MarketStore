using System;
using System.Collections.Generic;

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
    }
}

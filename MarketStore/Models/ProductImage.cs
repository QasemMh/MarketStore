using System;
using System.Collections.Generic;

#nullable disable

namespace MarketStore.Models
{
    public partial class ProductImage
    {
        public long Id { get; set; }
        public string MainImage { get; set; }
        public string SubImage { get; set; }
        public long ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}

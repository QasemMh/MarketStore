using System;
using System.Collections.Generic;

#nullable disable

namespace MarketStore.Models
{
    public partial class ProductDiscount
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public DateTime? DiscountDate { get; set; }
        public decimal? DiscountPrice { get; set; }
        public bool? IsValid { get; set; }

        public virtual Product Product { get; set; }
    }
}

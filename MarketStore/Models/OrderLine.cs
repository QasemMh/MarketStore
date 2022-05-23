using System;
using System.Collections.Generic;

#nullable disable

namespace MarketStore.Models
{
    public partial class OrderLine
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long ProductId { get; set; }
        public byte Quantity { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}

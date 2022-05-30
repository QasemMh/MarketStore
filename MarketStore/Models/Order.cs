using System;
using System.Collections.Generic;

#nullable disable

namespace MarketStore.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderLines = new HashSet<OrderLine>();
        }

        public long Id { get; set; }
        public long? CustomerId { get; set; }
        public DateTime? OrderDate { get; set; }
        public bool? Status { get; set; }

       

        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace MarketStore.Models
{
    public partial class CreditCard
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryYear { get; set; }
        public string ExpiryMonth { get; set; }
        public string SecurityCode { get; set; }
        public decimal? Balance { get; set; }
        public long? CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
    }
}

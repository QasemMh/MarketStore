using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace MarketStore.Models
{
    public partial class CreditCard
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string ExpiryYear { get; set; }
        [Required]
        public string ExpiryMonth { get; set; }
        [Required]
        public string SecurityCode { get; set; }
        public decimal? Balance { get; set; }
        public long? CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
    }
}

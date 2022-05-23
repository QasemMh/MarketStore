using System;
using System.Collections.Generic;

#nullable disable

namespace MarketStore.Models
{
    public partial class Address
    {
        public long Id { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }

        public virtual Customer Customer { get; set; }
    }
}

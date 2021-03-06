using System;
using System.Collections.Generic;

#nullable disable

namespace MarketStore.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
        }

        public long Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public long? AddressId { get; set; }

        public virtual Address Address { get; set; }
        public virtual CreditCard CreditCard { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace MarketStore.Models
{
    public partial class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string HashPassword { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public long? CustomerId { get; set; }
        public long RoleId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Role Role { get; set; }


        [NotMapped]
        public string Password { get; set; }
    }
}

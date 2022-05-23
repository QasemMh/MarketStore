using System;
using System.Collections.Generic;

#nullable disable

namespace MarketStore.Models
{
    public partial class UserMessage
    {
        public long Id { get; set; }
        public string Subject { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}

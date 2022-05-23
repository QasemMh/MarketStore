using System;
using System.Collections.Generic;

#nullable disable

namespace MarketStore.Models
{
    public partial class Review
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public string Review1 { get; set; }
        public string Image { get; set; }
    }
}

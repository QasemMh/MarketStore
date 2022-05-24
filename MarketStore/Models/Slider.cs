using System;
using System.Collections.Generic;

#nullable disable

namespace MarketStore.Models
{
    public partial class Slider
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}

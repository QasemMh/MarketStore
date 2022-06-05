using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace MarketStore.Models
{
    public partial class ProductImage
    {
        public long Id { get; set; }
        public string Image { get; set; }
        public long ProductId { get; set; }


        [JsonIgnore]

        public virtual Product Product { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace MarketStore.Models
{
    public partial class CategorySection
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }

        [NotMapped]
        public IFormFile FormFile { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace MarketStore.Models
{
    public partial class Review
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]

        public string JobTitle { get; set; }
        [Required]

        public string Review1 { get; set; }
        public string Image { get; set; }


        [NotMapped]
        public IFormFile FormFile { get; set; }

    }
}

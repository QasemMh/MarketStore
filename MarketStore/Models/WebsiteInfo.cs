using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace MarketStore.Models
{
    public partial class WebsiteInfo
    {
        public long Id { get; set; }
        [Required]
        public string LogoName { get; set; }


        public string IogoImage { get; set; }


        [Required]
        public string Location { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string BrefDescription { get; set; }

        [NotMapped]
        public IFormFile FormFile { get; set; }

    }
}

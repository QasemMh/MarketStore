using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace MarketStore.Models
{
    public partial class Home
    {
        public long Id { get; set; }
        [Required]
        public string LogoName { get; set; }
        public string IogoImage { get; set; }
        public string Location { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        public string BrefDescription { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace MarketStore.Models
{
    public partial class WebsiteInfo
    {
        public long Id { get; set; }
        public string LogoName { get; set; }
        public string IogoImage { get; set; }
        public string Location { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string BrefDescription { get; set; }
    }
}

using MarketStore.Models;
using System.Collections.Generic;

namespace MarketStore.ViewModels
{
    public class HomeViewModel
    {
        public List<Slider> Sliders { get; set; }
        public List<StoreCategory> CategoryList { get; set; }
        public List<Product> Products { get; set; }
    }
}

using MarketStore.Models;
using System.Collections.Generic;

namespace MarketStore.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public List<ProductImage> ProductImages { get; set; }

    }
}

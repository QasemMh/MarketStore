using MarketStore.constants;
using MarketStore.Models;
using System.Collections.Generic;

namespace MarketStore.ViewModels
{
    public class ShopViewModel
    {
        public PaginatedList<Product> Products { get; set; }
        public List<StoreCategory> StoresCategories { get; set; }
        public List<Category> ProductsCategories { get; set; }
        public List<Store> Stores { get; set; }

    }
}

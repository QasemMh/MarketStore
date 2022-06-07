using MarketStore.Models;

namespace MarketStore.ViewModels
{
    public class ProductSalesViewModel
    {
        public Product Product { get; set; }
        public decimal Total { get; set; }
        public int Sales { get; set; }
    }
}

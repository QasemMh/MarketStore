using MarketStore.Models;

namespace MarketStore.constants
{
    public class CartItem
    {
        public Product Product { get; set; }

        public string ProductImage { get; set; }
        public int Quantity { get; set; }
    }
}

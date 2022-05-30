using MarketStore.Models;
using System.Collections.Generic;

namespace MarketStore.ViewModels
{
    public class OrderViewModel
    {
        public List<OrderLine> OrderDetails { get; set; }
        public Order Order { get; set; }
        public Address CustomerAddress { get; set; }
    }
}

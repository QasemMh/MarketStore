using MarketStore.constants;
using MarketStore.Models;
using System.Collections.Generic;

namespace MarketStore.ViewModels
{
    public class CheckoutViewModel
    {
        public Address Address { get; set; }
        public CreditCard CreditCard { get; set; }
        public List<CartItem> Cart { get; set; }
    }
}

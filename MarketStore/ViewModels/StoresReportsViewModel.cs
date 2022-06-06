using MarketStore.Models;
using System;

namespace MarketStore.ViewModels
{
    public class StoresReportsViewModel
    {
        public string StoreName { get; set; }
        public int ProductCount { get; set; }
        public string StoreCategory { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

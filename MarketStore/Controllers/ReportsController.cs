using MarketStore.constants;
using MarketStore.Models;
using MarketStore.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarketStore.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportsController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task<IActionResult> StoreReport(DateTime? fromDate, DateTime? toDate)
        {

            ViewBag.fromDate = fromDate.HasValue ? fromDate.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.toDate = toDate.HasValue ? toDate.Value.ToString("yyyy-MM-dd") : "";



            var stores = _context.Stores
                .Include(p => p.Products)
                .Include(s => s.StoreCategory)
                .Select(item => new StoresReportsViewModel
                {
                    StoreName = item.Name,
                    StoreCategory = item.StoreCategory.Name,
                    ProductCount = item.Products.Count,
                    CreatedDate = item.CreateAt.Value

                });


            if (fromDate.HasValue && !toDate.HasValue)
            {
                stores = stores.Where(p =>
                p.CreatedDate.Year == fromDate.Value.Year
                && p.CreatedDate.Month == fromDate.Value.Month
                );
            }
            if (fromDate.HasValue && toDate.HasValue)
            {
                stores = stores.Where(p => p.CreatedDate >= fromDate.Value &&
                p.CreatedDate <= toDate.Value);
            }

            return View(await stores.ToListAsync());
        }



    }
}

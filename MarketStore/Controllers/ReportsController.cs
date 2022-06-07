using MarketStore.constants;
using MarketStore.Models;
using MarketStore.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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


        public async Task<IActionResult> StoreFinancialReport(DateTime? fromDate, DateTime? toDate)
        {
            DateTime today = DateTime.Today;

            if (fromDate.HasValue)
            {
                ViewBag.fromDate = fromDate.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
                fromDate = startOfMonth;
                ViewBag.fromDate = startOfMonth.ToString("yyyy-MM-dd");
            }

            if (toDate.HasValue)
            {
                ViewBag.toDate = toDate.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                DateTime endOfMonth = new DateTime(today.Year, today.Month,
                    DateTime.DaysInMonth(today.Year, today.Month));
                toDate = endOfMonth;
                ViewBag.toDate = endOfMonth.ToString("yyyy-MM-dd");
            }


            var store = await _context.Stores.FirstOrDefaultAsync();
            var productsIds = await _context.Products.Where(p => p.StoreId == store.Id)
                .Select(p => p.Id).ToListAsync();

            var productsSalesQuery = _context.OrderLines.Include(o => o.Order)
                .Where(o => productsIds.Contains(o.ProductId) &&
                o.Order.OrderDate >= fromDate &&
                o.Order.OrderDate <= toDate
                )
                .GroupBy(p => p.ProductId)
                .Select(p => new GroupByViewModel
                {
                    Key = p.Key,
                    Result = p.Sum(p => p.Quantity),
                });

            var productsSales = await productsSalesQuery.ToListAsync();
            var products = await _context.Products.Where(p => productsIds.Contains(p.Id)).ToListAsync();

            var viewModel = new List<ProductSalesViewModel>();

            foreach (var item in productsSales)
            {
                var product = products.Find(p => p.Id == item.Key);
                if (product != null)
                {
                    viewModel.Add(new ProductSalesViewModel
                    {
                        Product = product,
                        Total = product.Price * item.Result,
                        Sales = item.Result
                    });
                }
            }


            return View(viewModel);
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

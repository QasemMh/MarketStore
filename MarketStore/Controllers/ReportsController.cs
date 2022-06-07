using MarketStore.constants;
using MarketStore.Models;
using MarketStore.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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


        public async Task<IActionResult> StoreFinancialReport(long? storeId, DateTime? fromDate, DateTime? toDate)
        {
            ViewBag.Stores = new SelectList(await _context.Stores.ToListAsync(), "Id", "Name", storeId);

            if (storeId == null) return View(new List<ProductSalesViewModel>());

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


            var store = await _context.Stores
                .Include(s => s.StoreCategory)
                .FirstOrDefaultAsync(s => s.Id == storeId);

            if (store == null) return NotFound();




            var productsIds = await _context.Products.Where(p => p.StoreId == store.Id)
                .Select(p => p.Id).ToListAsync();



            //viewBag for store data
            ViewBag.Store = store;
            ViewBag.StoreProductCount = productsIds.Count();



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


        public async Task<IActionResult> ExpiredProduct(long? storeId)
        {
            ViewBag.Stores = new SelectList(await _context.Stores.ToListAsync(), "Id", "Name", storeId);

            var expiredProduct = _context.Products
                .Include(p => p.Store)
                .Where(p => p.ExpireDate < DateTime.Now.Date);

            if (storeId != null)
            {
                var store = await _context.Stores.FirstOrDefaultAsync(s => s.Id == storeId);
                ViewBag.Store = store;
                expiredProduct = expiredProduct.Where(p => p.StoreId == storeId);
                var modelFilterd = await expiredProduct.ToListAsync();
                ViewBag.StoreProductCount = expiredProduct.Count();

                return View(modelFilterd);
            }

            var model = await expiredProduct.ToListAsync();
            ViewBag.TotalExpired = expiredProduct.Count();
            return View(model);
        }

 

    }
}

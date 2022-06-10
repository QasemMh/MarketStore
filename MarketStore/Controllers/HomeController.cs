using MarketStore.constants;
using MarketStore.Models;
using MarketStore.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MarketStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {

            ViewBag.WebsiteInfo = await _context.WebsiteInfos.FirstOrDefaultAsync();
            if (ViewBag.WebsiteInfo == null)
                ViewBag.WebsiteInfo = new WebsiteInfo
                {
                    LogoName = "Logo",
                    Email = "Email@gmail.com",
                    Location = "location",
                    Phone = "079000000",
                    BrefDescription = "BrefDescription"
                };

            var viewModel = new HomeViewModel
            {
                Sliders = await _context.Sliders.ToListAsync(),
                CategoryList = await _context.StoreCategories.Take(10).ToListAsync(),
                Products = await _context.Products
                .Include(p => p.Store).Include(p => p.ProductImages)
                .Where(p => p.Quantitiy > 0 && p.ExpireDate.Value.Date > DateTime.Now.Date)
                .Take(16).ToListAsync()
            };




            return View(viewModel);
        }


        public async Task<IActionResult> Shop(string currentFilter, string searchString,
            long? storeCategoryId, long? productCategoryId, long? storeId,
            decimal? minPrice, decimal? maxPrice, int? pageNumber)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["StoreCategoryId"] = storeCategoryId;
            ViewData["ProductCategoryId"] = productCategoryId;
            ViewData["StoreId"] = storeId;
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;


            var products = _context.Products
                .Include(p => p.Store).Include(p => p.ProductImages)
                .Where(p => p.Quantitiy > 0 && p.ExpireDate.Value.Date > DateTime.Now.Date)
                .AsQueryable();

            if (storeId != null)
            {
                products = products.Where(p => p.Store.Id == storeId);
            }
            if (storeCategoryId != null)
            {
                products = products.Where(p => p.Store.CategoryId == storeCategoryId);
            }
            if (productCategoryId != null)
            {
                products = products.Where(p => p.CategoryId == productCategoryId);
            }
            //price
            if (minPrice.HasValue && maxPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
            }
            else if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice);
            }
            else if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Store.Name.StartsWith(searchString) ||
                s.Name.Contains(searchString));
            }

            var storesCategories = await _context.StoreCategories.ToListAsync();
            var productCategories = await _context.Categories.ToListAsync();
            var stores = await _context.Stores.ToListAsync();


            int pageSize = 12;
            var viewModel = new ShopViewModel
            {
                Products = await PaginatedList<Product>.CreateAsync(products.AsNoTracking(),
                pageNumber ?? 1, pageSize),
                StoresCategories = storesCategories,
                ProductsCategories = productCategories,
                Stores = stores
            };



            //website data
            ViewBag.WebsiteInfo = await _context.WebsiteInfos.FirstOrDefaultAsync();
            if (ViewBag.WebsiteInfo == null)
                ViewBag.WebsiteInfo = new WebsiteInfo
                {
                    LogoName = "Logo",
                    Email = "Email@gmail.com",
                    Location = "location",
                    Phone = "079000000",
                    BrefDescription = "BrefDescription"
                };

            return View(viewModel);
        }
        public async Task<IActionResult> Product(long? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(s => s.Store).Include(s => s.Category)
                .FirstOrDefaultAsync(p => p.Id == id &&
                p.Quantitiy > 0 &&
                p.ExpireDate.Value.Date > DateTime.Now.Date);

            if (product == null) return NotFound();

            var images = await _context.ProductImages
                .Where(p => p.ProductId == product.Id).ToListAsync();



            //website data
            ViewBag.WebsiteInfo = await _context.WebsiteInfos.FirstOrDefaultAsync();
            if (ViewBag.WebsiteInfo == null)
                ViewBag.WebsiteInfo = new WebsiteInfo
                {
                    LogoName = "Logo",
                    Email = "Email@gmail.com",
                    Location = "location",
                    Phone = "079000000",
                    BrefDescription = "BrefDescription"
                };

            return View(new ProductViewModel { Product = product, ProductImages = images });
        }



        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            //website data

           var model = await _context.WebsiteInfos.FirstOrDefaultAsync();
            if (model == null)
                model = new WebsiteInfo
                {
                    LogoName = "Logo",
                    Email = "Email@gmail.com",
                    Location = "location",
                    Phone = "079000000",
                    BrefDescription = "BrefDescription"
                };

            //website data
            ViewBag.WebsiteInfo = model;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromForm] string name, [FromForm] string email,
            [FromForm] string subject, [FromForm] string message)
        {

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email)
                && !string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(subject))
            {
                var userMessage = new UserMessage
                {
                    Name = name,
                    Message = message,
                    Email = email,
                    Subject = subject
                };

                _context.UserMessages.Add(userMessage);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return BadRequest();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

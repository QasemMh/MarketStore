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
            var viewModel = new HomeViewModel
            {
                Sliders = await _context.Sliders.ToListAsync(),
                CategoryList = await _context.StoreCategories.Take(10).ToListAsync(),
                Products = await _context.Products.Include(p => p.Store).Include(p => p.ProductImages)
                .Take(16).ToListAsync()
            };




            return View(viewModel);
        }



        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            return View(await _context.WebsiteInfos.FirstOrDefaultAsync());
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

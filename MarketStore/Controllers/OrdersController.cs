using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MarketStore.Models;
using MarketStore.constants;
using MarketStore.ViewModels;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;

namespace MarketStore.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public OrdersController(ModelContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _webHostEnvironment = hostEnvironment;
        }

        // GET: Orders
        public async Task<IActionResult> Index(string currentFilter,
            string searchString, int? pageNumber, long? userId,
            DateTime? fromDate, DateTime? toDate)
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
            ViewData["userId"] = userId;
            ViewBag.fromDate = fromDate.HasValue ? fromDate.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.toDate = toDate.HasValue ? toDate.Value.ToString("yyyy-MM-dd") : "";


            var customers = await _context.Users
                .Include(c => c.Customer)
                .Where(u => u.CustomerId != null).Select(u => new
                {
                    Id = u.Customer.Id,
                    Username = $"{u.Username} - {u.Customer.FirstName} {u.Customer.LastName}"
                }).ToListAsync();

            ViewBag.Users = new SelectList(customers, "Id", "Username", userId);

            var orders = _context.Orders
                .Include(c => c.Customer)
                .AsQueryable();

            if (userId.HasValue)
            {
                orders = orders.Where(p => p.CustomerId == userId);
            }
            if (fromDate.HasValue && !toDate.HasValue)
            {
                orders = orders.Where(p =>
                p.OrderDate.Value.Year == fromDate.Value.Year
                && p.OrderDate.Value.Month == fromDate.Value.Month
                && p.OrderDate.Value.Day == fromDate.Value.Day
                );
            }
            if (fromDate.HasValue && toDate.HasValue)
            {
                orders = orders.Where(p => p.OrderDate.Value >= fromDate.Value &&
                p.OrderDate.Value <= toDate.Value);
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(s =>
                  s.Customer.User.Username.StartsWith(searchString)
                );
            }

            int pageSize = 20;
            return View(await PaginatedList<Order>.CreateAsync(orders.AsNoTracking(),
                pageNumber ?? 1, pageSize));
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer).ThenInclude(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var orderDetails = await _context.OrderLines
                .Include(p => p.Product)
                .Where(o => o.OrderId == order.Id).ToListAsync();

            var customerAddress = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == order.Customer.AddressId);

            var viewModel = new OrderViewModel
            {
                Order = order,
                OrderDetails = orderDetails,
                CustomerAddress = customerAddress
            };

            return View(viewModel);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(long id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }


        [HttpPost]
        public async Task<IActionResult> SendEmail([FromForm] string email
            , [FromForm] string orderId)
        {
            email = email.ToLower();
            long id;
            if (!long.TryParse(orderId, out id))
            {
                return BadRequest();
            }
            var order = await _context.Orders
                .Include(o => o.Customer).ThenInclude(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return BadRequest();
            }
            if (order.Customer == null)
            {
                return BadRequest();
            }
            if (order.Customer.User.Email == null)
            {
                return BadRequest();
            }

            var orderDetails = await _context.OrderLines
                .Include(p => p.Product)
                .Where(o => o.OrderId == order.Id).ToListAsync();

            var customerAddress = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == order.Customer.AddressId);

            var viewModel = new OrderViewModel
            {
                Order = order,
                OrderDetails = orderDetails,
                CustomerAddress = customerAddress
            };

            var result = await EmailService.SendAsync(viewModel, _webHostEnvironment.ContentRootPath);
            if (result)
                return Ok();

            return BadRequest();
        }
    }
}

using MarketStore.constants;
using MarketStore.Models;
using MarketStore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketStore.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ModelContext _context;

        public CustomerController(ModelContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            //reomve this
            //   HttpContext.Session.SetString("userId", "21");
            // HttpContext.Session.SetString("username", "qasem1");


            if (HttpContext.Session.GetString("userId") == null)
                return RedirectToAction("Index", "Home");

            long userId = Convert.ToInt64(HttpContext.Session.GetString("userId"));
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var role = _context.Roles.FirstOrDefaultAsync(r => r.Id == user.RoleId).Result.Name == "Customer";
            if (!role)
                return RedirectToAction("Index", "Home");


            return View();
        }

        public async Task<IActionResult> Address()
        {
            long userId = Convert.ToInt64(HttpContext.Session.GetString("userId"));
            var user = await _context.Users
                .Include(u => u.Customer).FirstOrDefaultAsync(u => u.Id == userId);

            if (user.Customer.AddressId.HasValue)
            {
                var addressDb = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == user.Customer.AddressId);

                ViewBag.addressId = user.Customer.AddressId;
                return View(addressDb);
            }
            return View(new Address());
        }
        [HttpPost]
        public async Task<IActionResult> Address(long? addressId, Address address)
        {
            long userId = Convert.ToInt64(HttpContext.Session.GetString("userId"));
            var user = await _context.Users
                .Include(u => u.Customer).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {
                if (addressId == null)
                {
                    user.Customer.Address = address;
                    _context.Update(user);
                }
                else
                {
                    _context.Addresses.Update(address);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Address));
            }

            ModelState.AddModelError("Address1", "Error, Please Try Again");
            return View(address);
        }



        public async Task<IActionResult> Payment()
        {
            long userId = Convert.ToInt64(HttpContext.Session.GetString("userId"));
            var user = await _context.Users
                .Include(u => u.Customer).FirstOrDefaultAsync(u => u.Id == userId);

            var userVisa = await _context.CreditCards
                .FirstOrDefaultAsync(a => a.CustomerId == user.Customer.Id);

            ViewBag.userId = user.Customer.Id;

            return View(userVisa);
        }
        [HttpPost]
        public async Task<IActionResult> Payment(long? userId,
            [Bind("Id,Name,CardNumber,ExpiryYear,ExpiryMonth,SecurityCode")] CreditCard creditCard)
        {
            if (userId == null) return NotFound();

            if (ModelState.IsValid)
            {
                var userVisa = await _context.CreditCards.FirstOrDefaultAsync(a => a.CustomerId == userId);
                if (userVisa == null)
                {
                    creditCard.Balance = 500;
                    creditCard.CustomerId = userId;
                    _context.CreditCards.Add(creditCard);
                }
                else _context.CreditCards.Update(creditCard);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Payment));
            }

            ModelState.AddModelError("SecurityCode", "Error, Please Try Again");
            return View(creditCard);
        }


        public async Task<IActionResult> Orders(int? pageNumber,
            DateTime? fromDate, DateTime? toDate)
        {


            ViewBag.fromDate = fromDate.HasValue ? fromDate.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.toDate = toDate.HasValue ? toDate.Value.ToString("yyyy-MM-dd") : "";

            long userId = Convert.ToInt64(HttpContext.Session.GetString("userId"));
            var user = await _context.Users
                .Include(u => u.Customer).FirstOrDefaultAsync(u => u.Id == userId);

            var orders = (IQueryable<Order>)_context.Orders
                .Where(c => c.CustomerId == user.CustomerId).OrderByDescending(o => o.Id);

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


            int pageSize = 10;
            var model = await PaginatedList<Order>.CreateAsync(orders.AsNoTracking(),
                pageNumber ?? 1, pageSize);


            return View(model);
        }

        public async Task<IActionResult> OrderDetails(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var orderDetails = await _context.OrderLines
                .Include(p => p.Product)
                .Where(o => o.OrderId == order.Id).ToListAsync();


            var viewModel = new OrderViewModel
            {
                Order = order,
                OrderDetails = orderDetails,
            };

            return View(viewModel);
        }
        public async Task<IActionResult> Profile()
        {
            ViewBag.passwordError = TempData["passwordError"] ?? "";


            var userId = Convert.ToInt64(HttpContext.Session.GetString("userId"));
            var user = await _context.Users.Include(r => r.Role).Include(c => c.Customer)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(User user)
        {
            var userId = Convert.ToInt64(HttpContext.Session.GetString("userId"));
            var userDb = await _context.Users.Include(c => c.Customer).FirstOrDefaultAsync(u => u.Id == userId);

            AccountService accountService = new AccountService();
            //is valid data
            if (user.Email != null)
                if (!accountService.IsValidEmail(user.Email))
                {
                    ViewData["Edit"] = "Edit";
                    ModelState.AddModelError("Email", "Invalid Email Rule");
                    return View(user);
                }

            if (user.Phone != null)
                if (!accountService.IsValidPhone(user.Phone))
                {
                    ViewData["Edit"] = "Edit";
                    ModelState.AddModelError("Phone", "Invalid Phone Rule");
                    return View(user);
                }

            //is data not already exists
            if (user.Email != null && userDb.Email != user.Email)
                if (_context.Users
                   .FirstOrDefaultAsync(u => u.Email == user.Email.ToLower()).Result != null)
                {
                    ViewData["Edit"] = "Edit";
                    ModelState.AddModelError("Email", "Email Address already exists");
                    return View(user);
                }

            if (user.Phone != null && userDb.Phone != user.Phone)
                if (_context.Users
               .FirstOrDefaultAsync(u => u.Phone == user.Phone.ToLower()).Result != null)
                {
                    ViewData["Edit"] = "Edit";
                    ModelState.AddModelError("Phone", "Phone already exists");
                    return View(user);
                }

            userDb.Email = user.Email;
            userDb.Phone = user.Phone;

            userDb.Customer.FirstName = user.Customer.FirstName;
            if (user.Customer.MiddleName != null)
                userDb.Customer.MiddleName = user.Customer.MiddleName;
            userDb.Customer.LastName = user.Customer.LastName;


            _context.Users.Update(userDb);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Profile));
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword,
            string confirmPassword)
        {

            var userId = Convert.ToInt64(HttpContext.Session.GetString("userId"));
            var userDb = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (confirmPassword != newPassword)
            {
                TempData["passwordError"] = "password and confirm password not match";
                return RedirectToAction(nameof(Profile));
            }

            bool isVerified = SecurePasswordHasher.Verify(oldPassword, userDb.HashPassword);

            if (!isVerified)
            {
                TempData["passwordError"] = "your current password is invalid, try again";
                return RedirectToAction(nameof(Profile));
            }

            if (newPassword.Length < 6)
            {
                TempData["passwordError"] = "new password sholud be at least 6 chars";
                return RedirectToAction(nameof(Profile));
            }

            userDb.HashPassword = SecurePasswordHasher.Hash(newPassword);
            _context.Users.Update(userDb);
            await _context.SaveChangesAsync();


            TempData["passwordError"] = "Your Password Has been Changed Successfully!";
            return RedirectToAction(nameof(Profile));
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userId");
            HttpContext.Session.Remove("username");
            return RedirectToAction("Index", "Home");
        }

    }
}

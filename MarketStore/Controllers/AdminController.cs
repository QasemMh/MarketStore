using MarketStore.constants;
using MarketStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MarketStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly ModelContext _context;

        public AdminController(ModelContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {

            //reomve this
            HttpContext.Session.SetString("userId", "1");

            if (HttpContext.Session.GetString("userId") == null)
              return RedirectToAction("Index", "Home");

             
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userId");
            HttpContext.Session.Remove("username");
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Profile(string edit)
        {
            ViewBag.passwordError = TempData["passwordError"] ?? "";

            if (edit != null) ViewData["Edit"] = "Edit";

            var userId = Convert.ToInt64(HttpContext.Session.GetString("userId"));
            var user = await _context.Users.Include(r => r.Role).FirstOrDefaultAsync(u => u.Id == userId);

            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(User user)
        {
            var userId = Convert.ToInt64(HttpContext.Session.GetString("userId"));
            var userDb = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

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
                    ModelState.AddModelError("Username", "Email Address already exists");
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

            _context.Users.Update(userDb);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Profile));
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword) {

            var userId = Convert.ToInt64(HttpContext.Session.GetString("userId"));
            var userDb = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (confirmPassword != newPassword) {
                TempData["passwordError"] = "password and confirm password not match";
                return RedirectToAction(nameof(Profile));
            }

            bool isVerified = SecurePasswordHasher.Verify(oldPassword, userDb.HashPassword);

            if (!isVerified)
            {
                TempData["passwordError"] = "your current password is invalid, try again";
                return RedirectToAction(nameof(Profile));
            }

            if(newPassword.Length<6)
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









    }
}

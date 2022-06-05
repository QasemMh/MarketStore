using MarketStore.constants;
using MarketStore.Models;
using MarketStore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MarketStore.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly ModelContext _context;

        public AuthenticationController(ModelContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("userId") != null)
                return RedirectToAction("Index", "Home");


            return View(new User());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {

            if (ModelState.IsValid)
            {
                if (_context.Users
                   .FirstOrDefaultAsync(u => u.Username == user.Username.ToLower()).Result == null)
                {
                    ModelState.AddModelError("Username", "Invalid data");
                    return View(user);
                }

                var userDb = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == user.Username.ToLower());

                bool isVerified = SecurePasswordHasher.Verify(user.Password, userDb.HashPassword);

                if (!isVerified)
                {
                    ModelState.AddModelError("Username", "Invalid data");
                    return View(user);
                }


                HttpContext.Session.SetString("username", userDb.Username);
                HttpContext.Session.SetString("userId", userDb.Id.ToString());

                string roleName = _context.Roles
                    .FirstOrDefault(r => r.Id == userDb.RoleId).Name.ToLower();

                HttpContext.Session.SetString("roleName", roleName);

                switch (roleName)
                {
                    case "admin":
                        return RedirectToAction("Index", "Admin"); //admin dashboard
                    case "customer":
                        return RedirectToAction("Index", "Home");
                }




            }

            return View(user);
        }
        public IActionResult Register()
        {

            if (HttpContext.Session.GetString("userId") != null)
                return RedirectToAction("Index", "Home");


            return View(new RegisterUserViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                AccountService accountService = new AccountService();



                if (!accountService.ValidationRegister(viewModel))
                {
                    ModelState.AddModelError("Username", "Invalid data");
                    return View(viewModel);
                }

                if (_context.Users
                    .FirstOrDefaultAsync(u => u.Username == viewModel.Username.ToLower()).Result != null)
                {
                    ModelState.AddModelError("Username", "Username already exists");
                    return View(viewModel);
                }

                if (_context.Users
                   .FirstOrDefaultAsync(u => u.Email == viewModel.Email.ToLower()).Result != null)
                {
                    ModelState.AddModelError("Username", "Email Address already exists");
                    return View(viewModel);
                }



                Customer customer = new Customer();
                User user = new User();

                customer.FirstName = viewModel.FirstName.ToLower();
                customer.LastName = viewModel.LastName.ToLower();
                _context.Add(customer);
                await _context.SaveChangesAsync();


                user.CustomerId = customer.Id;

                user.Username = viewModel.Username.ToLower();
                user.Email = viewModel.Email.ToLower();

                //get role id for customer
                var customerRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Name == "Customer");
                //add role to customer
                user.RoleId = customerRole.Id;


                //hash the password
                user.HashPassword = SecurePasswordHasher.Hash(viewModel.Password);

                _context.Add(user);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("username", user.Username);
                HttpContext.Session.SetString("userId", user.Id.ToString());
                HttpContext.Session.SetString("roleName", "customer");

                return RedirectToAction("Index", "Home");
            }
            return View(viewModel);
        }
    }
}

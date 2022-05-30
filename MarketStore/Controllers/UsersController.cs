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

namespace MarketStore.Controllers
{
    public class UsersController : Controller
    {
        private readonly ModelContext _context;

        public UsersController(ModelContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index(string currentFilter,
            string searchString, int? pageNumber, long? roleId)
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
            ViewData["RoleId"] = roleId;
            ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "Id", "Name", roleId);

            var users = _context.Users
                .Include(u => u.Customer).Include(c => c.Role)
                .AsQueryable();

            if (roleId.HasValue)
            {
                users = users.Where(p => p.RoleId == roleId);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s =>
                s.Username.StartsWith(searchString) ||
                s.Email.Contains(searchString)
                 );
            }
            int pageSize = 20;
            return View(await PaginatedList<User>.CreateAsync(users.AsNoTracking(),
                pageNumber ?? 1, pageSize));
        }


        // GET: Users/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(r => r.Role)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            if (user.CustomerId != null)
            {
                user = await _context.Users
                .Include(r => r.Role)
                .Include(c => c.Customer)
                .ThenInclude(a => a.Address)
                .FirstOrDefaultAsync(m => m.Id == id);
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                user.Username = user.Username.ToLower();
                AccountService account = new AccountService();
                if (!account.IsValidUsername(user.Username))
                {
                    ModelState.AddModelError("Username", "Invalid username rule");
                    return View(user);
                }
                if (_context.Users.FirstOrDefaultAsync(u => u.Username == user.Username).Result != null)
                {
                    ModelState.AddModelError("Username", "Username already exists");
                    return View(user);
                }
                if (!user.Password.Equals(user.ConfirmPassword))
                {
                    ModelState.AddModelError("Password", "Password and ConfirmPassword NOT equals");
                    return View(user);
                }
                if (user.Password.Length < 6)
                {
                    ModelState.AddModelError("Password", "Password must be 6 digit at least");
                    return View(user);
                }

                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                User newUser = new User
                {
                    Username = user.Username,
                    HashPassword = SecurePasswordHasher.Hash(user.Password),
                    RoleId = adminRole.Id
                };

                _context.Add(newUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("Password", "Admin Not Added, try again");
            return View(user);

        }


        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}

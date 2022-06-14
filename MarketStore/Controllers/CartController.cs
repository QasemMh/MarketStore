using MarketStore.constants;
using MarketStore.Models;
using MarketStore.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
    public class CartController : Controller
    {

        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CartController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task<IActionResult> Index()
        {

            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

            if (cart != null && cart.Count > 0)
            {
                ViewBag.cart = cart;
                ViewBag.total = cart.Sum(item => item.Product.Price * item.Quantity);
            }
            ViewBag.IsLogin = false;
            if (HttpContext.Session.GetString("userId") != null)
            {
                var userId = Convert.ToInt64(HttpContext.Session.GetString("userId"));
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                bool isCustomer = _context.Roles.FirstOrDefaultAsync(r=>r.Id==user.RoleId).Result.Name == "Customer";
                ViewBag.IsLogin = isCustomer;

            }


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

            return View();
        }
        public async Task<IActionResult> Checkout()
        {


            if (HttpContext.Session.GetString("userId") == null)
                return RedirectToAction("Login", "Authentication");

            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            if (cart == null || cart.Count == 0)
                return RedirectToAction("Index", "Cart");

            var userId = Convert.ToInt64(HttpContext.Session.GetString("userId"));
            var user = await _context.Users.Include(u => u.Customer).FirstOrDefaultAsync(u => u.Id == userId);

            var creditCards = await _context.CreditCards.FirstOrDefaultAsync(c => c.CustomerId == user.Customer.Id);
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == user.Customer.AddressId);


            ViewBag.HasVisa = creditCards != null ? true : false;
            ViewBag.HasAddress = address != null ? true : false;


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


            return View(new CheckoutViewModel { CreditCard = creditCards, Address = address, Cart = cart });
        }
        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> CheckoutPost()
        {
            try
            {
                if (HttpContext.Session.GetString("userId") == null)
                    return BadRequest();

                var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
                if (cart == null || cart.Count == 0)
                    return BadRequest();

                if (!CheckCart(cart).Result) return BadRequest();

                var userId = Convert.ToInt64(HttpContext.Session.GetString("userId"));
                var user = await _context.Users.Include(u => u.Customer).FirstOrDefaultAsync(u => u.Id == userId);
                var creditCard = await _context.CreditCards.FirstOrDefaultAsync(c => c.CustomerId == user.CustomerId);

                var cartTotalPrice = cart.Select(c => c.Product.Price * c.Quantity).Sum();
                if (cartTotalPrice > creditCard.Balance)
                    return BadRequest();

                //update balance
                creditCard.Balance -= cartTotalPrice;
                _context.CreditCards.Update(creditCard);

                //update product quantity
                await UpdateStock(cart);


                var order = new Order { CustomerId = user.CustomerId };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();


                var items = new List<OrderLine>();
                foreach (var item in cart)
                {
                    items.Add(new OrderLine
                    {
                        OrderId = order.Id,
                        Quantity = (byte)item.Quantity,
                        ProductId = item.Product.Id
                    });
                }

                _context.OrderLines.AddRange(items);
                await _context.SaveChangesAsync();

                SessionHelper.RemoveSession(HttpContext.Session, "cart");
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        [HttpPost]
        public async Task<IActionResult> Add([FromForm] long id)
        {

            if (SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart") == null)
            {
                var productImage = await _context.ProductImages.FirstOrDefaultAsync(p => p.ProductId == id);
                List<CartItem> cart = new List<CartItem>();
                cart.Add(new CartItem
                {
                    Product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id),
                    ProductImage = productImage.Image,
                    Quantity = 1
                });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    var productImage = await _context.ProductImages.FirstOrDefaultAsync(p => p.ProductId == id);
                    cart.Add(new CartItem
                    {
                        Product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id),
                        ProductImage = productImage.Image,
                        Quantity = 1
                    });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }




            return Ok();
        }


        [HttpPost]
        public IActionResult Remove([FromForm] long id)
        {
            List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            int index = isExist(id);
            if (index != -1)
            {
                cart.RemoveAt(index);
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        public IActionResult Increment([FromForm] long id)
        {
            int index = isExist(id);
            if (index != -1)
            {
                List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

                var productQuantity = _context.Products
                    .FirstOrDefault(p => p.Id == cart[index].Product.Id).Quantitiy;

                if (cart[index].Quantity < productQuantity)
                    cart[index].Quantity++;

                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost]
        public IActionResult Decrement([FromForm] long id)
        {
            int index = isExist(id);
            if (index != -1)
            {
                List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

                if (cart[index].Quantity > 1)
                    cart[index].Quantity--;

                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation()
        {

            if (HttpContext.Session.GetString("userId") == null)
                return NotFound();

            var userId = Convert.ToInt64(HttpContext.Session.GetString("userId"));

            var user = await _context.Users
                .Include(c => c.Customer).FirstOrDefaultAsync(u => u.Id == userId);

            var customerAddress = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == user.Customer.AddressId);

            var order = await _context.Orders
                .Include(c => c.Customer).ThenInclude(u => u.User)
                .Where(o => o.CustomerId == user.CustomerId)
                .OrderByDescending(s => s.Id).FirstOrDefaultAsync();

            var orderDetails = await _context.OrderLines
                              .Include(p => p.Product)
                              .Where(o => o.OrderId == order.Id).ToListAsync();
            var viewModel = new OrderViewModel
            {
                Order = order,
                CustomerAddress = customerAddress,
                OrderDetails = orderDetails
            };

            //send email
            await EmailService.SendAsync(viewModel, _webHostEnvironment.ContentRootPath);

            //
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



        private int isExist(long id)
        {
            List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.Id == id)
                {
                    return i;
                }
            }
            return -1;
        }


        private async Task UpdateStock(List<CartItem> cart)
        {
            var productIds = cart.Select(item => item.Product.Id).ToList();
            var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

            foreach (var item in cart)
            {
                var product = products.Find(p => p.Id == item.Product.Id);
                product.Quantitiy -= (byte)item.Quantity;
                products[products.FindIndex(p => p.Id == item.Product.Id)] = product;
            }
            _context.Products.UpdateRange(products);
        }

        private async Task<bool> CheckCart(List<CartItem> cart)
        {
            var productIds = cart.Select(item => item.Product.Id).ToList();
            var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

            foreach (var item in cart)
            {
                var product = products.Find(p => p.Id == item.Product.Id);
                if (product == null)
                    return false;
                if (item.Quantity > product.Quantitiy)
                    return false;
            }


            return true;
        }




    }
}

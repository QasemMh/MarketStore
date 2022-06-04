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
        public CartController(ModelContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                ViewBag.cart = cart;
                ViewBag.total = cart.Sum(item => item.Product.Price * item.Quantity);
            }
            ViewBag.IsLogin = false;
            if (HttpContext.Session.GetString("userId") != null)
                ViewBag.IsLogin = true;

            return View();
        }
        public async Task<IActionResult> Checkout()
        {
            if (HttpContext.Session.GetString("userId") == null)
                return RedirectToAction("Login", "Authentication");

            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            if (cart == null)
                return RedirectToAction("Index", "Home");

            var userId = Convert.ToInt64(HttpContext.Session.GetString("userId"));
            var user = await _context.Users.Include(u => u.Customer).FirstOrDefaultAsync(u => u.Id == userId);

            var creditCards = await _context.CreditCards.FirstOrDefaultAsync(c => c.CustomerId == user.Customer.Id);
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == user.Customer.AddressId);

            ViewBag.cart = cart;
            ViewBag.total = cart.Sum(item => item.Product.Price * item.Quantity);
            ViewBag.HasVisa = creditCards != null ? true : false;
            ViewBag.HasAddress = address != null ? true : false;

            return View(new CheckoutViewModel { CreditCard = creditCards, Address = address });
        }

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> CheckoutPost()
        {

            //check for product quantity
            //check for balance
            //Edit product quantity
            //remove cart when end


            try
            {
                if (HttpContext.Session.GetString("userId") == null)
                    return BadRequest();

                var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
                if (cart == null)
                    return BadRequest();

                var userId = Convert.ToInt64(HttpContext.Session.GetString("userId"));
                var user = await _context.Users.Include(u => u.Customer).FirstOrDefaultAsync(u => u.Id == userId);


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
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return Ok();
        }

        [HttpPost]
        public IActionResult Increment([FromForm] long id)
        {
            int index = isExist(id);
            if (index != -1)
            {
                List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
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
                cart[index].Quantity--;
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                return Ok();
            }
            return BadRequest();
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




    }
}

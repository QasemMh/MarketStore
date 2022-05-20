using Microsoft.AspNetCore.Mvc;

namespace MarketStore.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

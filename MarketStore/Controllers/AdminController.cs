using Microsoft.AspNetCore.Mvc;

namespace MarketStore.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

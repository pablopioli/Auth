using Microsoft.AspNetCore.Mvc;

namespace TokenServer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace TourManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

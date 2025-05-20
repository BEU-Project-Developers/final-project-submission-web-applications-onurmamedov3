using Microsoft.AspNetCore.Mvc;

namespace TourManagementSystem.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

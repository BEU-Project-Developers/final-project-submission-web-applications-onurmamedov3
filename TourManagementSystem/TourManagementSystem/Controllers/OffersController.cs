using Microsoft.AspNetCore.Mvc;

namespace TourManagementSystem.Controllers
{
    public class OffersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

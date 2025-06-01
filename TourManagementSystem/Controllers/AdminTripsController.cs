using Microsoft.AspNetCore.Mvc;

namespace TourManagementSystem.Controllers
{
    public class AdminTripsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

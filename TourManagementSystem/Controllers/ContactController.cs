using Microsoft.AspNetCore.Mvc;

namespace TourManagementSystem.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

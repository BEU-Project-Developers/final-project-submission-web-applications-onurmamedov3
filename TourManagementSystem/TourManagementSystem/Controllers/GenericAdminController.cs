using Microsoft.AspNetCore.Mvc;

// If in an Area:
// namespace TourManagementSystem.Areas.Admin.Controllers
// {
//    [Area("Admin")]
//    public class GenericAdminController : Controller

namespace TourManagementSystem.Controllers // Or your appropriate namespace
{
    public class GenericAdminController : Controller
    {
        // This action will serve the single page for all CRUD operations
        // GET: /GenericAdmin/Manage or /Admin/GenericAdmin/Manage (if in Area)
        public IActionResult Manage()
        {
            // We don't pass any specific entity model here initially,
            // as the entity type will be selected on the page.
            ViewData["Title"] = "Generic Admin Management";
            return View();
        }
    }
}
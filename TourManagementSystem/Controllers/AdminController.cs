// File: TourManagementSystem/Controllers/AdminController.cs (MVC Controller)
// using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

public class AdminController : Controller
{
    // [Authorize(Roles = "Admin")] // Secure this page
    public IActionResult Manage() // This action will return your GenericAdmin.cshtml
    {
        return View("GenericAdmin"); // Assuming your view is named GenericAdmin.cshtml
    }
}
// File: TourManagementSystem/Controllers/ContactController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks; // For async
using TourManagementSystem.Models; // For ContactMessageViewModel
using TourManagementSystem.Services; // For IContactService

namespace TourManagementSystem.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactService _contactService; // Inject the service

        public ContactController(IContactService contactService) // Constructor injection
        {
            _contactService = contactService;
        }

        // GET: /Contact or /Contact/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View(new ContactMessageViewModel()); // Pass an empty ViewModel to the view
        }

        // POST: /Contact or /Contact/Index
        [HttpPost]
        [ValidateAntiForgeryToken] // Good for security
        public async Task<IActionResult> Index(ContactMessageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var (success, message) = await _contactService.SaveContactMessageAsync(model);
                if (success)
                {
                    TempData["SuccessMessage"] = message; // "Your message has been sent successfully!"
                    // It's good practice to redirect after a successful POST to prevent double submission
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Add the error from the service to display to the user
                    ModelState.AddModelError(string.Empty, message);
                }
            }
            // If ModelState is not valid, or if saving failed, redisplay the form with errors
            return View(model);
        }
    }
}
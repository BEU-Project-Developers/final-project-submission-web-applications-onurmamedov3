// File: TourManagementSystem/Controllers/AccountController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;      // Required for async Tasks
using TourManagementSystem.Models;   // For ViewModels (LoginViewModel, RegisterViewModel)
using TourManagementSystem.Services; // Required for IUserService

namespace TourManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService; // Declare a private field to hold the service

        // Constructor: IUserService is now "injected" here by the application
        public AccountController(IUserService userService)
        {
            _userService = userService; // Assign the injected service to our field
        }

        // GET: /Account/Register
        // This method just shows the empty registration form
        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new RegisterViewModel());
        }

        // POST: /Account/Register
        // This method is called when the user submits the registration form
        [HttpPost]
        [ValidateAntiForgeryToken] // Important for security
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            // Check if the data entered by the user (e.g., email format, required fields) is valid
            // This uses the validation attributes you put in RegisterViewModel.cs
            if (ModelState.IsValid)
            {
                // Call our UserService to actually try and create the user in the database
                var (success, errorMessage, createdUser) = await _userService.RegisterUserAsync(model);

                if (success && createdUser != null)
                {
                    // If registration was successful
                    TempData["SuccessMessage"] = "Registration successful! Please log in.";
                    return RedirectToAction(nameof(Login)); // Send the user to the Login page
                }
                else
                {
                    // If registration failed (e.g., email already exists, or database error)
                    // Add the error message to be displayed on the form
                    ModelState.AddModelError(string.Empty, errorMessage ?? "An unknown error occurred during registration.");
                }
            }
            // If ModelState is not valid (e.g., user didn't fill a required field),
            // or if registration failed, show the form again with error messages.
            return View(model);
        }

        // GET: /Account/Login
        // This method just shows the empty login form
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        // This method is called when the user submits the login form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Call our UserService to check if the email and password are correct
                var user = await _userService.ValidateUserAsync(model.Email, model.Password);
                if (user != null)
                {
                    // USER'S EMAIL AND PASSWORD ARE CORRECT!
                    // For a real application, you would now sign the user in (e.g., create a session cookie).
                    // This part is more complex and involves setting up Authentication in Program.cs.
                    // For now, we'll just show a success message and redirect.

                    TempData["SuccessMessage"] = $"Welcome back, {user.FullName}!";
                    return RedirectToLocal(returnUrl); // Redirect to where they were going, or Home
                }
                else
                {
                    // If login failed (user not found or password incorrect)
                    ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your email and password.");
                }
            }
            // If ModelState is not valid or login failed, show the form again with error messages.
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() // Made async in case you add await HttpContext.SignOutAsync later
        {
            // For a real application with login sessions, you would sign the user out here.
            // Example: await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Message"] = "You have been logged out.";
            return RedirectToAction("Index", "Home");
        }

        // Helper method to safely redirect the user after login
        private IActionResult RedirectToLocal(string? returnUrl) // string? means returnUrl can be null
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home"); // Default page after login
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TourManagementSystem.Models;
using TourManagementSystem.Services;
using System.Collections.Generic; // Required for List<Claim>
using System.Security.Claims;    // Required for ClaimsPrincipal, ClaimTypes, Claim
using Microsoft.AspNetCore.Authentication; // Required for HttpContext.SignInAsync/SignOutAsync
using Microsoft.AspNetCore.Authentication.Cookies; // Required for CookieAuthenticationDefaults
using Microsoft.AspNetCore.Authorization; // Required for [AllowAnonymous]

namespace TourManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous] // Explicitly allow anonymous access
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new RegisterViewModel());
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var (success, errorMessage, createdUser) = await _userService.RegisterUserAsync(model);
                if (success && createdUser != null)
                {
                    TempData["SuccessMessage"] = "Registration successful! Please log in.";
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessage ?? "An unknown error occurred during registration.");
                }
            }
            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = await _userService.ValidateUserAsync(model.Email, model.Password);
                if (user != null)
                {
                    // --- Create claims for the authenticated user ---
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Unique ID for the user
                        new Claim(ClaimTypes.Name, user.Email), // User.Identity.Name will be the email
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.GivenName, user.FullName), // For displaying full name
                        // Add the role claim. This is crucial for [Authorize(Roles="Admin")]
                        new Claim(ClaimTypes.Role, user.Role ?? "User") // Use "User" if role is null
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true, // Allow the cookie to be refreshed
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60), // Cookie expiration
                        IsPersistent = model.RememberMe, // If "RememberMe" is checked, cookie persists across browser sessions
                        // IssuedUtc = <DateTimeOffset>, // Optional: when the cookie was issued
                        // RedirectUri = <string> // Optional: if you want to redirect after successful sign-in
                    };

                    // --- Sign the user in ---
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    TempData["SuccessMessage"] = $"Welcome back, {user.FullName}!";

                    if (user.Role != null && user.Role.Equals("Admin", System.StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your email and password.");
                }
            }
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        // No [AllowAnonymous] needed here, as only authenticated users should be able to log out.
        // If you want to allow logout even if somehow unauthenticated, add [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            // --- Sign the user out ---
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Message"] = "You have been logged out.";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        [AllowAnonymous] // Allow anyone to see the access denied page
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
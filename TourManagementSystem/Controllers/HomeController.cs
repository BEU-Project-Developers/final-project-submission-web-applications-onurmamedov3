// File: TourManagementSystem/Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TourManagementSystem.Models; // For ErrorViewModel

namespace TourManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SearchOffers(
            string? searchTerm,
            string? checkInDate,
            string? checkOutDate,
            int? adults,
            int? children,
            int? minRating,
            decimal? maxPrice,
            string entityType = "Hotels") // Default to Hotels
        {
            _logger.LogInformation("Search initiated from Home for {EntityType} with term: {SearchTerm}, CheckIn: {CheckIn}, CheckOut: {CheckOut}, Adults: {Adults}, Children: {Children}, MinRating: {MinRating}, MaxPrice: {MaxPrice}",
                                   entityType, searchTerm, checkInDate, checkOutDate, adults, children, minRating, maxPrice);

            if (entityType.Equals("Hotels", System.StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Offers", new
                {
                    searchTerm,
                    checkInDate,
                    checkOutDate,
                    adults,
                    children,
                    minRating,
                    maxPrice,
                    entityType // Pass along to OffersController
                });
            }
            // Add 'else if' blocks here for other entity types (CarRentals, Flights, etc.)
            // else if (entityType.Equals("CarRentals", System.StringComparison.OrdinalIgnoreCase))
            // {
            //     // Example: Redirect to a different controller or action for car rentals
            //     // return RedirectToAction("SearchCarRentals", "Offers", new { searchTerm, pickupLocation, ... });
            // }

            TempData["ErrorMessage"] = $"Search for '{entityType}' is not yet implemented or is handled differently.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
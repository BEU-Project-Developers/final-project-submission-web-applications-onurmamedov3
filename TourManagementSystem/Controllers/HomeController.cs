// File: TourManagementSystem/Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TourManagementSystem.Models;
using Activity = System.Diagnostics.Activity; // For ErrorViewModel and TripType if used directly

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
            return View(); // No specific model needed for the home page view itself for now
        }

        [HttpGet]
        public IActionResult SearchOffers(
            // --- Parameters from ALL search forms on Home Page ---
            // These are nullable as not all forms will send all parameters.
            // The 'name' attributes in your Home/Index.cshtml forms must match these parameter names.

            // Hotel specific
            string? destination,        // From Hotel form (also generic destination)
            string? hotelName,

            // Car Rental specific
            string? location,           // From Car Rental form (pickup location)
            string? pickupDate,         // From Car Rental form (can also be hotel check-in)
            string? carRentalReturnDate,// Specific for Car Rental return
            string? carModelKeyword,
            string? driverAge,          // From Car Rental form

            // Flight specific
            string? origin,
            string? flightDestination,  // Specific for Flight 'To' field
            string? departureDate,      // Specific for Flight departure
            string? returnDate,         // Can be general return, or flight specific
            int? passengers,
            TripType? tripType,         // From Flight form (if you add a selector there)

            // Trips specific
            string? tripSearchTerm,     // From Trips form (could be general destination/activity)
            string? startDate,          // From Trips form
            int? travelers,

            // Cruises specific
            string? cruiseSearchTerm,   // From Cruises form (destination region)
            string? departureMonth,     // From Cruises form (format yyyy-MM)

            // Activities specific
            string? activitySearchTerm, // From Activities form (activity name or location)
            string? activityDate,
            int? participants,

            // Common/General parameters that might be shared or are fallbacks
            string? searchTerm,         // A very generic search term
            string? checkInDate,        // Generic check-in/start date
            string? checkOutDate,       // Generic check-out/end date
            int? adults,               // Generic adults count
            int? children,             // Generic children count
            int? minRating,            // For hotels
            decimal? maxPrice,         // For hotels

            string entityType = "Hotels") // The hidden input from the active search panel
        {
            _logger.LogInformation("HomeController/SearchOffers called for EntityType: {EntityType}", entityType);

            object routeValues = new { entityType }; // Start with entityType

            if (entityType.Equals("Hotels", System.StringComparison.OrdinalIgnoreCase))
            {
                routeValues = new
                {
                    destination,
                    hotelName,
                    checkInDate = checkInDate ?? pickupDate ?? departureDate, // Consolidate date inputs if needed
                    checkOutDate = checkOutDate ?? returnDate ?? carRentalReturnDate,
                    adults,
                    children,
                    minRating,
                    maxPrice,
                    entityType
                };
            }
            else if (entityType.Equals("CarRentals", System.StringComparison.OrdinalIgnoreCase))
            {
                routeValues = new
                {
                    location = location ?? destination ?? searchTerm, // Use specific car location, fallback to general
                    pickupDate = pickupDate ?? checkInDate,       // Use specific car pickup, fallback
                    returnDate = carRentalReturnDate ?? checkOutDate, // Use specific car return, fallback
                    carModelKeyword,
                    // driverAge, // Pass if you have logic for it in OffersController/CarRentalService
                    entityType
                };
            }
            else if (entityType.Equals("Flights", System.StringComparison.OrdinalIgnoreCase))
            {
                routeValues = new
                {
                    origin,
                    flightDestination = flightDestination ?? destination, // Use specific flight dest, fallback
                    departureDate = departureDate ?? checkInDate,
                    returnDate = returnDate ?? checkOutDate,
                    passengers,
                    tripType = tripType ?? (string.IsNullOrEmpty(returnDate ?? checkOutDate) ? TripType.OneWay : TripType.RoundTrip), // Infer if not explicitly passed
                    entityType
                };
            }
            else if (entityType.Equals("Trips", StringComparison.OrdinalIgnoreCase))
            {
                routeValues = new
                {
                    destination = tripSearchTerm ?? searchTerm ?? destination,
                    startDate = startDate ?? checkInDate,
                    travelers,
                    entityType
                };
            }
            else if (entityType.Equals("Cruises", StringComparison.OrdinalIgnoreCase))
            {
                routeValues = new
                {
                    destinationRegion = cruiseSearchTerm ?? searchTerm ?? destination,
                    departureMonth,
                    entityType
                };
            }
            else if (entityType.Equals("Activities", StringComparison.OrdinalIgnoreCase))
            {
                routeValues = new
                {
                    activitySearchTerm = activitySearchTerm ?? searchTerm ?? destination,
                    activityDate = activityDate ?? checkInDate,
                    participants,
                    entityType
                };
            }

            if (routeValues != null)
            {
                _logger.LogInformation("Redirecting to Offers/Index with route values for {EntityType}", entityType);
                return RedirectToAction("Index", "Offers", routeValues);
            }
            else
            {
                _logger.LogWarning("No specific route values determined for EntityType: {EntityType}. Redirecting to Home/Index.", entityType);
                TempData["ErrorMessage"] = $"Search for '{entityType}' is not yet fully configured for redirection.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
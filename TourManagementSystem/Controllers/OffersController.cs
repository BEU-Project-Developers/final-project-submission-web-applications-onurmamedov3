// File: TourManagementSystem/Controllers/OffersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Controllers
{
    public class OffersController : Controller
    {
        private readonly IHotelService _hotelService;
        private readonly ILogger<OffersController> _logger;

        public OffersController(IHotelService hotelService, ILogger<OffersController> logger)
        {
            _hotelService = hotelService ?? throw new ArgumentNullException(nameof(hotelService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: /Offers/Index
        public async Task<IActionResult> Index(
            string? searchTerm,
            string? checkInDate,
            string? checkOutDate,
            int? adults,
            int? children,
            int? minRating,
            decimal? maxPrice,
            string entityType = "Hotels", // Optional parameter
            List<string>? amenities = null // OPTIONAL parameter, must come after or be made required
                                           // Defaulting to null makes it optional and keeps it last.
            )
        {
            ViewData["Title"] = "Our Offers";
            ViewData["SearchTerm"] = searchTerm;
            ViewData["CheckInDate"] = checkInDate;
            ViewData["CheckOutDate"] = checkOutDate;
            ViewData["Adults"] = adults;
            ViewData["Children"] = children;
            ViewData["MinRating"] = minRating;
            ViewData["MaxPrice"] = maxPrice;
            ViewData["EntityType"] = entityType;
            ViewData["SelectedAmenities"] = amenities ?? new List<string>(); // Handles null case gracefully

            IEnumerable<HotelViewModel> viewModels = new List<HotelViewModel>();

            if (entityType?.Equals("Hotels", StringComparison.OrdinalIgnoreCase) == true)
            {
                _logger.LogInformation("Fetching hotel offers. Criteria - Term: {SearchTerm}, CheckIn: {CheckIn}, CheckOut: {CheckOut}, Adults: {Adults}, Children: {Children}, MinRating: {MinRating}, MaxPrice: {MaxPrice}, Amenities: {Amenities}",
                                   searchTerm, checkInDate, checkOutDate, adults, children, minRating, maxPrice, string.Join(",", amenities ?? new List<string>()));

                var hotelsData = await _hotelService.SearchHotelsAsync(searchTerm, checkInDate, checkOutDate, adults, children, minRating, maxPrice, amenities);

                viewModels = hotelsData.Select(h => new HotelViewModel
                {
                    Id = h.Id,
                    Name = h.Name,
                    Destination = h.Destination,
                    Address = h.Address,
                    Description = h.Description,
                    PricePerNight = h.PricePerNight,
                    Rating = h.Rating,
                    AvailableRooms = h.AvailableRooms,
                    PrimaryImageUrl = h.PrimaryImageUrl
                }).ToList();

                if (!viewModels.Any() && (
                    !string.IsNullOrWhiteSpace(searchTerm) ||
                    minRating.HasValue || maxPrice.HasValue ||
                    !string.IsNullOrWhiteSpace(checkInDate) || !string.IsNullOrWhiteSpace(checkOutDate) ||
                    (amenities != null && amenities.Any())))
                {
                    TempData["InfoMessage"] = "No hotel offers found matching your specific criteria. Try broadening your search.";
                }
                else if (!viewModels.Any())
                {
                    TempData["InfoMessage"] = "No hotel offers currently available.";
                }
            }
            else
            {
                _logger.LogWarning("Offers page requested for an unsupported or unspecified entityType: {EntityType}", entityType);
                TempData["InfoMessage"] = $"Offers for '{entityType}' are not currently displayed on this page.";
            }

            return View(viewModels);
        }

        // GET: Offers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _hotelService.GetHotelByIdAsync(id.Value);
            if (hotel == null)
            {
                return NotFound();
            }
            var viewModel = new HotelViewModel
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Destination = hotel.Destination,
                Address = hotel.Address,
                Description = hotel.Description,
                PricePerNight = hotel.PricePerNight,
                Rating = hotel.Rating,
                AvailableRooms = hotel.AvailableRooms,
                PrimaryImageUrl = hotel.PrimaryImageUrl
            };
            return View("Details_Placeholder", viewModel);
        }
    }
}
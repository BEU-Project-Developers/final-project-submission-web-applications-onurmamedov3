// File: TourManagementSystem/Controllers/BookingController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System; // For Random
using System.Threading.Tasks;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Controllers
{
    public class BookingController : Controller
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IHotelService _hotelService;
        private readonly ICarRentalService _carRentalService;
        private readonly IFlightService _flightService;
        private readonly ICruiseService _cruiseService;
        private readonly IActivityService _activityService;
        private readonly ITripService _tripService;

        public BookingController(ILogger<BookingController> logger,
                                 IHotelService hotelService,
                                 ICarRentalService carRentalService,
                                 IFlightService flightService,
                                 ICruiseService cruiseService,
                                 IActivityService activityService,
                                 ITripService tripService)
        {
            _logger = logger;
            _hotelService = hotelService;
            _carRentalService = carRentalService;
            _flightService = flightService;
            _cruiseService = cruiseService;
            _activityService = activityService;
            _tripService = tripService;
        }

        // GET: /Booking/Create?offerId=1&offerType=Hotel
        [HttpGet]
        public async Task<IActionResult> Create(int? offerId, string offerType)
        {
            if (offerId == null || string.IsNullOrEmpty(offerType))
            {
                _logger.LogWarning("Booking/Create [GET] called with missing offerId or offerType.");
                TempData["ErrorMessage"] = "Invalid booking request. Offer details are missing.";
                return RedirectToAction("Index", "Offers", new { entityType = "Hotels" });
            }

            _logger.LogInformation("Booking/Create [GET] for OfferId: {OfferId}, OfferType: {OfferType}", offerId, offerType);
            object? offerDetails = null;

            switch (offerType.ToLower())
            {
                case "hotel":
                    var hotel = await _hotelService.GetHotelByIdAsync(offerId.Value);
                    if (hotel != null) offerDetails = new HotelViewModel { Id = hotel.Id, Name = hotel.Name, PricePerNight = hotel.PricePerNight, Description = hotel.Description, PrimaryImageUrl = hotel.PrimaryImageUrl, Rating = hotel.Rating, Destination = hotel.Destination /* Add all needed fields for display */ };
                    break;
                case "carrental":
                    var car = await _carRentalService.GetCarRentalByIdAsync(offerId.Value);
                    if (car != null) offerDetails = new CarRentalViewModel { Id = car.Id, CarModel = car.CarModel, Company = car.Company, PricePerDay = car.PricePerDay, Location = car.Location, ImageUrl = car.ImageUrl };
                    break;
                case "flight": // Error was on line 66
                    var flight = await _flightService.GetFlightByIdAsync(offerId.Value);
                    if (flight != null) offerDetails = new FlightViewModel
                    {
                        Id = flight.Id,
                        Airline = flight.Airline,
                        Price = flight.Price,
                        AirlineLogoUrl = flight.AirlineLogoUrl, // Use AirlineLogoUrl from FlightViewModel
                        DepartureCity = flight.DepartureCity,
                        ArrivalCity = flight.ArrivalCity,
                        FlightNumber = flight.FlightNumber
                        // Add other necessary fields from flight entity
                    };
                    break;
                case "cruise":
                    var cruise = await _cruiseService.GetCruiseByIdAsync(offerId.Value);
                    if (cruise != null) offerDetails = new CruiseViewModel { Id = cruise.Id, CruiseLine = cruise.CruiseLine, Destination = cruise.Destination, DeparturePort = cruise.DeparturePort, DurationDays = cruise.DurationDays, Price = cruise.Price, ImageUrl = cruise.ImageUrl, ItinerarySummary = $"{cruise.DurationDays}-Day {cruise.Destination} Cruise" };
                    break;
                case "activity": // Error was on line 74
                    var activity = await _activityService.GetActivityByIdAsync(offerId.Value);
                    // Ensure Activity entity has ImageUrl property
                    if (activity != null) offerDetails = new ActivityViewModel
                    {
                        Id = activity.Id,
                        Name = activity.Name,
                        Location = activity.Location,
                        Category = activity.Category,
                        Price = activity.Price,
                        DurationHours = activity.DurationHours,
                        ImageUrl = activity.ImageUrl, // Maps from Activity.ImageUrl
                        Description = activity.Description
                    };
                    break;
                case "trip":
                    var trip = await _tripService.GetTripByIdAsync(offerId.Value);
                    if (trip != null) offerDetails = new TripViewModel { Id = trip.Id, Title = trip.Title, Destination = trip.Destination, Price = trip.Price, DurationDays = trip.DurationDays, ImageUrl = trip.ImageUrl, Description = trip.Description };
                    break;
                default:
                    _logger.LogWarning("Booking/Create [GET] - Unknown offerType: {OfferType}", offerType);
                    TempData["ErrorMessage"] = $"Cannot process booking for an unknown offer type: {offerType}.";
                    return RedirectToAction("Index", "Home");
            }

            if (offerDetails == null)
            {
                _logger.LogWarning("Booking/Create [GET] - Offer not found for Type: {OfferType}, ID: {OfferId}", offerType, offerId);
                TempData["ErrorMessage"] = $"The selected {offerType.ToLower()} (ID: {offerId}) could not be found.";
                return RedirectToAction("Index", "Offers", new { entityType = offerType });
            }

            ViewBag.OfferDetails = offerDetails;
            var bookingModel = new BookingSubmissionViewModel { OfferId = offerId.Value, OfferType = offerType };
            return View(bookingModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingSubmissionViewModel model)
        {
            _logger.LogInformation("Booking/Create [POST] attempt for OfferId: {OfferId}, OfferType: {OfferType} by {CustomerEmail}", model.OfferId, model.OfferType, model.CustomerEmail);

            if (ModelState.IsValid)
            {
                // ... (Processing logic as before) ...
                _logger.LogInformation("Booking processed successfully for {OfferType} ID {OfferId}.", model.OfferType, model.OfferId);
                TempData["SuccessMessage"] = $"Your booking request for {model.OfferType} (ID: {model.OfferId}) has been submitted successfully! We will contact you shortly.";
                return RedirectToAction("BookingConfirmation", new { bookingId = new Random().Next(10000, 99999) });
            }

            _logger.LogWarning("Booking/Create [POST] - Invalid ModelState for OfferId: {OfferId}.", model.OfferId);
            object? offerDetails = null;
            switch (model.OfferType.ToLower()) // Use model.OfferType here
            {
                case "hotel": var h = await _hotelService.GetHotelByIdAsync(model.OfferId); if (h != null) offerDetails = new HotelViewModel { Name = h.Name, PricePerNight = h.PricePerNight, PrimaryImageUrl = h.PrimaryImageUrl, Destination = h.Destination, Rating = h.Rating }; break;
                case "carrental": var cr = await _carRentalService.GetCarRentalByIdAsync(model.OfferId); if (cr != null) offerDetails = new CarRentalViewModel { CarModel = cr.CarModel, Company = cr.Company, PricePerDay = cr.PricePerDay, ImageUrl = cr.ImageUrl, Location = cr.Location }; break;
                case "flight": // Error was on line 139
                    var fl = await _flightService.GetFlightByIdAsync(model.OfferId);
                    if (fl != null) offerDetails = new FlightViewModel
                    {
                        Airline = fl.Airline,
                        Price = fl.Price,
                        AirlineLogoUrl = fl.AirlineLogoUrl, // Use AirlineLogoUrl
                        DepartureCity = fl.DepartureCity,
                        ArrivalCity = fl.ArrivalCity,
                        FlightNumber = fl.FlightNumber
                    };
                    break;
                case "cruise": var cru = await _cruiseService.GetCruiseByIdAsync(model.OfferId); if (cru != null) offerDetails = new CruiseViewModel { CruiseLine = cru.CruiseLine, Price = cru.Price, ImageUrl = cru.ImageUrl, Destination = cru.Destination, DurationDays = cru.DurationDays, DeparturePort = cru.DeparturePort, ItinerarySummary = $"{cru.DurationDays}-Day {cru.Destination} Cruise" }; break;
                case "activity": // Error was on line 141
                    var act = await _activityService.GetActivityByIdAsync(model.OfferId);
                    if (act != null) offerDetails = new ActivityViewModel
                    {
                        Name = act.Name,
                        Price = act.Price,
                        ImageUrl = act.ImageUrl, // Maps from Activity.ImageUrl
                        Location = act.Location,
                        Category = act.Category,
                        DurationHours = act.DurationHours
                    };
                    break;
                case "trip": var trp = await _tripService.GetTripByIdAsync(model.OfferId); if (trp != null) offerDetails = new TripViewModel { Title = trp.Title, Price = trp.Price, ImageUrl = trp.ImageUrl, Destination = trp.Destination, DurationDays = trp.DurationDays }; break;
            }
            ViewBag.OfferDetails = offerDetails;
            ViewData["Title"] = $"Book Your {model.OfferType} - Review Details";
            return View(model);
        }

        public IActionResult BookingConfirmation(int bookingId)
        {
            ViewData["Title"] = "Booking Confirmed";
            ViewBag.BookingId = bookingId;
            return View();
        }
    }
}
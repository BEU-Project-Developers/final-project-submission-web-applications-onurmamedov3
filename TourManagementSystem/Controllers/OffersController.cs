// File: TourManagementSystem/Controllers/OffersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourManagementSystem.Models; // Needed for ViewModels and TripType enum
using TourManagementSystem.Services; // Needed for service interfaces

namespace TourManagementSystem.Controllers
{
    public class OffersController : Controller
    {
        private readonly IHotelService _hotelService;
        private readonly ICarRentalService _carRentalService;
        private readonly IFlightService _flightService;
        private readonly ICruiseService _cruiseService;
        private readonly IActivityService _activityService;
        // Inject ITripService when ready
        private readonly ILogger<OffersController> _logger;

        public OffersController(
            IHotelService hotelService,
            ICarRentalService carRentalService,
            IFlightService flightService,
            ICruiseService cruiseService,
            IActivityService activityService,
            // ITripService tripService,
            ILogger<OffersController> logger)
        {
            _hotelService = hotelService ?? throw new ArgumentNullException(nameof(hotelService));
            _carRentalService = carRentalService ?? throw new ArgumentNullException(nameof(carRentalService));
            _flightService = flightService ?? throw new ArgumentNullException(nameof(flightService));
            _cruiseService = cruiseService ?? throw new ArgumentNullException(nameof(cruiseService));
            _activityService = activityService ?? throw new ArgumentNullException(nameof(activityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index(
            // --- Parameters without default values first ---
            // These come from form submissions or query strings.
            // They are nullable to indicate they might not always be provided.

            // Hotel specific
            string? destination, string? hotelName, int? minRating, decimal? maxPrice,
            // Car Rental specific
            string? location, string? carModelKeyword,
            // Flight specific
            string? origin, string? flightDestination, string? departureDate, int? passengers,
            // Cruise specific
            string? destinationRegion, string? cruiseDeparturePort, string? cruiseLineName,
            int? minCruiseDuration, int? maxCruiseDuration, decimal? maxCruisePrice, string? departureMonth,
            // Activity specific
            string? activitySearchTerm, string? activityCategory, string? activityDate,
            int? maxActivityDuration, decimal? maxActivityPrice, int? participants,
            // Common dates that might be aliased
            string? checkInDate, string? checkOutDate,
            string? pickupDate, string? returnDate, // Generic returnDate, used by multiple
                                                    // Common occupancy
            int? adults, int? children,

            // --- Optional parameters (with default values) at the end ---
            List<string>? amenities = null,          // Default is null
            TripType tripType = TripType.RoundTrip, // Default is RoundTrip for flights
            string entityType = "Hotels"            // Default is Hotels
            )
        {
            ViewData["Title"] = "Our Offers";
            ViewData["EntityType"] = entityType;

            // Pre-fill ViewData for search forms on the Offers page
            ViewData["Destination"] = destination; ViewData["HotelName"] = hotelName; ViewData["CheckInDate"] = checkInDate;
            ViewData["CheckOutDate"] = checkOutDate; ViewData["Adults"] = adults; ViewData["Children"] = children;
            ViewData["MinRating"] = minRating; ViewData["MaxPrice"] = maxPrice; ViewData["SelectedAmenities"] = amenities ?? new List<string>();

            ViewData["Location"] = location; ViewData["PickupDate"] = pickupDate; ViewData["ReturnDate_Car"] = returnDate;
            ViewData["CarModelKeyword"] = carModelKeyword;

            ViewData["Origin"] = origin; ViewData["FlightDestination"] = flightDestination; ViewData["DepartureDate"] = departureDate;
            ViewData["ReturnDate_Flight"] = returnDate; ViewData["Passengers"] = passengers; ViewData["TripType"] = tripType;

            ViewData["DestinationRegion"] = destinationRegion; ViewData["CruiseDeparturePort"] = cruiseDeparturePort;
            ViewData["CruiseLineName"] = cruiseLineName; ViewData["MinCruiseDuration"] = minCruiseDuration;
            ViewData["MaxCruiseDuration"] = maxCruiseDuration; ViewData["MaxCruisePrice"] = maxCruisePrice;
            ViewData["DepartureMonth"] = departureMonth;

            ViewData["ActivitySearchTerm"] = activitySearchTerm; ViewData["ActivityCategory"] = activityCategory;
            ViewData["ActivityDate"] = activityDate; ViewData["MaxActivityDuration"] = maxActivityDuration;
            ViewData["MaxActivityPrice"] = maxActivityPrice; ViewData["Participants"] = participants;


            object displayData = new List<object>();
            _logger.LogInformation("OffersController/Index. Processing EntityType: {EntityType}", entityType);

            if (entityType.Equals("Hotels", StringComparison.OrdinalIgnoreCase))
            {
                var hotelsData = await _hotelService.SearchHotelsAsync(destination, hotelName, checkInDate, checkOutDate, adults, children, minRating, maxPrice, amenities);
                var vms = hotelsData.Select(h => new HotelViewModel { Id = h.Id, Name = h.Name, Destination = h.Destination, Address = h.Address, Description = h.Description, PricePerNight = h.PricePerNight, Rating = h.Rating, AvailableRooms = h.AvailableRooms, PrimaryImageUrl = h.PrimaryImageUrl }).ToList();
                displayData = vms;
                SetInfoMessage(vms.Any(), IsSearchAttempt(new object[] { destination, hotelName, minRating, maxPrice, checkInDate, amenities, checkOutDate }), "hotel");
            }
            else if (entityType.Equals("CarRentals", StringComparison.OrdinalIgnoreCase))
            {
                var carsData = await _carRentalService.SearchCarRentalsAsync(location, pickupDate, returnDate, carModelKeyword);
                var vms = carsData.Select(cr => new CarRentalViewModel { Id = cr.Id, CarModel = cr.CarModel, Company = cr.Company, PricePerDay = cr.PricePerDay, Location = cr.Location, ImageUrl = cr.ImageUrl }).ToList();
                displayData = vms;
                SetInfoMessage(vms.Any(), IsSearchAttempt(new object[] { location, carModelKeyword, pickupDate, returnDate }), "car rental");
            }
            else if (entityType.Equals("Flights", StringComparison.OrdinalIgnoreCase))
            {
                DateTime depDateParsed; DateTime? retDateParsed = null;
                if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(flightDestination) || !DateTime.TryParse(departureDate, out depDateParsed))
                {
                    TempData["InfoMessage"] = "Please provide Origin, Destination, and a valid Departure Date for flight search.";
                }
                else
                {
                    if (!string.IsNullOrEmpty(returnDate) && DateTime.TryParse(returnDate, out DateTime rdp)) retDateParsed = rdp;
                    var actualTripType = tripType; // Use the value passed in or defaulted
                    if (actualTripType == TripType.RoundTrip && !retDateParsed.HasValue) actualTripType = TripType.OneWay;
                    if (actualTripType == TripType.OneWay) retDateParsed = null;

                    var flightsData = await _flightService.SearchFlightsAsync(origin, flightDestination, depDateParsed, retDateParsed, passengers ?? 1, actualTripType);
                    var vms = flightsData.Select(f => new FlightViewModel { Id = f.Id, Airline = f.Airline, DepartureCity = f.DepartureCity, ArrivalCity = f.ArrivalCity, DepartureTime = f.DepartureTime, ArrivalTime = f.ArrivalTime, Price = f.Price, AirlineLogoUrl = f.AirlineLogoUrl, FlightNumber = f.FlightNumber, Duration = (f.ArrivalTime - f.DepartureTime).ToString(@"hh\h\ mm\m"), SearchedTripType = actualTripType }).ToList();
                    displayData = vms;
                    SetInfoMessage(vms.Any(), IsSearchAttempt(new object[] { origin, flightDestination, departureDate, returnDate, passengers, tripType }), "flight");
                }
            }
            else if (entityType.Equals("Cruises", StringComparison.OrdinalIgnoreCase))
            {
                var cruisesData = await _cruiseService.SearchCruisesAsync(destinationRegion, cruiseDeparturePort, cruiseLineName, minCruiseDuration, maxCruiseDuration, maxCruisePrice);
                var vms = cruisesData.Select(c => new CruiseViewModel { Id = c.Id, CruiseLine = c.CruiseLine, DeparturePort = c.DeparturePort, Destination = c.Destination, DurationDays = c.DurationDays, Price = c.Price, ImageUrl = c.ImageUrl, ItinerarySummary = $"{c.DurationDays}-Day {c.Destination} Cruise" }).ToList();
                displayData = vms;
                SetInfoMessage(vms.Any(), IsSearchAttempt(new object[] { destinationRegion, cruiseDeparturePort, cruiseLineName, minCruiseDuration, maxCruiseDuration, maxCruisePrice, departureMonth }), "cruise");
            }
            else if (entityType.Equals("Activities", StringComparison.OrdinalIgnoreCase))
            {
                DateTime? actDateParsed = null;
                if (!string.IsNullOrEmpty(activityDate) && DateTime.TryParse(activityDate, out DateTime adp)) actDateParsed = adp;
                var activitiesData = await _activityService.SearchActivitiesAsync(activitySearchTerm, activityCategory, actDateParsed, maxActivityDuration, maxActivityPrice);
                var vms = activitiesData.Select(a => new ActivityViewModel { Id = a.Id, Name = a.Name, Location = a.Location, Category = a.Category, DurationHours = a.DurationHours, Price = a.Price, Description = a.Description, ImageUrl = "~/images/default_activity.jpg" }).ToList();
                displayData = vms;
                SetInfoMessage(vms.Any(), IsSearchAttempt(new object[] { activitySearchTerm, activityCategory, activityDate, maxActivityDuration, maxActivityPrice, participants }), "activity");
            }
            else
            {
                TempData["InfoMessage"] = $"Offers for '{entityType}' are under construction.";
            }
            return View(displayData);
        }

        private void SetInfoMessage(bool hasResults, bool wasSearchAttempt, string itemType)
        {
            if (!hasResults && wasSearchAttempt)
            {
                TempData["InfoMessage"] = $"No {itemType} offers found matching your criteria.";
            }
            else if (!hasResults)
            { // No search attempt, but still no results (e.g., empty database)
                TempData["InfoMessage"] = $"No {itemType} offers currently available.";
            }
        }

        // Generic IsSearchAttempt helper
        private bool IsSearchAttempt(object[] criteria)
        {
            foreach (var criterion in criteria)
            {
                if (criterion == null) continue;

                if (criterion is string s && !string.IsNullOrWhiteSpace(s)) return true;
                if (criterion.GetType().IsValueType && !object.Equals(criterion, Activator.CreateInstance(criterion.GetType())))
                {
                    // This checks if a value type (int, decimal, DateTime, enum) is not its default value
                    // For nullable value types (int?, decimal?, DateTime?, TripType?), this works if they have a value.
                    return true;
                }
                if (criterion is List<string> l && l.Any()) return true;
            }
            return false;
        }


        public async Task<IActionResult> Details(int? id, string entityType = "Hotels")
        {
            if (id == null) return NotFound();
            _logger.LogInformation("Details requested for ID: {Id}, EntityType: {EntityType}", id, entityType);

            if (entityType.Equals("Hotels", StringComparison.OrdinalIgnoreCase))
            {
                var hotel = await _hotelService.GetHotelByIdAsync(id.Value); if (hotel == null) return NotFound();
                var vm = new HotelViewModel { Id = hotel.Id, Name = hotel.Name, Destination = hotel.Destination, Address = hotel.Address, Description = hotel.Description, PricePerNight = hotel.PricePerNight, Rating = hotel.Rating, AvailableRooms = hotel.AvailableRooms, PrimaryImageUrl = hotel.PrimaryImageUrl };
                return View("Details_Placeholder", vm);
            }
            else if (entityType.Equals("CarRentals", StringComparison.OrdinalIgnoreCase))
            {
                var car = await _carRentalService.GetCarRentalByIdAsync(id.Value); if (car == null) return NotFound();
                var vm = new CarRentalViewModel { Id = car.Id, CarModel = car.CarModel, Company = car.Company, PricePerDay = car.PricePerDay, Location = car.Location, ImageUrl = car.ImageUrl };
                return View("Details_Placeholder_CarRental", vm);
            }
            else if (entityType.Equals("Flights", StringComparison.OrdinalIgnoreCase))
            {
                var flight = await _flightService.GetFlightByIdAsync(id.Value); if (flight == null) return NotFound();
                // Corrected mapping for FlightViewModel in Details
                var vm = new FlightViewModel
                {
                    Id = flight.Id,
                    Airline = flight.Airline,
                    DepartureCity = flight.DepartureCity,
                    ArrivalCity = flight.ArrivalCity,
                    DepartureTime = flight.DepartureTime,
                    ArrivalTime = flight.ArrivalTime,
                    Price = flight.Price,
                    AirlineLogoUrl = flight.AirlineLogoUrl,
                    FlightNumber = flight.FlightNumber,
                    Duration = (flight.ArrivalTime - flight.DepartureTime).ToString(@"hh\h\ mm\m")
                    // SearchedTripType is not directly applicable when fetching a single flight record by ID for details
                };
                return View("Details_Placeholder_Flight", vm);
            }
            else if (entityType.Equals("Cruises", StringComparison.OrdinalIgnoreCase))
            {
                var cruise = await _cruiseService.GetCruiseByIdAsync(id.Value); if (cruise == null) return NotFound();
                var vm = new CruiseViewModel { Id = cruise.Id, CruiseLine = cruise.CruiseLine, DeparturePort = cruise.DeparturePort, Destination = cruise.Destination, DurationDays = cruise.DurationDays, Price = cruise.Price, ImageUrl = cruise.ImageUrl, ItinerarySummary = $"{cruise.DurationDays}-Day {cruise.Destination} Cruise" };
                return View("Details_Placeholder_Cruise", vm);
            }
            else if (entityType.Equals("Activities", StringComparison.OrdinalIgnoreCase))
            {
                var activity = await _activityService.GetActivityByIdAsync(id.Value); if (activity == null) return NotFound();
                var vm = new ActivityViewModel { Id = activity.Id, Name = activity.Name, Location = activity.Location, Category = activity.Category, DurationHours = activity.DurationHours, Price = activity.Price, Description = activity.Description, ImageUrl = "~/images/default_activity.jpg" };
                return View("Details_Placeholder_Activity", vm);
            }
            return NotFound();
        }
    }
}
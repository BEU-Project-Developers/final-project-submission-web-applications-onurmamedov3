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
        private readonly ITripService _tripService;
        private readonly ILogger<OffersController> _logger;

        public OffersController(
            IHotelService hotelService,
            ICarRentalService carRentalService,
            IFlightService flightService,
            ICruiseService cruiseService,
            IActivityService activityService,
            ITripService tripService,
            ILogger<OffersController> logger)
        {
            _hotelService = hotelService ?? throw new ArgumentNullException(nameof(hotelService));
            _carRentalService = carRentalService ?? throw new ArgumentNullException(nameof(carRentalService));
            _flightService = flightService ?? throw new ArgumentNullException(nameof(flightService));
            _cruiseService = cruiseService ?? throw new ArgumentNullException(nameof(cruiseService));
            _activityService = activityService ?? throw new ArgumentNullException(nameof(activityService));
            _tripService = tripService ?? throw new ArgumentNullException(nameof(tripService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index(
            // Hotel specific
            string? destination, string? hotelName, int? minRating, decimal? maxPrice,
            // Car Rental specific
            string? location, string? carModelKeyword,
            // Flight specific
            string? origin, string? flightDestination, string? departureDate, int? passengers,
            // Cruise specific
            string? destinationRegion, string? cruiseDeparturePort, string? cruiseLineName,
            int? minCruiseDuration, int? maxCruiseDuration, decimal? maxCruisePrice, string? departureMonth, // Kept departureMonth
                                                                                                             // Activity specific
            string? activitySearchTerm, string? activityCategory, string? activityDate,
            int? maxActivityDuration, decimal? maxActivityPrice, int? participants, // Kept maxActivityDuration
                                                                                    // Trip specific
            string? tripSearchTerm, string? startDate, int? travelers, decimal? maxTripPrice,
            // Common dates that might be aliased
            string? checkInDate, string? checkOutDate,
            string? pickupDate, string? returnDate,
            // Common occupancy
            int? adults, int? children,
            // Optional parameters
            List<string>? amenities = null,
            TripType tripType = TripType.RoundTrip,
            string entityType = "Hotels")
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
            ViewData["DepartureMonth"] = departureMonth; // For Cruise search form

            ViewData["ActivitySearchTerm"] = activitySearchTerm; ViewData["ActivityCategory"] = activityCategory;
            ViewData["ActivityDate"] = activityDate; ViewData["MaxActivityDuration"] = maxActivityDuration; // For Activity search form
            ViewData["MaxActivityPrice"] = maxActivityPrice; ViewData["Participants"] = participants;

            ViewData["TripSearchTerm"] = tripSearchTerm; ViewData["StartDate"] = startDate;
            ViewData["Travelers"] = travelers; ViewData["MaxTripPrice"] = maxTripPrice;


            object displayData = new List<object>(); // Use object to hold different ViewModel list types
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
                var actualPickupDate = pickupDate ?? checkInDate;
                var actualReturnDate = returnDate;

                var carsData = await _carRentalService.SearchCarRentalsAsync(location, actualPickupDate, actualReturnDate, carModelKeyword);
                var vms = carsData.Select(cr => new CarRentalViewModel
                {
                    Id = cr.Id,
                    CarModel = cr.CarModel, // Assuming CarRental entity has 'Model' that maps to CarRentalViewModel.CarModel
                    Company = cr.Company,
                    PricePerDay = cr.PricePerDay,
                    Location = cr.Location,
                    ImageUrl = cr.ImageUrl
                }).ToList();
                displayData = vms;
                SetInfoMessage(vms.Any(), IsSearchAttempt(new object[] { location, carModelKeyword, actualPickupDate, actualReturnDate }), "car rental");
            }
            else if (entityType.Equals("Flights", StringComparison.OrdinalIgnoreCase))
            {
                DateTime depDateParsed; DateTime? retDateParsed = null;
                var actualDepartureDate = departureDate ?? checkInDate;
                var actualReturnDate = returnDate;

                if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(flightDestination) || !DateTime.TryParse(actualDepartureDate, out depDateParsed))
                {
                    TempData["InfoMessage"] = "Please provide Origin, Destination, and a valid Departure Date for flight search.";
                }
                else
                {
                    if (!string.IsNullOrEmpty(actualReturnDate) && DateTime.TryParse(actualReturnDate, out DateTime rdp)) retDateParsed = rdp;
                    var actualTripType = tripType;
                    if (actualTripType == TripType.RoundTrip && !retDateParsed.HasValue) actualTripType = TripType.OneWay;
                    if (actualTripType == TripType.OneWay) retDateParsed = null;

                    var flightsData = await _flightService.SearchFlightsAsync(origin, flightDestination, depDateParsed, retDateParsed, passengers ?? 1, actualTripType);
                    var vms = flightsData.Select(f => new FlightViewModel
                    {
                        Id = f.Id,
                        Airline = f.Airline,
                        DepartureCity = f.DepartureCity,
                        ArrivalCity = f.ArrivalCity,
                        DepartureTime = f.DepartureTime,
                        ArrivalTime = f.ArrivalTime,
                        Price = f.Price,
                        AirlineLogoUrl = f.AirlineLogoUrl,
                        FlightNumber = f.FlightNumber,
                        Duration = (f.ArrivalTime - f.DepartureTime).ToString(@"hh\h\ mm\m"),
                        SearchedTripType = actualTripType
                    }).ToList();
                    displayData = vms;
                    SetInfoMessage(vms.Any(), IsSearchAttempt(new object[] { origin, flightDestination, actualDepartureDate, actualReturnDate, passengers, tripType }), "flight");
                }
            }
            else if (entityType.Equals("Cruises", StringComparison.OrdinalIgnoreCase))
            {
                // This call now has 7 arguments. Ensure your ICruiseService.SearchCruisesAsync matches.
                var cruisesData = await _cruiseService.SearchCruisesAsync(destinationRegion, cruiseDeparturePort, cruiseLineName, minCruiseDuration, maxCruiseDuration, maxCruisePrice);
                var vms = cruisesData.Select(c => new CruiseViewModel
                {
                    Id = c.Id,
                    CruiseLine = c.CruiseLine,
                    DeparturePort = c.DeparturePort,
                    Destination = c.Destination,
                    DurationDays = c.DurationDays,
                    Price = c.Price,
                    ImageUrl = c.ImageUrl,
                    ItinerarySummary = $"{c.DurationDays}-Day {c.Destination} Cruise" // Generated
                }).ToList();
                displayData = vms;
                SetInfoMessage(vms.Any(), IsSearchAttempt(new object[] { destinationRegion, cruiseDeparturePort, cruiseLineName, minCruiseDuration, maxCruiseDuration, maxCruisePrice, departureMonth }), "cruise");
            }
            else if (entityType.Equals("Activities", StringComparison.OrdinalIgnoreCase))
            {
                DateTime? actDateParsed = null;
                if (!string.IsNullOrEmpty(activityDate) && DateTime.TryParse(activityDate, out DateTime adp)) actDateParsed = adp;
                // This call now has 6 arguments. Ensure your IActivityService.SearchActivitiesAsync matches.
                var activitiesData = await _activityService.SearchActivitiesAsync(activitySearchTerm, activityCategory, actDateParsed, maxActivityDuration, maxActivityPrice);
                var vms = activitiesData.Select(a => new ActivityViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Location = a.Location,
                    Category = a.Category,
                    DurationHours = a.DurationHours,
                    Price = a.Price,
                    Description = a.Description,
                    ImageUrl = a.ImageUrl ?? "~/images/default_activity.jpg"
                }).ToList();
                displayData = vms;
                SetInfoMessage(vms.Any(), IsSearchAttempt(new object[] { activitySearchTerm, activityCategory, activityDate, maxActivityDuration, maxActivityPrice, participants }), "activity");
            }
            else if (entityType.Equals("Trips", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Fetching trip offers with criteria - Term: {TripSearchTerm}, StartDate: {StartDate}, Travelers: {Travelers}, MaxPrice: {MaxTripPrice}",
                                   tripSearchTerm, startDate, travelers, maxTripPrice);
                var tripsData = await _tripService.SearchTripsAsync(tripSearchTerm, startDate, travelers, maxTripPrice);
                var vms = tripsData.Select(t => new TripViewModel { Id = t.Id, Title = t.Title, Destination = t.Destination, DurationDays = t.DurationDays, Price = t.Price, Description = t.Description, ImageUrl = t.ImageUrl }).ToList();
                displayData = vms;
                SetInfoMessage(vms.Any(), IsSearchAttempt(new object[] { tripSearchTerm, startDate, travelers, maxTripPrice }), "trip package");
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
            {
                TempData["InfoMessage"] = $"No {itemType} offers currently available.";
            }
        }

        private bool IsSearchAttempt(object[] criteria)
        {
            foreach (var criterion in criteria)
            {
                if (criterion == null) continue;
                if (criterion is string s && !string.IsNullOrWhiteSpace(s)) return true;
                // Check for non-default value types (int, decimal, DateTime, enum)
                if (criterion.GetType().IsValueType && !object.Equals(criterion, Activator.CreateInstance(criterion.GetType()))) return true;
                // Check for nullable value types that have a value
                var underlyingType = Nullable.GetUnderlyingType(criterion.GetType());
                if (underlyingType != null && !object.Equals(criterion, null)) return true;

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
                var vm = new CarRentalViewModel
                {
                    Id = car.Id,
                    CarModel = car.CarModel, // Assuming CarRental entity has 'Model' that maps to CarRentalViewModel.CarModel
                    Company = car.Company,
                    PricePerDay = car.PricePerDay,
                    Location = car.Location,
                    ImageUrl = car.ImageUrl
                };
                return View("Details_Placeholder_CarRental", vm);
            }
            else if (entityType.Equals("Flights", StringComparison.OrdinalIgnoreCase))
            {
                var flight = await _flightService.GetFlightByIdAsync(id.Value); if (flight == null) return NotFound();
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
                    // SearchedTripType is intentionally omitted for single flight details
                };
                return View("Details_Placeholder_Flight", vm);
            }
            else if (entityType.Equals("Cruises", StringComparison.OrdinalIgnoreCase))
            {
                var cruise = await _cruiseService.GetCruiseByIdAsync(id.Value); if (cruise == null) return NotFound();
                var vm = new CruiseViewModel
                {
                    Id = cruise.Id,
                    CruiseLine = cruise.CruiseLine,
                    DeparturePort = cruise.DeparturePort,
                    Destination = cruise.Destination,
                    DurationDays = cruise.DurationDays,
                    Price = cruise.Price,
                    ImageUrl = cruise.ImageUrl,
                    ItinerarySummary = $"{cruise.DurationDays}-Day {cruise.Destination} Cruise" // Generated
                };
                return View("Details_Placeholder_Cruise", vm);
            }
            else if (entityType.Equals("Activities", StringComparison.OrdinalIgnoreCase))
            {
                var activity = await _activityService.GetActivityByIdAsync(id.Value); if (activity == null) return NotFound();
                var vm = new ActivityViewModel
                {
                    Id = activity.Id,
                    Name = activity.Name,
                    Location = activity.Location,
                    Category = activity.Category,
                    DurationHours = activity.DurationHours,
                    Price = activity.Price,
                    Description = activity.Description,
                    ImageUrl = activity.ImageUrl ?? "~/images/default_activity.jpg"
                };
                return View("Details_Placeholder_Activity", vm);
            }
            else if (entityType.Equals("Trips", StringComparison.OrdinalIgnoreCase))
            {
                var trip = await _tripService.GetTripByIdAsync(id.Value); if (trip == null) return NotFound();
                var vm = new TripViewModel { Id = trip.Id, Title = trip.Title, Destination = trip.Destination, DurationDays = trip.DurationDays, Price = trip.Price, Description = trip.Description, ImageUrl = trip.ImageUrl };
                return View("Details_Placeholder_Trip", vm);
            }
            _logger.LogWarning("Details action called for unknown EntityType: {EntityType} with ID: {Id}", entityType, id);
            return NotFound();
        }
    }
}
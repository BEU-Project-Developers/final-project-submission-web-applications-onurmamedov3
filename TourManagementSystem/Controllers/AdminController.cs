using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TourManagementSystem.Models; // For AdminDashboardViewModel, ActivityLogViewModel
using TourManagementSystem.Services; // For IHotelService

namespace TourManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IHotelService _hotelService;
        // Inject other services as needed:
        // private readonly ICarService _carService;
        // private readonly IFlightService _flightService;
        // private readonly ITripService _tripService;
        // private readonly ICruiseService _cruiseService;
        // private readonly IActivityService _activityService;
        // private readonly IBookingService _bookingService;
        // private readonly IUserService _userService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IHotelService hotelService,
            // ICarService carService,
            // IFlightService flightService,
            // ITripService tripService,
            // ICruiseService cruiseService,
            // IActivityService activityService,
            // IBookingService bookingService,
            // IUserService userService,
            ILogger<AdminController> logger)
        {
            _hotelService = hotelService;
            // _carService = carService;
            // _flightService = flightService;
            // _tripService = tripService;
            // _cruiseService = cruiseService;
            // _activityService = activityService;
            // _bookingService = bookingService;
            // _userService = userService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                UserName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? User.Identity?.Name,
                BookingTrendsLabels = new List<string>(), // Initialize to prevent null reference
                BookingTrendsData = new List<int>(),      // Initialize
                RecentActivities = new List<ActivityLogViewModel>() // Initialize
            };

            try
            {
                var hotels = await _hotelService.GetAllHotelsAsync();
                viewModel.TotalHotelsCount = hotels?.Count() ?? 0;

                // Placeholder: Replace with actual service calls
                // viewModel.TotalCarsCount = await _carService.GetTotalCountAsync();
                // viewModel.TotalFlightsCount = await _flightService.GetTotalCountAsync();
                // viewModel.TotalTripsCount = await _tripService.GetTotalCountAsync();
                // viewModel.TotalCruisesCount = await _cruiseService.GetTotalCountAsync();
                // viewModel.TotalActivitiesCount = await _activityService.GetTotalCountAsync();
                // viewModel.TotalBookingsCount = await _bookingService.GetTotalCountAsync();
                // viewModel.TotalUsersCount = await _userService.GetTotalCountAsync();
                // viewModel.PendingBookingsCount = await _bookingService.GetPendingCountAsync();
                // viewModel.NewUsersTodayCount = await _userService.GetNewUsersTodayCountAsync();


                // Example: Populate Booking Trends (replace with actual data retrieval)
                // For example, bookings in the last 7 days
                for (int i = 6; i >= 0; i--)
                {
                    viewModel.BookingTrendsLabels.Add(DateTime.Today.AddDays(-i).ToString("MMM dd"));
                    // Replace with actual booking count for that day
                    // viewModel.BookingTrendsData.Add(await _bookingService.GetBookingsCountOnDateAsync(DateTime.Today.AddDays(-i)));
                    viewModel.BookingTrendsData.Add(new Random().Next(5, 25)); // Placeholder random data
                }

                // Example: Populate Recent Activities (replace with actual data retrieval from an audit log service)
                viewModel.RecentActivities = new List<ActivityLogViewModel>
                {
                    new ActivityLogViewModel { Action = "New Hotel 'Grand Plaza' Added", User = "AdminUserX", Timestamp = DateTime.Now.AddHours(-1), DetailsUrl = Url.Action("Details", "AdminHotels", new { id = 1 }) },
                    new ActivityLogViewModel { Action = "User 'john.doe' Registered", User = "System", Timestamp = DateTime.Now.AddHours(-2) },
                    new ActivityLogViewModel { Action = "Booking #B00123 Confirmed", User = "AdminUserY", Timestamp = DateTime.Now.AddHours(-3), DetailsUrl = "/Admin/Bookings/Details/B00123" }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error populating admin dashboard.");
                TempData["ErrorMessage"] = "Could not load all dashboard data. Some information may be missing.";
                // Set default/error indicator values
                viewModel.TotalHotelsCount = viewModel.TotalHotelsCount == 0 ? -1 : viewModel.TotalHotelsCount; // Keep if already loaded, else -1
            }

            return View(viewModel);
        }
    }
}
//// File: TourManagementSystem/Controllers/AdminFlightsController.cs
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using TourManagementSystem.Models;        // For Flight entity and TripType enum
//using TourManagementSystem.Services;    // For IFlightService
//using TourManagementSystem.Models; // For AdminFlightViewModel

//namespace TourManagementSystem.Controllers
//{
//    [Authorize(Roles = "Admin")]
//    public class AdminFlightsController : Controller
//    {
//        private readonly IFlightService _flightService;
//        private readonly IUserService _userService; // Optional
//        private readonly ILogger<AdminFlightsController> _logger;

//        public AdminFlightsController(IFlightService flightService, IUserService userService, ILogger<AdminFlightsController> logger)
//        {
//            _flightService = flightService;
//            _userService = userService;
//            _logger = logger;
//        }

//        // GET: AdminFlights
//        public async Task<IActionResult> Index()
//        {
//            ViewData["AdminPageTitle"] = "Manage Flights";
//            var flights = await _flightService.GetAllFlightsAsync();
//            return View(flights); // Passes IEnumerable<Flight>
//        }

//        // GET: AdminFlights/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null) return NotFound();
//            var flightEntity = await _flightService.GetFlightByIdAsync(id.Value);
//            if (flightEntity == null) return NotFound();

//            var model = new AdminFlightViewModel // Map to ViewModel
//            {
//                Id = flightEntity.Id,
//                Airline = flightEntity.Airline,
//                FlightNumber = flightEntity.FlightNumber,
//                DepartureCity = flightEntity.DepartureCity,
//                ArrivalCity = flightEntity.ArrivalCity,
//                DepartureTime = flightEntity.DepartureTime,
//                ArrivalTime = flightEntity.ArrivalTime,
//                Price = flightEntity.Price,
//                AirlineLogoUrl = flightEntity.AirlineLogoUrl,
//                UserId = flightEntity.UserId
//            };
//            ViewData["AdminPageTitle"] = $"Details: {model.Airline} {model.FlightNumber}";
//            return View(model); // Needs Views/AdminFlights/Details.cshtml
//        }

//        // GET: AdminFlights/Create
//        public async Task<IActionResult> Create()
//        {
//            ViewData["AdminPageTitle"] = "Add New Flight";
//            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName");
//            return View(new AdminFlightViewModel());
//        }

//        // POST: AdminFlights/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(AdminFlightViewModel model)
//        {
//            if (model.ArrivalTime <= model.DepartureTime)
//            {
//                ModelState.AddModelError("ArrivalTime", "Arrival time must be after departure time.");
//            }

//            if (ModelState.IsValid)
//            {
//                int? currentAdminUserId = User.Identity?.IsAuthenticated == true && int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int uid) ? uid : (int?)null;
//                var (success, createdFlight, errorMessage) = await _flightService.CreateFlightAsync(model, model.UserId ?? currentAdminUserId);
//                if (success && createdFlight != null)
//                {
//                    TempData["SuccessMessage"] = $"Flight {createdFlight.Airline} {createdFlight.FlightNumber} created successfully!";
//                    return RedirectToAction(nameof(Index));
//                }
//                ModelState.AddModelError(string.Empty, errorMessage ?? "Creation failed.");
//            }
//            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
//            ViewData["AdminPageTitle"] = "Add New Flight - Errors";
//            return View(model);
//        }

//        // GET: AdminFlights/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null) return NotFound();
//            var flightEntity = await _flightService.GetFlightByIdAsync(id.Value);
//            if (flightEntity == null) return NotFound();

//            var model = new AdminFlightViewModel // Map entity to ViewModel for editing
//            {
//                Id = flightEntity.Id,
//                Airline = flightEntity.Airline,
//                FlightNumber = flightEntity.FlightNumber,
//                DepartureCity = flightEntity.DepartureCity,
//                ArrivalCity = flightEntity.ArrivalCity,
//                DepartureTime = flightEntity.DepartureTime,
//                ArrivalTime = flightEntity.ArrivalTime,
//                Price = flightEntity.Price,
//                AirlineLogoUrl = flightEntity.AirlineLogoUrl,
//                UserId = flightEntity.UserId
//            };
//            ViewData["AdminPageTitle"] = $"Edit Flight: {model.Airline} {model.FlightNumber}";
//            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
//            return View(model);
//        }

//        // POST: AdminFlights/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, AdminFlightViewModel model)
//        {
//            if (id != model.Id) return NotFound();

//            if (model.ArrivalTime <= model.DepartureTime)
//            {
//                ModelState.AddModelError("ArrivalTime", "Arrival time must be after departure time.");
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    var (success, errorMessage) = await _flightService.UpdateFlightAsync(model);
//                    if (success)
//                    {
//                        TempData["SuccessMessage"] = "Flight updated successfully!";
//                        return RedirectToAction(nameof(Index));
//                    }
//                    ModelState.AddModelError(string.Empty, errorMessage ?? "Update failed.");
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (await _flightService.GetFlightByIdAsync(model.Id) == null) return NotFound();
//                    else { ModelState.AddModelError(string.Empty, "The record was modified. Please reload."); }
//                }
//            }
//            ViewData["AdminPageTitle"] = $"Edit Flight - Errors: {model.Airline} {model.FlightNumber}";
//            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
//            return View(model);
//        }

//        // GET: AdminFlights/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null) return NotFound();
//            var flight = await _flightService.GetFlightByIdAsync(id.Value);
//            if (flight == null) return NotFound();
//            ViewData["AdminPageTitle"] = $"Confirm Delete: {flight.Airline} {flight.FlightNumber}";
//            return View(flight); // Pass Flight entity for confirmation display
//        }

//        // POST: AdminFlights/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var success = await _flightService.DeleteFlightAsync(id);
//            if (success) TempData["SuccessMessage"] = "Flight deleted successfully.";
//            else TempData["ErrorMessage"] = "Error deleting flight.";
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
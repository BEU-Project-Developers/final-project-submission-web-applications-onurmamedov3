using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminFlightsController : Controller
    {
        private readonly IFlightService _flightService;
        private readonly IUserService _userService; // Optional: for assigning User to Flight
        private readonly ILogger<AdminFlightsController> _logger;

        public AdminFlightsController(IFlightService flightService, IUserService userService, ILogger<AdminFlightsController> logger)
        {
            _flightService = flightService ?? throw new ArgumentNullException(nameof(flightService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: AdminFlights
        public async Task<IActionResult> Index()
        {
            var flights = await _flightService.GetAllFlightsAsync();
            return View(flights); // View expects IEnumerable<Flight>
        }

        // GET: AdminFlights/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var flightEntity = await _flightService.GetFlightByIdAsync(id.Value);
            if (flightEntity == null)
            {
                TempData["ErrorMessage"] = "Flight not found.";
                return NotFound();
            }

            var model = new AdminFlightViewModel
            {
                Id = flightEntity.Id,
                Airline = flightEntity.Airline,
                FlightNumber = flightEntity.FlightNumber,
                DepartureCity = flightEntity.DepartureCity,
                ArrivalCity = flightEntity.ArrivalCity,
                DepartureTime = flightEntity.DepartureTime,
                ArrivalTime = flightEntity.ArrivalTime,
                Price = flightEntity.Price,
                AirlineLogoUrl = flightEntity.AirlineLogoUrl,
                UserId = flightEntity.UserId
            };
            return View(model); // View expects AdminFlightViewModel
        }

        // GET: AdminFlights/Create
        public IActionResult Create()
        {
            // Example: Populate users for dropdown if UserId is to be selected
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName");
            return View(new AdminFlightViewModel());
        }

        // POST: AdminFlights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminFlightViewModel model)
        {
            // Custom validation for ArrivalTime > DepartureTime (also in ViewModel's Validate method)
            if (model.ArrivalTime <= model.DepartureTime)
            {
                ModelState.AddModelError("ArrivalTime", "Arrival time must be after departure time.");
            }

            if (ModelState.IsValid)
            {
                int? currentAdminUserId = GetCurrentUserId();
                var (success, createdFlight, errorMessage) = await _flightService.CreateFlightAsync(model, model.UserId ?? currentAdminUserId);
                if (success && createdFlight != null)
                {
                    TempData["SuccessMessage"] = $"Flight {createdFlight.Airline} {createdFlight.FlightNumber} created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                _logger.LogWarning("Flight creation failed. Error: {Error}", errorMessage);
                ModelState.AddModelError(string.Empty, errorMessage ?? "An unknown error occurred during creation.");
            }
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
            return View(model);
        }

        // GET: AdminFlights/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var flightEntity = await _flightService.GetFlightByIdAsync(id.Value);
            if (flightEntity == null)
            {
                TempData["ErrorMessage"] = "Flight not found.";
                return NotFound();
            }

            var model = new AdminFlightViewModel
            {
                Id = flightEntity.Id,
                Airline = flightEntity.Airline,
                FlightNumber = flightEntity.FlightNumber,
                DepartureCity = flightEntity.DepartureCity,
                ArrivalCity = flightEntity.ArrivalCity,
                DepartureTime = flightEntity.DepartureTime,
                ArrivalTime = flightEntity.ArrivalTime,
                Price = flightEntity.Price,
                AirlineLogoUrl = flightEntity.AirlineLogoUrl,
                UserId = flightEntity.UserId
            };
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
            return View(model);
        }

        // POST: AdminFlights/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminFlightViewModel model)
        {
            if (id != model.Id) return BadRequest("ID mismatch.");

            if (model.ArrivalTime <= model.DepartureTime)
            {
                ModelState.AddModelError("ArrivalTime", "Arrival time must be after departure time.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var (success, errorMessage) = await _flightService.UpdateFlightAsync(model);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Flight updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    _logger.LogWarning("Flight update failed for ID {FlightId}. Error: {Error}", model.Id, errorMessage);
                    ModelState.AddModelError(string.Empty, errorMessage ?? "An unknown error occurred during update.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogWarning(ex, "Concurrency error editing Flight ID {FlightId}", model.Id);
                    if (await _flightService.GetFlightByIdAsync(model.Id) == null)
                    {
                        TempData["ErrorMessage"] = "Flight not found (possibly deleted).";
                        return NotFound();
                    }
                    else { ModelState.AddModelError(string.Empty, "The record was modified by another user. Please reload."); }
                }
            }
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
            return View(model);
        }

        // GET: AdminFlights/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var flight = await _flightService.GetFlightByIdAsync(id.Value); // Gets Flight entity
            if (flight == null)
            {
                TempData["ErrorMessage"] = "Flight not found.";
                return NotFound();
            }
            return View(flight); // Delete view expects Flight entity
        }

        // POST: AdminFlights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _flightService.DeleteFlightAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Flight deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting flight. It might be in use or no longer exist.";
            }
            return RedirectToAction(nameof(Index));
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}
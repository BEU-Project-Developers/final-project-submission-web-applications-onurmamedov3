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
    public class AdminTripsController : Controller
    {
        private readonly ITripService _tripService;
        private readonly IUserService _userService; // Optional, for assigning user
        private readonly ILogger<AdminTripsController> _logger;

        public AdminTripsController(ITripService tripService, IUserService userService, ILogger<AdminTripsController> logger)
        {
            _tripService = tripService ?? throw new ArgumentNullException(nameof(tripService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: AdminTrips
        public async Task<IActionResult> Index()
        {
            var trips = await _tripService.GetAllTripsAsync();
            return View(trips); // View expects IEnumerable<Trip>
        }

        // GET: AdminTrips/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var tripEntity = await _tripService.GetTripByIdAsync(id.Value);
            if (tripEntity == null)
            {
                TempData["ErrorMessage"] = "Trip not found.";
                return NotFound();
            }

            var model = new AdminTripViewModel // Map from Trip entity to ViewModel
            {
                Id = tripEntity.Id,
                Title = tripEntity.Title,
                Destination = tripEntity.Destination,
                DurationDays = tripEntity.DurationDays,
                Price = tripEntity.Price,
                Description = tripEntity.Description,
                ImageUrl = tripEntity.ImageUrl,
                UserId = tripEntity.UserId
            };
            return View(model); // Details view expects AdminTripViewModel
        }

        // GET: AdminTrips/Create
        public IActionResult Create()
        {
            // If you wanted to allow selecting a user from a dropdown:
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName");
            var viewModel = new AdminTripViewModel();
            // Optionally pre-fill UserId if it should default to current admin and not be on form
            // var currentAdminId = GetCurrentUserId();
            // if (currentAdminId.HasValue) { viewModel.UserId = currentAdminId.Value; }
            return View(viewModel);
        }

        // POST: AdminTrips/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminTripViewModel model)
        {
            if (ModelState.IsValid)
            {
                // AdminTripViewModel.UserId is required, so use it directly.
                // creatingUserId is a fallback if ViewModel's UserId were nullable.
                int? currentAdminUserIdForAudit = GetCurrentUserId(); // Could be used for logging/auditing if needed
                var (success, createdTrip, errorMessage) = await _tripService.CreateTripAsync(model, model.UserId);

                if (success && createdTrip != null)
                {
                    TempData["SuccessMessage"] = $"Trip '{createdTrip.Title}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                _logger.LogWarning("Trip creation failed. Error: {Error}", errorMessage);
                ModelState.AddModelError(string.Empty, errorMessage ?? "An unknown error occurred during creation.");
            }
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
            return View(model);
        }

        // GET: AdminTrips/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var tripEntity = await _tripService.GetTripByIdAsync(id.Value);
            if (tripEntity == null)
            {
                TempData["ErrorMessage"] = "Trip not found.";
                return NotFound();
            }

            var model = new AdminTripViewModel
            {
                Id = tripEntity.Id,
                Title = tripEntity.Title,
                Destination = tripEntity.Destination,
                DurationDays = tripEntity.DurationDays,
                Price = tripEntity.Price,
                Description = tripEntity.Description,
                ImageUrl = tripEntity.ImageUrl,
                UserId = tripEntity.UserId
            };
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
            return View(model);
        }

        // POST: AdminTrips/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminTripViewModel model)
        {
            if (id != model.Id) return BadRequest("ID mismatch.");

            if (ModelState.IsValid)
            {
                try
                {
                    var (success, errorMessage) = await _tripService.UpdateTripAsync(model);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Trip updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    _logger.LogWarning("Trip update failed for ID {TripId}. Error: {Error}", model.Id, errorMessage);
                    ModelState.AddModelError(string.Empty, errorMessage ?? "An unknown error occurred during update.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogWarning(ex, "Concurrency error editing Trip ID {TripId}", model.Id);
                    if (await _tripService.GetTripByIdAsync(model.Id) == null)
                    {
                        TempData["ErrorMessage"] = "Trip not found (possibly deleted).";
                        return NotFound();
                    }
                    else { ModelState.AddModelError(string.Empty, "The record was modified by another user. Please reload."); }
                }
            }
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
            return View(model);
        }

        // GET: AdminTrips/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var trip = await _tripService.GetTripByIdAsync(id.Value); // Gets Trip entity
            if (trip == null)
            {
                TempData["ErrorMessage"] = "Trip not found.";
                return NotFound();
            }
            return View(trip); // Delete view expects Trip entity
        }

        // POST: AdminTrips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _tripService.DeleteTripAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Trip deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting trip. It might be in use or no longer exist.";
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
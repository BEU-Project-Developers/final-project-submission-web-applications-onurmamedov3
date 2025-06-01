using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TourManagementSystem.Models; // For Cruise entity and AdminCruiseViewModel
using TourManagementSystem.Services;

namespace TourManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminCruisesController : Controller
    {
        private readonly ICruiseService _cruiseService;
        private readonly IUserService _userService; // Optional for assigning User
        private readonly ILogger<AdminCruisesController> _logger;

        public AdminCruisesController(ICruiseService cruiseService, IUserService userService, ILogger<AdminCruisesController> logger)
        {
            _cruiseService = cruiseService ?? throw new ArgumentNullException(nameof(cruiseService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: AdminCruises
        public async Task<IActionResult> Index()
        {
            var cruises = await _cruiseService.GetAllCruisesAsync();
            return View(cruises); // View expects IEnumerable<Cruise>
        }

        // GET: AdminCruises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var cruiseEntity = await _cruiseService.GetCruiseByIdAsync(id.Value);
            if (cruiseEntity == null)
            {
                TempData["ErrorMessage"] = "Cruise not found.";
                return NotFound();
            }

            var model = new AdminCruiseViewModel
            {
                Id = cruiseEntity.Id,
                CruiseLine = cruiseEntity.CruiseLine,
                DeparturePort = cruiseEntity.DeparturePort,
                Destination = cruiseEntity.Destination,
                DurationDays = cruiseEntity.DurationDays,
                Price = cruiseEntity.Price,
                ImageUrl = cruiseEntity.ImageUrl,
                UserId = cruiseEntity.UserId,
                // ItinerarySummary is not in Cruise entity, so it won't be populated from there
                // If you want to display something here, it needs a source or be generated.
                ItinerarySummary = $"Details for {cruiseEntity.CruiseLine} to {cruiseEntity.Destination}" // Example generated summary
            };
            return View(model); // Details view expects AdminCruiseViewModel
        }

        // GET: AdminCruises/Create
        public IActionResult Create()
        {
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName");
            var viewModel = new AdminCruiseViewModel();
            // var currentAdminId = GetCurrentUserId();
            // if (currentAdminId.HasValue) { viewModel.UserId = currentAdminId.Value; } // Optional pre-fill
            return View(viewModel);
        }

        // POST: AdminCruises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCruiseViewModel model)
        {
            if (ModelState.IsValid)
            {
                int? currentAdminUserIdForAudit = GetCurrentUserId();
                var (success, createdCruise, errorMessage) = await _cruiseService.CreateCruiseAsync(model, model.UserId);

                if (success && createdCruise != null)
                {
                    TempData["SuccessMessage"] = $"Cruise '{createdCruise.CruiseLine} - {createdCruise.Destination}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                _logger.LogWarning("Cruise creation failed. Error: {Error}", errorMessage);
                ModelState.AddModelError(string.Empty, errorMessage ?? "An unknown error occurred during creation.");
            }
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
            return View(model);
        }

        // GET: AdminCruises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var cruiseEntity = await _cruiseService.GetCruiseByIdAsync(id.Value);
            if (cruiseEntity == null)
            {
                TempData["ErrorMessage"] = "Cruise not found.";
                return NotFound();
            }

            var model = new AdminCruiseViewModel
            {
                Id = cruiseEntity.Id,
                CruiseLine = cruiseEntity.CruiseLine,
                DeparturePort = cruiseEntity.DeparturePort,
                Destination = cruiseEntity.Destination,
                DurationDays = cruiseEntity.DurationDays,
                Price = cruiseEntity.Price,
                ImageUrl = cruiseEntity.ImageUrl,
                UserId = cruiseEntity.UserId
                // ItinerarySummary from ViewModel will be empty here unless you manually fetch/set it
                // or if the form field for ItinerarySummary was intended to be edited independently.
            };
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
            return View(model);
        }

        // POST: AdminCruises/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminCruiseViewModel model)
        {
            if (id != model.Id) return BadRequest("ID mismatch.");

            if (ModelState.IsValid)
            {
                try
                {
                    // Note: UpdateCruiseAsync will not update ItinerarySummary in the DB
                    // unless Cruise entity has such a field and service maps it.
                    var (success, errorMessage) = await _cruiseService.UpdateCruiseAsync(model);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Cruise updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    _logger.LogWarning("Cruise update failed for ID {CruiseId}. Error: {Error}", model.Id, errorMessage);
                    ModelState.AddModelError(string.Empty, errorMessage ?? "An unknown error occurred during update.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogWarning(ex, "Concurrency error editing Cruise ID {CruiseId}", model.Id);
                    if (await _cruiseService.GetCruiseByIdAsync(model.Id) == null)
                    {
                        TempData["ErrorMessage"] = "Cruise not found (possibly deleted).";
                        return NotFound();
                    }
                    else { ModelState.AddModelError(string.Empty, "The record was modified by another user. Please reload."); }
                }
            }
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
            return View(model);
        }

        // GET: AdminCruises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var cruise = await _cruiseService.GetCruiseByIdAsync(id.Value); // Gets Cruise entity
            if (cruise == null)
            {
                TempData["ErrorMessage"] = "Cruise not found.";
                return NotFound();
            }
            return View(cruise); // Delete view expects Cruise entity
        }

        // POST: AdminCruises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _cruiseService.DeleteCruiseAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Cruise deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting cruise. It might be in use or no longer exist.";
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
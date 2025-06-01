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
    public class AdminCarsController : Controller
    {
        private readonly ICarRentalService _carRentalService;
        private readonly IUserService _userService; // Kept for GetCurrentUserId, ensure it's registered
        private readonly ILogger<AdminCarsController> _logger;

        public AdminCarsController(
            ICarRentalService carRentalService,
            IUserService userService,
            ILogger<AdminCarsController> logger)
        {
            _carRentalService = carRentalService ?? throw new ArgumentNullException(nameof(carRentalService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            var carRentals = await _carRentalService.GetAllCarRentalsAsync();
            return View(carRentals); // View expects IEnumerable<CarRental>
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var carRentalEntity = await _carRentalService.GetCarRentalByIdAsync(id.Value);
            if (carRentalEntity == null) return NotFound();

            var viewModel = new AdminCarRentalViewModel // Map from simplified entity
            {
                Id = carRentalEntity.Id,
                CarModel = carRentalEntity.CarModel,
                Company = carRentalEntity.Company,
                Location = carRentalEntity.Location,
                PricePerDay = carRentalEntity.PricePerDay,
                ImageUrl = carRentalEntity.ImageUrl,
                UserId = carRentalEntity.UserId
            };
            return View(viewModel); // View expects AdminCarRentalViewModel
        }

        public IActionResult Create()
        {
            // If AdminCarRentalViewModel.UserId is required, you might want to pre-fill it here
            // with GetCurrentUserId() if that's the desired default, or ensure the form handles it.
            // For now, the ViewModel itself has UserId as required.
            var viewModel = new AdminCarRentalViewModel();
            // Optionally pre-fill UserId if it should default to current admin and not be on form
            // var currentAdminId = GetCurrentUserId();
            // if (currentAdminId.HasValue) { viewModel.UserId = currentAdminId.Value; }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCarRentalViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Since AdminCarRentalViewModel.UserId is now a required int,
                // creatingUserId parameter in service is less critical as a fallback unless you change ViewModel
                var (success, createdCarRental, errorMessage) = await _carRentalService.CreateCarRentalAsync(model, model.UserId);

                if (success && createdCarRental != null)
                {
                    TempData["SuccessMessage"] = $"Car Rental '{createdCarRental.CarModel}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, errorMessage ?? "Creation failed.");
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var carRentalEntity = await _carRentalService.GetCarRentalByIdAsync(id.Value);
            if (carRentalEntity == null) return NotFound();

            var viewModel = new AdminCarRentalViewModel // Map from simplified entity
            {
                Id = carRentalEntity.Id,
                CarModel = carRentalEntity.CarModel,
                Company = carRentalEntity.Company,
                Location = carRentalEntity.Location,
                PricePerDay = carRentalEntity.PricePerDay,
                ImageUrl = carRentalEntity.ImageUrl,
                UserId = carRentalEntity.UserId
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminCarRentalViewModel model)
        {
            if (id != model.Id) return BadRequest("ID mismatch.");
            if (ModelState.IsValid)
            {
                try
                {
                    var (success, errorMessage) = await _carRentalService.UpdateCarRentalAsync(model);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Car Rental updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError(string.Empty, errorMessage ?? "Update failed.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogWarning(ex, "Concurrency error editing Car Rental ID {Id}", model.Id);
                    if (await _carRentalService.GetCarRentalByIdAsync(model.Id) == null) return NotFound();
                    else ModelState.AddModelError(string.Empty, "Record modified. Reload and try again.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var carRental = await _carRentalService.GetCarRentalByIdAsync(id.Value);
            if (carRental == null) return NotFound();
            return View(carRental); // Delete view expects the CarRental entity
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _carRentalService.DeleteCarRentalAsync(id);
            if (success) TempData["SuccessMessage"] = "Car Rental deleted successfully.";
            else TempData["ErrorMessage"] = "Error deleting car rental.";
            return RedirectToAction(nameof(Index));
        }

        // Kept for potential auditing or if UserId logic in ViewModel changes
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
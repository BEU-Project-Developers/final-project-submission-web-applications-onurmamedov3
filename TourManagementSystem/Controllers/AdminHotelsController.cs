// File: TourManagementSystem/Controllers/AdminHotelsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TourManagementSystem.Models;    // For Hotel entity
using TourManagementSystem.Services;   // For IHotelService, IUserService


namespace TourManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminHotelsController : Controller
    {
        private readonly IHotelService _hotelService;
        private readonly IUserService _userService;
        private readonly ILogger<AdminHotelsController> _logger;

        public AdminHotelsController(IHotelService hotelService, IUserService userService, ILogger<AdminHotelsController> logger)
        {
            _hotelService = hotelService;
            _userService = userService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var hotels = await _hotelService.GetAllHotelsAsync();
            return View(hotels);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var hotelEntity = await _hotelService.GetHotelByIdAsync(id.Value);
            if (hotelEntity == null) return NotFound();
            // Map to AdminHotelViewModel for consistency if your Details view expects it.
            // If Details.cshtml expects Hotel entity, you can pass hotelEntity directly.
            // For this example, assuming Details view takes AdminHotelViewModel for consistency.
            var viewModel = new AdminHotelViewModel
            {
                Id = hotelEntity.Id,
                Name = hotelEntity.Name,
                Destination = hotelEntity.Destination,
                Address = hotelEntity.Address,
                Description = hotelEntity.Description,
                PricePerNight = hotelEntity.PricePerNight,
                Rating = hotelEntity.Rating,
                AvailableRooms = hotelEntity.AvailableRooms,
                PrimaryImageUrl = hotelEntity.PrimaryImageUrl,
                UserId = hotelEntity.UserId
            };
            return View(viewModel);
        }

        public IActionResult Create() // Can be async if you populate ViewBag.UserList
        {
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName");
            return View(new AdminHotelViewModel()); // Pass AdminHotelViewModel
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminHotelViewModel model) // Now takes AdminHotelViewModel
        {
            if (ModelState.IsValid)
            {
                int? creatingUserId = User.Identity?.IsAuthenticated == true && int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int uid) ? uid : (int?)null;

                // Line 84 in your error screenshot
                var result = await _hotelService.CreateHotelAsync(model, model.UserId ?? creatingUserId);
                if (result.Success && result.CreatedHotel != null)
                {
                    TempData["SuccessMessage"] = $"Hotel '{result.CreatedHotel.Name}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Creation failed.");
            }
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var hotelEntity = await _hotelService.GetHotelByIdAsync(id.Value);
            if (hotelEntity == null) return NotFound();
            var model = new AdminHotelViewModel // Map to AdminHotelViewModel
            {
                Id = hotelEntity.Id,
                Name = hotelEntity.Name,
                Destination = hotelEntity.Destination,
                Address = hotelEntity.Address,
                Description = hotelEntity.Description,
                PricePerNight = hotelEntity.PricePerNight,
                Rating = hotelEntity.Rating,
                AvailableRooms = hotelEntity.AvailableRooms,
                PrimaryImageUrl = hotelEntity.PrimaryImageUrl,
                UserId = hotelEntity.UserId
            };
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminHotelViewModel model) // Now takes AdminHotelViewModel
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    // Line 132 in your error screenshot
                    // UpdateHotelAsync in service now takes AdminHotelViewModel
                    var result = await _hotelService.UpdateHotelAsync(model);
                    if (result.Success)
                    {
                        TempData["SuccessMessage"] = "Hotel updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Update failed.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _hotelService.GetHotelByIdAsync(model.Id) == null) return NotFound();
                    else { ModelState.AddModelError(string.Empty, "The record was modified by another user."); }
                }
            }
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var hotel = await _hotelService.GetHotelByIdAsync(id.Value);
            if (hotel == null) return NotFound();
            return View(hotel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _hotelService.DeleteHotelAsync(id);
            if (success) TempData["SuccessMessage"] = "Hotel deleted successfully.";
            else TempData["ErrorMessage"] = "Error deleting hotel.";
            return RedirectToAction(nameof(Index));
        }
    }
}
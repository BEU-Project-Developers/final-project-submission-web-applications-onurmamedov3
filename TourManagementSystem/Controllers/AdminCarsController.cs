//// File: TourManagementSystem/Controllers/AdminCarsController.cs
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using TourManagementSystem.Models;
//using TourManagementSystem.Services;

//namespace TourManagementSystem.Controllers
//{
//    [Authorize(Roles = "Admin")]
//    public class AdminCarsController : Controller
//    {
//        private readonly ICarRentalService _CarRentalService;
//        private readonly IUserService _userService; // Optional
//        private readonly ILogger<AdminCarsController> _logger;

//        public AdminCarsController(ICarRentalService CarRentalService, IUserService userService, ILogger<AdminCarsController> logger)
//        {
//            _CarRentalService = CarRentalService;
//            _userService = userService;
//            _logger = logger;
//        }
        
//        // GET: AdminCars
//        public async Task<IActionResult> Index()
//        {
//            ViewData["AdminPageTitle"] = "Manage Car Rentals";
//            var CarRentals = await _CarRentalService.GetAllCarRentalsAsync(); // Returns IEnumerable<CarRental>
//            return View(CarRentals);
//        }

//        // GET: AdminCars/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null) return NotFound();
//            var CarRentalEntity = await _CarRentalService.GetCarRentalByIdAsync(id.Value);
//            if (CarRentalEntity == null) return NotFound();

//            // Map to AdminCarRentalViewModel for display, using only fields present in the simple CarRental entity
//            var model = new AdminCarRentalViewModel
//            {
//                Id = CarRentalEntity.Id,
//                CarModel = CarRentalEntity.CarModel,
//                Company = CarRentalEntity.Company,
//                PricePerDay = CarRentalEntity.PricePerDay,
//                Location = CarRentalEntity.Location,
//                ImageUrl = CarRentalEntity.ImageUrl,
//                UserId = CarRentalEntity.UserId
//                // Fields like Make, Model (separate), Year, Description, PassengerCapacity, IsAvailable
//                // are not mapped because they are not in your simple CarRental entity.
//            };
//            ViewData["AdminPageTitle"] = $"Details: {model.CarModel}";
//            return View(model);
//        }

//        // GET: AdminCars/Create
//        public async Task<IActionResult> Create()
//        {
//            ViewData["AdminPageTitle"] = "Add New Car Rental";
//            // Example if populating User dropdown
//            // var users = await _userService.GetAllActiveUsersAsync();
//            // ViewBag.UserList = new SelectList(users, "Id", "FullName");
//            return View(new AdminCarRentalViewModel()); // Use simplified AdminCarRentalViewModel
//        }

//        // POST: AdminCars/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(AdminCarRentalViewModel model) // Takes simplified AdminCarRentalViewModel
//        {
//            if (ModelState.IsValid)
//            {
//                int? currentAdminUserId = null;
//                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
//                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int adminId))
//                {
//                    currentAdminUserId = adminId;
//                }

//                var (success, createdCar, errorMessage) = await _CarRentalService.CreateCarRentalAsync(model, model.UserId ?? currentAdminUserId);
//                if (success && createdCar != null)
//                {
//                    TempData["SuccessMessage"] = $"Car Rental '{createdCar.CarModel}' created successfully!";
//                    return RedirectToAction(nameof(Index));
//                }
//                ModelState.AddModelError(string.Empty, errorMessage ?? "Creation failed.");
//            }
//            // var users = await _userService.GetAllActiveUsersAsync();
//            // ViewBag.UserList = new SelectList(users, "Id", "FullName", model.UserId);
//            ViewData["AdminPageTitle"] = "Add New Car Rental - Errors";
//            return View(model);
//        }

//        // GET: AdminCars/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null) return NotFound();
//            var CarRentalEntity = await _CarRentalService.GetCarRentalByIdAsync(id.Value);
//            if (CarRentalEntity == null) return NotFound();

//            var model = new AdminCarRentalViewModel // Map from simple CarRental entity
//            {
//                Id = CarRentalEntity.Id,
//                CarModel = CarRentalEntity.CarModel,
//                Company = CarRentalEntity.Company,
//                PricePerDay = CarRentalEntity.PricePerDay,
//                Location = CarRentalEntity.Location,
//                ImageUrl = CarRentalEntity.ImageUrl,
//                UserId = CarRentalEntity.UserId
//            };
//            ViewData["AdminPageTitle"] = $"Edit Car Rental: {model.CarModel}";
//            // var users = await _userService.GetAllActiveUsersAsync();
//            // ViewBag.UserList = new SelectList(users, "Id", "FullName", model.UserId);
//            return View(model);
//        }

//        // POST: AdminCars/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, AdminCarRentalViewModel model) // Takes simplified AdminCarRentalViewModel
//        {
//            if (id != model.Id) return NotFound();

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    var (success, errorMessage) = await _CarRentalService.UpdateCarRentalAsync(model);
//                    if (success)
//                    {
//                        TempData["SuccessMessage"] = "Car Rental updated successfully!";
//                        return RedirectToAction(nameof(Index));
//                    }
//                    ModelState.AddModelError(string.Empty, errorMessage ?? "Update failed.");
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (await _CarRentalService.GetCarRentalByIdAsync(model.Id) == null) return NotFound();
//                    else { ModelState.AddModelError(string.Empty, "The record was modified. Please reload."); }
//                }
//            }
//            ViewData["AdminPageTitle"] = $"Edit Car Rental - Errors: {model.CarModel}";
//            // var users = await _userService.GetAllActiveUsersAsync();
//            // ViewBag.UserList = new SelectList(users, "Id", "FullName", model.UserId);
//            return View(model);
//        }

//        // GET: AdminCars/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null) return NotFound();
//            var CarRental = await _CarRentalService.GetCarRentalByIdAsync(id.Value);
//            if (CarRental == null) return NotFound();
//            ViewData["AdminPageTitle"] = $"Confirm Delete: {CarRental.CarModel}";
//            return View(CarRental); // Pass CarRental entity
//        }

//        // POST: AdminCars/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var success = await _CarRentalService.DeleteCarRentalAsync(id);
//            if (success) TempData["SuccessMessage"] = "Car Rental deleted successfully.";
//            else TempData["ErrorMessage"] = "Error deleting car rental.";
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
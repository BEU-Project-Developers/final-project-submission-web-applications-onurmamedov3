using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TourManagementSystem.Models; // For Activity entity and AdminActivityViewModel
using TourManagementSystem.Services;

namespace TourManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminActivitiesController : Controller
    {
        private readonly IActivityService _activityService;
        private readonly IUserService _userService; // Optional
        private readonly ILogger<AdminActivitiesController> _logger;

        public AdminActivitiesController(IActivityService activityService, IUserService userService, ILogger<AdminActivitiesController> logger)
        {
            _activityService = activityService ?? throw new ArgumentNullException(nameof(activityService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: AdminActivities
        public async Task<IActionResult> Index()
        {
            var activities = await _activityService.GetAllActivitiesAsync();
            return View(activities); // View expects IEnumerable<Activity>
        }

        // GET: AdminActivities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var activityEntity = await _activityService.GetActivityByIdAsync(id.Value);
            if (activityEntity == null)
            {
                TempData["ErrorMessage"] = "Activity not found.";
                return NotFound();
            }

            var model = new AdminActivityViewModel
            {
                Id = activityEntity.Id,
                Name = activityEntity.Name,
                Location = activityEntity.Location,
                Category = activityEntity.Category,
                DurationHours = activityEntity.DurationHours,
                Price = activityEntity.Price,
                Description = activityEntity.Description,
                ImageUrl = activityEntity.ImageUrl,
                UserId = activityEntity.UserId
            };
            return View(model); // View expects AdminActivityViewModel
        }

        // GET: AdminActivities/Create
        public IActionResult Create()
        {
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName");
            return View(new AdminActivityViewModel());
        }

        // POST: AdminActivities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminActivityViewModel model)
        {
            if (ModelState.IsValid)
            {
                int? currentAdminUserId = GetCurrentUserId();
                var (success, createdActivity, errorMessage) = await _activityService.CreateActivityAsync(model, model.UserId ?? currentAdminUserId);

                if (success && createdActivity != null)
                {
                    TempData["SuccessMessage"] = $"Activity '{createdActivity.Name}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                _logger.LogWarning("Activity creation failed. Error: {Error}", errorMessage);
                ModelState.AddModelError(string.Empty, errorMessage ?? "An unknown error occurred during creation.");
            }
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
            return View(model);
        }

        // GET: AdminActivities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var activityEntity = await _activityService.GetActivityByIdAsync(id.Value);
            if (activityEntity == null)
            {
                TempData["ErrorMessage"] = "Activity not found.";
                return NotFound();
            }

            var model = new AdminActivityViewModel
            {
                Id = activityEntity.Id,
                Name = activityEntity.Name,
                Location = activityEntity.Location,
                Category = activityEntity.Category,
                DurationHours = activityEntity.DurationHours,
                Price = activityEntity.Price,
                Description = activityEntity.Description,
                ImageUrl = activityEntity.ImageUrl,
                UserId = activityEntity.UserId
            };
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
            return View(model);
        }

        // POST: AdminActivities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminActivityViewModel model)
        {
            if (id != model.Id) return BadRequest("ID mismatch.");

            if (ModelState.IsValid)
            {
                try
                {
                    var (success, errorMessage) = await _activityService.UpdateActivityAsync(model);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Activity updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    _logger.LogWarning("Activity update failed for ID {ActivityId}. Error: {Error}", model.Id, errorMessage);
                    ModelState.AddModelError(string.Empty, errorMessage ?? "An unknown error occurred during update.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogWarning(ex, "Concurrency error editing Activity ID {ActivityId}", model.Id);
                    if (await _activityService.GetActivityByIdAsync(model.Id) == null)
                    {
                        TempData["ErrorMessage"] = "Activity not found (possibly deleted).";
                        return NotFound();
                    }
                    else { ModelState.AddModelError(string.Empty, "The record was modified by another user. Please reload."); }
                }
            }
            // ViewBag.UserList = new SelectList(await _userService.GetAllActiveUsersAsync(), "Id", "FullName", model.UserId);
            return View(model);
        }

        // GET: AdminActivities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var activity = await _activityService.GetActivityByIdAsync(id.Value); // Gets Activity entity
            if (activity == null)
            {
                TempData["ErrorMessage"] = "Activity not found.";
                return NotFound();
            }
            return View(activity); // Delete view expects Activity entity
        }

        // POST: AdminActivities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _activityService.DeleteActivityAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Activity deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting activity. It might be in use or no longer exist.";
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
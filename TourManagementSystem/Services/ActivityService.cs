using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourManagementSystem.Data;
using TourManagementSystem.Models; // For Activity entity and AdminActivityViewModel

namespace TourManagementSystem.Services
{
    public class ActivityService : IActivityService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(ApplicationDbContext context, ILogger<ActivityService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Activity>> GetAllActivitiesAsync()
        {
            try
            {
                return await _context.Activities
                    .AsNoTracking()
                    .OrderBy(a => a.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all activities.");
                return new List<Activity>();
            }
        }

        public async Task<Activity?> GetActivityByIdAsync(int id)
        {
            try
            {
                return await _context.Activities
                    .Include(a => a.User) // Eager load User
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching activity with ID {ActivityId}", id);
                return null;
            }
        }

        public async Task<int> GetTotalActivitiesCountAsync()
        {
            try
            {
                return await _context.Activities.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total activities count.");
                return 0;
            }
        }

        public async Task<IEnumerable<Activity>> SearchActivitiesAsync(
            string? searchTerm, string? category, DateTime? activityDate,
            int? maxDurationHours, decimal? maxPrice)
        {
            try
            {
                _logger.LogInformation("Searching activities. Term: {Term}, Cat: {Cat}, Date: {Date}", searchTerm, category, activityDate?.ToShortDateString());
                var query = _context.Activities.AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(a =>
                        (a.Name != null && EF.Functions.Like(a.Name, $"%{searchTerm}%")) ||
                        (a.Location != null && EF.Functions.Like(a.Location, $"%{searchTerm}%"))
                    );
                }
                if (!string.IsNullOrWhiteSpace(category))
                {
                    query = query.Where(a => a.Category != null && EF.Functions.Like(a.Category, $"%{category}%"));
                }
                if (activityDate.HasValue)
                {
                    _logger.LogInformation("Activity search for date {ActivityDate}. Complex date/time slot availability not implemented.", activityDate.Value.ToShortDateString());
                }
                if (maxDurationHours.HasValue)
                {
                    query = query.Where(a => a.DurationHours <= maxDurationHours.Value);
                }
                if (maxPrice.HasValue)
                {
                    query = query.Where(a => a.Price <= maxPrice.Value);
                }

                return await query.AsNoTracking().OrderBy(a => a.Name).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching activities.");
                return new List<Activity>();
            }
        }

        public async Task<(bool Success, Activity? CreatedActivity, string? ErrorMessage)> CreateActivityAsync(AdminActivityViewModel model, int? creatingUserId)
        {
            if (model == null) return (false, null, "Activity data model cannot be null.");

            var activityEntity = new Activity
            {
                Name = model.Name,
                Location = model.Location,
                Category = model.Category,
                DurationHours = model.DurationHours, // Non-nullable in entity & VM
                Price = model.Price,                 // Non-nullable in entity & VM
                Description = model.Description,
                ImageUrl = model.ImageUrl,           // Non-nullable in entity & VM
                UserId = model.UserId ?? creatingUserId // UserId is nullable in entity
            };

            try
            {
                _context.Activities.Add(activityEntity);
                await _context.SaveChangesAsync();
                return (true, activityEntity, "Activity created successfully.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DB error creating activity '{Name}'. Inner: {InnerEx}", model.Name, ex.InnerException?.Message);
                return (false, null, $"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating activity '{Name}'.", model.Name);
                return (false, null, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateActivityAsync(AdminActivityViewModel model)
        {
            if (model == null) return (false, "Activity data model cannot be null.");

            var existingActivity = await _context.Activities.FindAsync(model.Id);
            if (existingActivity == null) return (false, "Activity not found for update.");

            existingActivity.Name = model.Name;
            existingActivity.Location = model.Location;
            existingActivity.Category = model.Category;
            existingActivity.DurationHours = model.DurationHours;
            existingActivity.Price = model.Price;
            existingActivity.Description = model.Description;
            existingActivity.ImageUrl = model.ImageUrl;
            existingActivity.UserId = model.UserId;

            try
            {
                _context.Entry(existingActivity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return (true, "Activity updated successfully.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency error updating activity ID {Id}", model.Id);
                return (false, "Record modified. Refresh and try again.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DB error updating activity ID {Id}. Inner: {InnerEx}", model.Id, ex.InnerException?.Message);
                return (false, $"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating activity ID {Id}", model.Id);
                return (false, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<bool> DeleteActivityAsync(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                _logger.LogWarning("Attempt to delete non-existent activity ID {Id}.", id);
                return false;
            }
            try
            {
                _context.Activities.Remove(activity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting activity ID {Id}.", id);
                return false;
            }
        }
    }
}
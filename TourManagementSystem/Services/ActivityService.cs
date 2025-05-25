// File: TourManagementSystem/Services/ActivityService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourManagementSystem.Data;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public class ActivityService : IActivityService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(ApplicationDbContext context, ILogger<ActivityService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Activity?> GetActivityByIdAsync(int id)
        {
            try
            {
                return await _context.Activities.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching activity with ID {ActivityId}", id);
                return null;
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
                    // This is a simplification. Real activity availability is date/time specific
                    // and might involve schedules or booking slots.
                    _logger.LogInformation("Activity search for date {ActivityDate}. Complex date/time slot availability not implemented.", activityDate.Value.ToShortDateString());
                    // query = query.Where(a => a.AvailableDates.Any(d => d.Date == activityDate.Value.Date)); // Example if you had schedules
                }
                if (maxDurationHours.HasValue)
                {
                    query = query.Where(a => a.DurationHours <= maxDurationHours.Value);
                }
                if (maxPrice.HasValue)
                {
                    query = query.Where(a => a.Price <= maxPrice.Value);
                }

                return await query.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching activities.");
                return new List<Activity>();
            }
        }
    }
}
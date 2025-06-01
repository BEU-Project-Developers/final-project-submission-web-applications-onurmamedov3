using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public interface IActivityService
    {
        // Existing search method
        Task<IEnumerable<Activity>> SearchActivitiesAsync(
            string? searchTerm,
            string? category,
            DateTime? activityDate,
            int? maxDurationHours,
            decimal? maxPrice
        );
        Task<Activity?> GetActivityByIdAsync(int id);

        // NEW Admin CRUD Methods
        Task<IEnumerable<Activity>> GetAllActivitiesAsync();
        Task<(bool Success, Activity? CreatedActivity, string? ErrorMessage)> CreateActivityAsync(AdminActivityViewModel model, int? creatingUserId);
        Task<(bool Success, string? ErrorMessage)> UpdateActivityAsync(AdminActivityViewModel model);
        Task<bool> DeleteActivityAsync(int id);
        Task<int> GetTotalActivitiesCountAsync();
    }
}
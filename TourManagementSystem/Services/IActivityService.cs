// File: TourManagementSystem/Services/IActivityService.cs
using System; // For DateTime
using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public interface IActivityService
    {
        Task<IEnumerable<Activity>> SearchActivitiesAsync(
            string? searchTerm, // Can search Name or Location
            string? category,
            DateTime? activityDate, // For specific date availability
            int? maxDurationHours,
            decimal? maxPrice
        );
        Task<Activity?> GetActivityByIdAsync(int id);
        // Add GetAll, Create, Update, Delete methods as needed
    }
}
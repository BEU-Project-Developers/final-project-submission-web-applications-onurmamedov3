using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagementSystem.Models; // For Cruise entity
// using TourManagementSystem.ViewModels; // If AdminCruiseViewModel was in a separate ViewModels folder

namespace TourManagementSystem.Services
{
    public interface ICruiseService
    {
        // Existing search method
        Task<IEnumerable<Cruise>> SearchCruisesAsync(
            string? destinationRegion,
            string? departurePort,
            string? cruiseLine,
            int? minDurationDays,
            int? maxDurationDays,
            decimal? maxPrice
        );
        Task<Cruise?> GetCruiseByIdAsync(int id);

        // NEW Admin CRUD Methods
        Task<IEnumerable<Cruise>> GetAllCruisesAsync();
        Task<(bool Success, Cruise? CreatedCruise, string? ErrorMessage)> CreateCruiseAsync(AdminCruiseViewModel model, int? creatingUserId);
        Task<(bool Success, string? ErrorMessage)> UpdateCruiseAsync(AdminCruiseViewModel model);
        Task<bool> DeleteCruiseAsync(int id);
        Task<int> GetTotalCruisesCountAsync();
    }
}
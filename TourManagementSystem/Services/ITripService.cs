using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagementSystem.Models;
 // For AdminTripViewModel

namespace TourManagementSystem.Services
{
    public interface ITripService
    {
        // Existing methods
        Task<IEnumerable<Trip>> SearchTripsAsync(string? tripSearchTerm, string? startDate, int? travelers, decimal? maxTripPrice);
        Task<Trip?> GetTripByIdAsync(int id);

        // NEW Admin CRUD Methods
        Task<IEnumerable<Trip>> GetAllTripsAsync();
        Task<(bool Success, Trip? CreatedTrip, string? ErrorMessage)> CreateTripAsync(AdminTripViewModel model, int? creatingUserId);
        Task<(bool Success, string? ErrorMessage)> UpdateTripAsync(AdminTripViewModel model);
        Task<bool> DeleteTripAsync(int id);
        Task<int> GetTotalTripsCountAsync();
    }
}
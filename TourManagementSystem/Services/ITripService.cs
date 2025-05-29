using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public interface ITripService
    {
        Task<IEnumerable<Trip>> SearchTripsAsync(string? tripSearchTerm, string? startDate, int? travelers, decimal? maxTripPrice);
        Task<Trip?> GetTripByIdAsync(int id);
        // Add Create, Update, Delete methods if needed
    }
}
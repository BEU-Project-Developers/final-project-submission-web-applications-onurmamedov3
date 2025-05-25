// File: TourManagementSystem/Services/ICruiseService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public interface ICruiseService
    {
        Task<IEnumerable<Cruise>> SearchCruisesAsync(
            string? destinationRegion, // e.g., Caribbean, Alaska, Mediterranean
            string? departurePort,
            string? cruiseLine,
            int? minDurationDays,
            int? maxDurationDays,
            decimal? maxPrice
        );
        Task<Cruise?> GetCruiseByIdAsync(int id);
        // Add GetAll, Create, Update, Delete methods as needed
    }
}
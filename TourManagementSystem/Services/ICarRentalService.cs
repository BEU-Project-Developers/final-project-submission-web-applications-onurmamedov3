using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public interface ICarRentalService
    {
        Task<IEnumerable<CarRental>> GetAllCarRentalsAsync();
        Task<CarRental?> GetCarRentalByIdAsync(int id);

        // Search method simplified to only use fields available in the simple CarRental entity
        Task<IEnumerable<CarRental>> SearchCarRentalsAsync(
            string? location,
            string? pickupDateStr, // Kept for potential future basic filtering, not for complex availability
            string? returnDateStr,  // Kept for potential future basic filtering
            string? carModelKeyword
        // carPassengerCapacity and carMaxPrice removed as entity is simple
        );

        // Admin CRUD Operations (using the simplified AdminCarRentalViewModel)
        Task<(bool Success, CarRental? CreatedCarRental, string? ErrorMessage)> CreateCarRentalAsync(AdminCarRentalViewModel model, int? creatingUserId);
        Task<(bool Success, string? ErrorMessage)> UpdateCarRentalAsync(AdminCarRentalViewModel model);
        Task<bool> DeleteCarRentalAsync(int id);
        Task<int> GetTotalCarRentalsCountAsync();
    }
}
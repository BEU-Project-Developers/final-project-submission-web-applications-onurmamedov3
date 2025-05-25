// File: TourManagementSystem/Services/ICarRentalService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public interface ICarRentalService
    {
        Task<IEnumerable<CarRental>> GetAllCarRentalsAsync();
        Task<CarRental?> GetCarRentalByIdAsync(int id);
        Task<IEnumerable<CarRental>> SearchCarRentalsAsync(
            string? location,
            string? pickupDateStr,
            string? returnDateStr,
            string? carModelKeyword // For searching by model or type
        );
        // Add Create, Update, Delete methods if admins will manage car rentals
    }
}
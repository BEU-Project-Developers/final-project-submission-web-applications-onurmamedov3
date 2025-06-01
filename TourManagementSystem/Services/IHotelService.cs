using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagementSystem.Models;      // For Hotel entity
using TourManagementSystem.ViewModels; // For AdminHotelViewModel (if it's in ViewModels namespace)
                                       // Or using TourManagementSystem.Models; // If AdminHotelViewModel is in Models namespace

namespace TourManagementSystem.Services
{
    public interface IHotelService
    {
        Task<IEnumerable<Hotel>> GetAllHotelsAsync();
        Task<Hotel?> GetHotelByIdAsync(int id);

        Task<IEnumerable<Hotel>> SearchHotelsAsync(
            string? destination, string? hotelName, string? checkInDateStr, string? checkOutDateStr,
            int? adults, int? children, int? minRating, decimal? maxPrice, List<string>? amenities);

        // --- CRUD methods use AdminHotelViewModel ---
        // creatingUserId would typically come from the currently logged-in admin user's ID.
        Task<(bool Success, Hotel? CreatedHotel, string? ErrorMessage)> CreateHotelAsync(AdminHotelViewModel model, int? creatingUserId);
        Task<(bool Success, string? ErrorMessage)> UpdateHotelAsync(AdminHotelViewModel model);
        Task<bool> DeleteHotelAsync(int id);

        Task<IEnumerable<Hotel>> GetFeaturedHotelsAsync(int count, string? category = null);
        Task<IEnumerable<Hotel>> GetRandomHotelsAsync(int count);
    }
}
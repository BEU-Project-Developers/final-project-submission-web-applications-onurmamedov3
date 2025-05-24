// File: TourManagementSystem/Services/IHotelService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public interface IHotelService
    {
        Task<IEnumerable<Hotel>> GetAllHotelsAsync();
        Task<Hotel?> GetHotelByIdAsync(int id);

        Task<IEnumerable<Hotel>> SearchHotelsAsync(
            string? searchTerm,
            string? checkInDateStr,
            string? checkOutDateStr,
            int? adults,
            int? children,
            int? minRating,
            decimal? maxPrice,
            List<string>? amenities
        );

        Task<(bool Success, Hotel? Hotel, string ErrorMessage)> CreateHotelAsync(HotelViewModel model, int creatingUserId);
        Task<(bool Success, string ErrorMessage)> UpdateHotelAsync(int id, HotelViewModel model);
        Task<bool> DeleteHotelAsync(int id);
    }
}
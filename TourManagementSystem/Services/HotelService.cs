// File: TourManagementSystem/Services/HotelService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourManagementSystem.Data;
using TourManagementSystem.Models;
using TourManagementSystem.ViewModels; // For AdminHotelViewModel

namespace TourManagementSystem.Services
{
    public class HotelService : IHotelService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HotelService> _logger;

        public HotelService(ApplicationDbContext context, ILogger<HotelService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Hotel>> GetAllHotelsAsync()
        {
            try { return await _context.Hotels.AsNoTracking().OrderBy(h => h.Name).ToListAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "Error fetching all hotels."); return new List<Hotel>(); }
        }

        public async Task<Hotel?> GetHotelByIdAsync(int id)
        {
            try { return await _context.Hotels.Include(h => h.User).FirstOrDefaultAsync(h => h.Id == id); }
            catch (Exception ex) { _logger.LogError(ex, "Error fetching hotel by ID {HotelId}", id); return null; }
        }

        public async Task<IEnumerable<Hotel>> SearchHotelsAsync(
            string? destination, string? hotelName, string? checkInDateStr, string? checkOutDateStr,
            int? adults, int? children, int? minRating, decimal? maxPrice, List<string>? amenities)
        {
            // ... (Search logic as previously defined, no change needed here for this specific error)
            var query = _context.Hotels.AsQueryable();
            if (!string.IsNullOrWhiteSpace(destination)) query = query.Where(h => h.Destination != null && EF.Functions.Like(h.Destination, $"%{destination}%"));
            if (!string.IsNullOrWhiteSpace(hotelName)) query = query.Where(h => h.Name != null && EF.Functions.Like(h.Name, $"%{hotelName}%"));
            if (minRating.HasValue) query = query.Where(h => h.Rating >= minRating.Value);
            if (maxPrice.HasValue) query = query.Where(h => h.PricePerNight <= maxPrice.Value);
            if (adults.HasValue && adults > 0) query = query.Where(h => h.AvailableRooms > 0);
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<(bool Success, Hotel? CreatedHotel, string ErrorMessage)> CreateHotelAsync(AdminHotelViewModel model, int? creatingUserId)
        {
            if (model == null) return (false, null, "Hotel data is null.");
            var hotelEntity = new Hotel // Map AdminHotelViewModel to Hotel entity
            {
                Name = model.Name,
                Destination = model.Destination,
                Address = model.Address,
                Description = model.Description,
                PricePerNight = model.PricePerNight,
                Rating = model.Rating,
                AvailableRooms = model.AvailableRooms,
                PrimaryImageUrl = model.PrimaryImageUrl,
                UserId = model.UserId ?? creatingUserId // If UserId in entity is int?, this is fine. If it's int, ensure a non-null value.
            };
            // Assuming Hotel.UserId is int? (nullable) based on AdminHotelViewModel.UserId being int?
            // If Hotel.UserId is non-nullable int, and model.UserId and creatingUserId are both null,
            // you'd need to assign a default valid UserId or handle the error.
            if (hotelEntity.UserId == null && !(typeof(Hotel).GetProperty("UserId").PropertyType == typeof(int?)))
            {
                _logger.LogWarning("Attempting to create Hotel with null UserId, but entity's UserId is non-nullable. Assigning default or check logic.");
                // hotelEntity.UserId = DEFAULT_SYSTEM_USER_ID; // Example if you have one
            }

            try
            {
                _context.Hotels.Add(hotelEntity); await _context.SaveChangesAsync();
                return (Success: true, Hotel: hotelEntity, ErrorMessage: "Hotel created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating hotel '{Name}'", model.Name);
                return (false, null, $"Database error: {ex.Message}");
            }
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateHotelAsync(AdminHotelViewModel model) // Takes AdminHotelViewModel
        {
            if (model == null) return (false, "Hotel data is null.");
            var existingHotel = await _context.Hotels.FindAsync(model.Id);
            if (existingHotel == null) return (false, "Hotel not found.");

            // Map from AdminHotelViewModel to existing Hotel entity
            existingHotel.Name = model.Name; existingHotel.Destination = model.Destination;
            existingHotel.Address = model.Address; existingHotel.Description = model.Description;
            existingHotel.PricePerNight = model.PricePerNight; existingHotel.Rating = model.Rating;
            existingHotel.AvailableRooms = model.AvailableRooms;
            existingHotel.PrimaryImageUrl = model.PrimaryImageUrl; // Allow null to clear if desired
            existingHotel.UserId = model.UserId; // ViewModel UserId is int?, Entity Hotel.UserId should also be int?

            try
            {
                await _context.SaveChangesAsync();
                return (Success: true, ErrorMessage: "Hotel updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating hotel ID {HotelId}", model.Id);
                return (false, $"Database error: {ex.Message}");
            }
        }

        public async Task<bool> DeleteHotelAsync(int id)
        {
            // ... (Delete logic remains the same) ...
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return false;
            try { _context.Hotels.Remove(hotel); await _context.SaveChangesAsync(); return true; }
            catch (Exception ex) { _logger.LogError(ex, "Error deleting hotel ID {HotelId}", id); return false; }
        }

        public async Task<IEnumerable<Hotel>> GetFeaturedHotelsAsync(int count, string? category = null)
        { /* ... implementation ... */ return await _context.Hotels.Take(count).ToListAsync(); } // Simplified
        public async Task<IEnumerable<Hotel>> GetRandomHotelsAsync(int count)
        { /* ... implementation ... */ return await _context.Hotels.OrderBy(r => Guid.NewGuid()).Take(count).ToListAsync(); } // Simplified
    }
}
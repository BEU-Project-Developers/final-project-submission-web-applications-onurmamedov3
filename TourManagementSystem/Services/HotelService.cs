using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourManagementSystem.Data;
using TourManagementSystem.Models;


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
            try
            {
                return await _context.Hotels.AsNoTracking().OrderBy(h => h.Name).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all hotels.");
                return new List<Hotel>();
            }
        }

        public async Task<Hotel?> GetHotelByIdAsync(int id)
        {
            try
            {
                // Include the User navigation property if you need to display user info related to the hotel
                return await _context.Hotels.Include(h => h.User).FirstOrDefaultAsync(h => h.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching hotel by ID {HotelId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<Hotel>> SearchHotelsAsync(
            string? destination, string? hotelName, string? checkInDateStr, string? checkOutDateStr,
            int? adults, int? children, int? minRating, decimal? maxPrice, List<string>? amenities)
        {
            var query = _context.Hotels.AsQueryable();

            if (!string.IsNullOrWhiteSpace(destination))
            {
                query = query.Where(h => h.Destination != null && EF.Functions.Like(h.Destination, $"%{destination}%"));
            }
            if (!string.IsNullOrWhiteSpace(hotelName))
            {
                query = query.Where(h => h.Name != null && EF.Functions.Like(h.Name, $"%{hotelName}%"));
            }
            if (minRating.HasValue)
            {
                query = query.Where(h => h.Rating >= minRating.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(h => h.PricePerNight <= maxPrice.Value);
            }
            if (adults.HasValue && adults > 0)
            {
                // This condition might need refinement (e.g., check against specific room types or capacities if available)
                query = query.Where(h => h.AvailableRooms > 0);
            }
            // Further filtering for amenities, check-in/out dates would go here if implemented.

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<(bool Success, Hotel? CreatedHotel, string? ErrorMessage)> CreateHotelAsync(AdminHotelViewModel model, int? creatingUserId)
        {
            if (model == null)
            {
                return (false, null, "Hotel data is null.");
            }

            var hotelEntity = new Hotel
            {
                Name = model.Name,
                Destination = model.Destination,
                Address = model.Address,
                Description = model.Description,
                PricePerNight = model.PricePerNight,
                Rating = model.Rating,
                AvailableRooms = model.AvailableRooms,
                PrimaryImageUrl = model.PrimaryImageUrl,
                // Assign UserId from the model if provided, otherwise use the creatingUserId (logged-in admin)
                // This assumes your Hotel.UserId is int? to align with User.Id (int)
                UserId = model.UserId ?? creatingUserId
            };

            // This check from your original code is good if Hotel.UserId was ever non-nullable.
            // For Hotel.UserId as int?, this check is less critical unless creatingUserId is also null
            // and you have a business rule that a Hotel must always have a UserId.
            if (hotelEntity.UserId == null && !(typeof(Hotel).GetProperty(nameof(Hotel.UserId))!.PropertyType == typeof(int?)))
            {
                _logger.LogWarning("Attempting to create Hotel with null UserId, but entity's UserId is non-nullable. This scenario should be handled or prevented.");
                // Potentially return an error or assign a default system user ID if applicable
                // return (false, null, "Hotel must be assigned to a user.");
            }

            try
            {
                _context.Hotels.Add(hotelEntity);
                await _context.SaveChangesAsync();
                return (true, hotelEntity, "Hotel created successfully.");
            }
            catch (DbUpdateException ex) // More specific exception
            {
                _logger.LogError(ex, "Database error while creating hotel '{Name}'. InnerException: {InnerEx}", model.Name, ex.InnerException?.Message);
                return (false, null, $"A database error occurred: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating hotel '{Name}'", model.Name);
                return (false, null, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateHotelAsync(AdminHotelViewModel model)
        {
            if (model == null)
            {
                return (false, "Hotel data is null.");
            }

            var existingHotel = await _context.Hotels.FindAsync(model.Id);
            if (existingHotel == null)
            {
                return (false, "Hotel not found.");
            }

            // Map from AdminHotelViewModel to existing Hotel entity
            existingHotel.Name = model.Name;
            existingHotel.Destination = model.Destination;
            existingHotel.Address = model.Address;
            existingHotel.Description = model.Description;
            existingHotel.PricePerNight = model.PricePerNight;
            existingHotel.Rating = model.Rating;
            existingHotel.AvailableRooms = model.AvailableRooms;
            existingHotel.PrimaryImageUrl = model.PrimaryImageUrl;
            existingHotel.UserId = model.UserId; // Hotel.UserId is int?, AdminHotelViewModel.UserId is int?

            try
            {
                _context.Entry(existingHotel).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return (true, "Hotel updated successfully.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict while updating hotel ID {HotelId}", model.Id);
                return (false, "The hotel record was modified by another user. Please reload and try again.");
            }
            catch (DbUpdateException ex) // More specific exception
            {
                _logger.LogError(ex, "Database error while updating hotel ID {HotelId}. InnerException: {InnerEx}", model.Id, ex.InnerException?.Message);
                return (false, $"A database error occurred: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating hotel ID {HotelId}", model.Id);
                return (false, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<bool> DeleteHotelAsync(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                _logger.LogWarning("Attempted to delete non-existent hotel with ID {HotelId}", id);
                return false;
            }
            try
            {
                _context.Hotels.Remove(hotel);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting hotel ID {HotelId}", id);
                return false;
            }
        }

        public async Task<IEnumerable<Hotel>> GetFeaturedHotelsAsync(int count, string? category = null)
        {
            try
            {
                var query = _context.Hotels.AsNoTracking();
                if (!string.IsNullOrEmpty(category))
                {
                    // Example: if you had a Category property on Hotel
                    // query = query.Where(h => h.Category == category);
                }
                // Add ordering for "featured" - e.g., by rating, by special flag, etc.
                return await query.OrderByDescending(h => h.Rating).ThenBy(h => h.PricePerNight).Take(count).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching featured hotels.");
                return new List<Hotel>();
            }
        }

        public async Task<IEnumerable<Hotel>> GetRandomHotelsAsync(int count)
        {
            try
            {
                // For SQL Server, EF.Functions.Random() can be used if available or use Guid.NewGuid()
                // For MySQL, you might need raw SQL or a different approach for true randomness efficiently.
                // Guid.NewGuid() is database-agnostic for ordering but might not be performant on huge datasets.
                return await _context.Hotels.AsNoTracking().OrderBy(r => Guid.NewGuid()).Take(count).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching random hotels.");
                return new List<Hotel>();
            }
        }
    }
}
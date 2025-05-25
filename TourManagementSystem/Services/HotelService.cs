// File: TourManagementSystem/Services/HotelService.cs
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
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Hotel>> GetAllHotelsAsync()
        {
            try
            {
                return await _context.Hotels.AsNoTracking().ToListAsync();
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
                return await _context.Hotels.FirstOrDefaultAsync(h => h.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching hotel with ID {HotelId}.", id);
                return null;
            }
        }

        // Corrected implementation with 9 arguments
        public async Task<IEnumerable<Hotel>> SearchHotelsAsync(
            string? destination,     // For searching by location
            string? hotelName,       // For searching by specific hotel name
            string? checkInDateStr,
            string? checkOutDateStr,
            int? adults,
            int? children,
            int? minRating,
            decimal? maxPrice,
            List<string>? amenities)
        {
            try
            {
                var query = _context.Hotels.AsQueryable();

                if (!string.IsNullOrWhiteSpace(destination))
                {
                    // Search in hotel's destination field
                    query = query.Where(h => h.Destination != null && EF.Functions.Like(h.Destination, $"%{destination}%"));
                }

                if (!string.IsNullOrWhiteSpace(hotelName))
                {
                    // Search in hotel's name field
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
                    query = query.Where(h => h.AvailableRooms > 0); // Simplified
                }

                if (DateTime.TryParse(checkInDateStr, out DateTime checkIn) && DateTime.TryParse(checkOutDateStr, out DateTime checkOut))
                {
                    _logger.LogInformation("Date range: {CheckIn} to {CheckOut}. Full availability logic pending.", checkIn, checkOut);
                    // Actual date-based availability check would go here against a bookings table
                }
                if (amenities != null && amenities.Any())
                {
                    _logger.LogInformation("Amenities: {Amenities}. Filtering logic pending.", string.Join(", ", amenities));
                    // Actual amenity filtering would go here
                    // Example if Hotel model had boolean flags:
                    // if (amenities.Contains("PetFriendly")) query = query.Where(h => h.IsPetFriendly == true);
                }

                return await query.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching hotels with Destination: {Destination}, Name: {HotelName}", destination, hotelName);
                return new List<Hotel>();
            }
        }

        public async Task<(bool Success, Hotel? Hotel, string ErrorMessage)> CreateHotelAsync(HotelViewModel model, int creatingUserId)
        {
            var hotel = new Hotel
            {
                Name = model.Name,
                Destination = model.Destination,
                Address = model.Address,
                Description = model.Description,
                PricePerNight = model.PricePerNight,
                Rating = model.Rating,
                AvailableRooms = model.AvailableRooms,
                PrimaryImageUrl = model.PrimaryImageUrl,
                UserId = creatingUserId
            };

            try
            {
                _context.Hotels.Add(hotel);
                await _context.SaveChangesAsync();
                return (true, hotel, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating hotel '{HotelName}'.", model.Name);
                return (false, null, $"Database error: {ex.Message}");
            }
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateHotelAsync(int id, HotelViewModel model)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return (false, "Hotel not found.");

            hotel.Name = model.Name;
            hotel.Destination = model.Destination;
            hotel.Address = model.Address;
            hotel.Description = model.Description;
            hotel.PricePerNight = model.PricePerNight;
            hotel.Rating = model.Rating;
            hotel.AvailableRooms = model.AvailableRooms;
            if (!string.IsNullOrWhiteSpace(model.PrimaryImageUrl))
            {
                hotel.PrimaryImageUrl = model.PrimaryImageUrl;
            }

            try
            {
                _context.Hotels.Update(hotel);
                await _context.SaveChangesAsync();
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating hotel ID {HotelId}.", id);
                return (false, $"Database error: {ex.Message}");
            }
        }

        public async Task<bool> DeleteHotelAsync(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return false;

            try
            {
                _context.Hotels.Remove(hotel);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting hotel ID {HotelId}.", id);
                return false;
            }
        }
    }
}
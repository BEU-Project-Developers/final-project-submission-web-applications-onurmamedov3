// File: TourManagementSystem/Services/CarRentalService.cs
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
    public class CarRentalService : ICarRentalService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CarRentalService> _logger;

        public CarRentalService(ApplicationDbContext context, ILogger<CarRentalService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CarRental>> GetAllCarRentalsAsync()
        {
            try
            {
                return await _context.CarRentals.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all car rentals.");
                return new List<CarRental>();
            }
        }

        public async Task<CarRental?> GetCarRentalByIdAsync(int id)
        {
            try
            {
                return await _context.CarRentals.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching car rental with ID {CarRentalId}.", id);
                return null;
            }
        }

        public async Task<IEnumerable<CarRental>> SearchCarRentalsAsync(
            string? location, string? pickupDateStr, string? returnDateStr, string? carModelKeyword)
        {
            try
            {
                var query = _context.CarRentals.AsQueryable();

                if (!string.IsNullOrWhiteSpace(location))
                {
                    query = query.Where(cr => EF.Functions.Like(cr.Location, $"%{location}%"));
                }

                if (!string.IsNullOrWhiteSpace(carModelKeyword))
                {
                    query = query.Where(cr =>
                        EF.Functions.Like(cr.CarModel, $"%{carModelKeyword}%") ||
                        EF.Functions.Like(cr.Company, $"%{carModelKeyword}%") // Optionally search company too
                    );
                }

                // Date-based availability for car rentals is complex.
                // It requires checking a bookings/reservations table against the requested dates.
                // This is a placeholder for that logic.
                if (DateTime.TryParse(pickupDateStr, out DateTime pickupDate) &&
                    DateTime.TryParse(returnDateStr, out DateTime returnDate))
                {
                    _logger.LogInformation("Car rental search with dates: Pickup {PickupDate}, Return {ReturnDate}. Full availability logic is complex and not fully implemented here.", pickupDate, returnDate);
                    // Placeholder: In a real system, you would join with a CarBookings table and check for overlaps.
                    // For now, we assume if dates are given, we're just noting them.
                    // Example (very naive): query = query.Where(cr => cr.IsAvailable); // If you had such a flag
                }

                return await query.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching car rentals. Location: {Location}, Model: {CarModelKeyword}", location, carModelKeyword);
                return new List<CarRental>();
            }
        }
    }
}
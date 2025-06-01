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
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<CarRental>> GetAllCarRentalsAsync()
        {
            try
            {
                return await _context.CarRentals
                    .AsNoTracking()
                    .OrderBy(cr => cr.Company)
                    .ThenBy(cr => cr.CarModel)
                    .ToListAsync();
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
                return await _context.CarRentals
                    .Include(cr => cr.User) // Assuming you want to see User info
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cr => cr.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching car rental with ID {CarRentalId}.", id);
                return null;
            }
        }

        public async Task<int> GetTotalCarRentalsCountAsync()
        {
            try
            {
                return await _context.CarRentals.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total car rentals count.");
                return 0;
            }
        }

        // Simplified SearchCarRentalsAsync
        public async Task<IEnumerable<CarRental>> SearchCarRentalsAsync(
            string? location,
            string? pickupDateStr,
            string? returnDateStr,
            string? carModelKeyword)
        {
            try
            {
                var query = _context.CarRentals.AsQueryable();

                if (!string.IsNullOrWhiteSpace(location))
                {
                    query = query.Where(cr => cr.Location != null && EF.Functions.Like(cr.Location, $"%{location}%"));
                }

                if (!string.IsNullOrWhiteSpace(carModelKeyword))
                {
                    query = query.Where(cr =>
                        (cr.CarModel != null && EF.Functions.Like(cr.CarModel, $"%{carModelKeyword}%")) ||
                        (cr.Company != null && EF.Functions.Like(cr.Company, $"%{carModelKeyword}%"))
                    );
                }

                // Basic date logging, no complex availability check with this simple model
                if (DateTime.TryParse(pickupDateStr, out DateTime pickupDate) && DateTime.TryParse(returnDateStr, out DateTime returnDate))
                {
                    _logger.LogInformation("Car rental search with dates: Pickup {PickupDate}, Return {ReturnDate}. Simple model, no advanced availability check.", pickupDate, returnDate);
                }

                return await query.AsNoTracking().OrderBy(cr => cr.PricePerDay).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching car rentals. Location: {Location}, Model: {CarModelKeyword}", location, carModelKeyword);
                return new List<CarRental>();
            }
        }

        public async Task<(bool Success, CarRental? CreatedCarRental, string? ErrorMessage)> CreateCarRentalAsync(AdminCarRentalViewModel model, int? creatingUserId)
        {
            if (model == null) return (false, null, "Car rental data model cannot be null.");

            var carRentalEntity = new CarRental
            {
                CarModel = model.CarModel,
                Company = model.Company,
                Location = model.Location,
                PricePerDay = model.PricePerDay,
                ImageUrl = model.ImageUrl.Trim(), // ViewModel's ImageUrl is required
                // If model.UserId is a non-nullable int as per simplified ViewModel, use it directly.
                // If creatingUserId is a fallback, AdminCarRentalViewModel.UserId would need to be int?
                UserId = model.UserId
            };

            try
            {
                _context.CarRentals.Add(carRentalEntity);
                await _context.SaveChangesAsync();
                return (true, carRentalEntity, "Car rental created successfully.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DB error creating car rental '{CarModel}'. Inner: {InnerEx}", model.CarModel, ex.InnerException?.Message);
                return (false, null, $"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating car rental '{CarModel}'.", model.CarModel);
                return (false, null, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateCarRentalAsync(AdminCarRentalViewModel model)
        {
            if (model == null) return (false, "Car rental data model cannot be null.");

            var existingCarRental = await _context.CarRentals.FindAsync(model.Id);
            if (existingCarRental == null) return (false, "Car rental not found for update.");

            existingCarRental.CarModel = model.CarModel;
            existingCarRental.Company = model.Company;
            existingCarRental.Location = model.Location;
            existingCarRental.PricePerDay = model.PricePerDay;
            existingCarRental.ImageUrl = model.ImageUrl.Trim();
            existingCarRental.UserId = model.UserId;

            try
            {
                _context.Entry(existingCarRental).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return (true, "Car rental updated successfully.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency error updating car rental ID {Id}", model.Id);
                return (false, "Record modified. Refresh and try again.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DB error updating car rental ID {Id}. Inner: {InnerEx}", model.Id, ex.InnerException?.Message);
                return (false, $"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating car rental ID {Id}", model.Id);
                return (false, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<bool> DeleteCarRentalAsync(int id)
        {
            var carRental = await _context.CarRentals.FindAsync(id);
            if (carRental == null)
            {
                _logger.LogWarning("Attempted to delete non-existent car rental ID {Id}.", id);
                return false;
            }
            try
            {
                _context.CarRentals.Remove(carRental);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting car rental ID {Id}.", id);
                return false;
            }
        }
    }
}
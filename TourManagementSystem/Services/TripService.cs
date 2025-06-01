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
    public class TripService : ITripService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TripService> _logger;

        public TripService(ApplicationDbContext context, ILogger<TripService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Trip>> GetAllTripsAsync()
        {
            try
            {
                return await _context.Trips
                    .AsNoTracking()
                    .OrderBy(t => t.Title)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all trips.");
                return new List<Trip>();
            }
        }

        public async Task<Trip?> GetTripByIdAsync(int id)
        {
            try
            {
                return await _context.Trips
                    .Include(t => t.User) // Eager load User for details view
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching trip with ID {TripId}", id);
                return null;
            }
        }

        public async Task<int> GetTotalTripsCountAsync()
        {
            try
            {
                return await _context.Trips.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total trips count.");
                return 0;
            }
        }

        public async Task<IEnumerable<Trip>> SearchTripsAsync(string? tripSearchTerm, string? startDateStr, int? travelers, decimal? maxTripPrice)
        {
            var query = _context.Trips.AsQueryable();

            if (!string.IsNullOrWhiteSpace(tripSearchTerm))
            {
                query = query.Where(t =>
                    (t.Title != null && EF.Functions.Like(t.Title, $"%{tripSearchTerm}%")) ||
                    (t.Destination != null && EF.Functions.Like(t.Destination, $"%{tripSearchTerm}%")) ||
                    (t.Description != null && EF.Functions.Like(t.Description, $"%{tripSearchTerm}%"))
                );
            }

            if (DateTime.TryParse(startDateStr, out DateTime startDateValue))
            {
                _logger.LogInformation("Trip search by StartDate ({StartDate}) - Note: Trip model needs a StartDate property and DB column for this filter to work.", startDateValue);
                // To use this, add: public DateTime StartDate { get; set; } to Trip.cs and migrate.
                // query = query.Where(t => t.StartDate >= startDateValue);
            }

            if (travelers.HasValue && travelers > 0)
            {
                _logger.LogInformation("Trip search by Travelers ({Travelers}) - Note: Trip model needs capacity info (e.g., MaxParticipants) for this filter to work.", travelers);
                // To use this, add a capacity property to Trip.cs and migrate.
                // query = query.Where(t => t.MaxParticipants >= travelers);
            }

            if (maxTripPrice.HasValue)
            {
                query = query.Where(t => t.Price <= maxTripPrice.Value);
            }

            return await query.AsNoTracking().OrderBy(t => t.Price).ToListAsync();
        }

        public async Task<(bool Success, Trip? CreatedTrip, string? ErrorMessage)> CreateTripAsync(AdminTripViewModel model, int? creatingUserId)
        {
            if (model == null) return (false, null, "Trip data model cannot be null.");

            var tripEntity = new Trip
            {
                Title = model.Title,
                Destination = model.Destination,
                DurationDays = model.DurationDays,
                Price = model.Price,
                Description = model.Description, // ViewModel.Description is now required string
                ImageUrl = model.ImageUrl,     // ViewModel.ImageUrl is now required string
                // UserId in AdminTripViewModel is required int.
                // creatingUserId is a fallback if ViewModel.UserId was nullable, not needed here.
                UserId = model.UserId
            };

            try
            {
                _context.Trips.Add(tripEntity);
                await _context.SaveChangesAsync();
                return (true, tripEntity, "Trip created successfully.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DB error creating trip '{Title}'. Inner: {InnerEx}", model.Title, ex.InnerException?.Message);
                return (false, null, $"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating trip '{Title}'.", model.Title);
                return (false, null, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateTripAsync(AdminTripViewModel model)
        {
            if (model == null) return (false, "Trip data model cannot be null.");

            var existingTrip = await _context.Trips.FindAsync(model.Id);
            if (existingTrip == null) return (false, "Trip not found for update.");

            existingTrip.Title = model.Title;
            existingTrip.Destination = model.Destination;
            existingTrip.DurationDays = model.DurationDays;
            existingTrip.Price = model.Price;
            existingTrip.Description = model.Description;
            existingTrip.ImageUrl = model.ImageUrl;
            existingTrip.UserId = model.UserId;

            try
            {
                _context.Entry(existingTrip).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return (true, "Trip updated successfully.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency error updating trip ID {Id}", model.Id);
                return (false, "Record modified. Refresh and try again.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DB error updating trip ID {Id}. Inner: {InnerEx}", model.Id, ex.InnerException?.Message);
                return (false, $"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating trip ID {Id}", model.Id);
                return (false, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<bool> DeleteTripAsync(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                _logger.LogWarning("Attempt to delete non-existent trip ID {Id}.", id);
                return false;
            }
            try
            {
                _context.Trips.Remove(trip);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting trip ID {Id}.", id);
                return false;
            }
        }
    }
}
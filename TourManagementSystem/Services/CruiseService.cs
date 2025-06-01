using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourManagementSystem.Data;
using TourManagementSystem.Models; // For Cruise entity and AdminCruiseViewModel

namespace TourManagementSystem.Services
{
    public class CruiseService : ICruiseService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CruiseService> _logger;

        public CruiseService(ApplicationDbContext context, ILogger<CruiseService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Cruise>> GetAllCruisesAsync()
        {
            try
            {
                return await _context.Cruises
                    .AsNoTracking()
                    .OrderBy(c => c.CruiseLine)
                    .ThenBy(c => c.Destination)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all cruises.");
                return new List<Cruise>();
            }
        }

        public async Task<Cruise?> GetCruiseByIdAsync(int id)
        {
            try
            {
                return await _context.Cruises
                    .Include(c => c.User) // Eager load User
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cruise with ID {CruiseId}", id);
                return null;
            }
        }

        public async Task<int> GetTotalCruisesCountAsync()
        {
            try
            {
                return await _context.Cruises.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total cruises count.");
                return 0;
            }
        }

        public async Task<IEnumerable<Cruise>> SearchCruisesAsync(
            string? destinationRegion, string? departurePort, string? cruiseLine,
            int? minDurationDays, int? maxDurationDays, decimal? maxPrice)
        {
            try
            {
                _logger.LogInformation("Searching cruises. DestRegion: {DR}, DepPort: {DP}, Line: {CL}", destinationRegion, departurePort, cruiseLine);
                var query = _context.Cruises.AsQueryable();

                if (!string.IsNullOrWhiteSpace(destinationRegion))
                {
                    query = query.Where(c => c.Destination != null && EF.Functions.Like(c.Destination, $"%{destinationRegion}%"));
                }
                if (!string.IsNullOrWhiteSpace(departurePort))
                {
                    query = query.Where(c => c.DeparturePort != null && EF.Functions.Like(c.DeparturePort, $"%{departurePort}%"));
                }
                if (!string.IsNullOrWhiteSpace(cruiseLine))
                {
                    query = query.Where(c => c.CruiseLine != null && EF.Functions.Like(c.CruiseLine, $"%{cruiseLine}%"));
                }
                if (minDurationDays.HasValue)
                {
                    query = query.Where(c => c.DurationDays >= minDurationDays.Value);
                }
                if (maxDurationDays.HasValue)
                {
                    query = query.Where(c => c.DurationDays <= maxDurationDays.Value);
                }
                if (maxPrice.HasValue)
                {
                    query = query.Where(c => c.Price <= maxPrice.Value);
                }

                return await query.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching cruises.");
                return new List<Cruise>();
            }
        }

        public async Task<(bool Success, Cruise? CreatedCruise, string? ErrorMessage)> CreateCruiseAsync(AdminCruiseViewModel model, int? creatingUserId)
        {
            if (model == null) return (false, null, "Cruise data model cannot be null.");

            var cruiseEntity = new Cruise
            {
                CruiseLine = model.CruiseLine,
                DeparturePort = model.DeparturePort,
                Destination = model.Destination,
                DurationDays = model.DurationDays,
                Price = model.Price,
                ImageUrl = model.ImageUrl, // ViewModel.ImageUrl is required
                UserId = model.UserId      // ViewModel.UserId is required
                // ItinerarySummary from ViewModel is not directly mapped to Cruise entity
                // If you wanted to store it, you'd add a property to Cruise.cs and map it here.
            };

            try
            {
                _context.Cruises.Add(cruiseEntity);
                await _context.SaveChangesAsync();
                return (true, cruiseEntity, "Cruise created successfully.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DB error creating cruise '{CruiseLine}' - '{Dest}'. Inner: {InnerEx}", model.CruiseLine, model.Destination, ex.InnerException?.Message);
                return (false, null, $"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cruise '{CruiseLine}' - '{Dest}'.", model.CruiseLine, model.Destination);
                return (false, null, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateCruiseAsync(AdminCruiseViewModel model)
        {
            if (model == null) return (false, "Cruise data model cannot be null.");

            var existingCruise = await _context.Cruises.FindAsync(model.Id);
            if (existingCruise == null) return (false, "Cruise not found for update.");

            existingCruise.CruiseLine = model.CruiseLine;
            existingCruise.DeparturePort = model.DeparturePort;
            existingCruise.Destination = model.Destination;
            existingCruise.DurationDays = model.DurationDays;
            existingCruise.Price = model.Price;
            existingCruise.ImageUrl = model.ImageUrl;
            existingCruise.UserId = model.UserId;
            // existingCruise.ItinerarySummary is not a property of Cruise entity

            try
            {
                _context.Entry(existingCruise).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return (true, "Cruise updated successfully.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency error updating cruise ID {Id}", model.Id);
                return (false, "Record modified. Refresh and try again.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DB error updating cruise ID {Id}. Inner: {InnerEx}", model.Id, ex.InnerException?.Message);
                return (false, $"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cruise ID {Id}", model.Id);
                return (false, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<bool> DeleteCruiseAsync(int id)
        {
            var cruise = await _context.Cruises.FindAsync(id);
            if (cruise == null)
            {
                _logger.LogWarning("Attempt to delete non-existent cruise ID {Id}.", id);
                return false;
            }
            try
            {
                _context.Cruises.Remove(cruise);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cruise ID {Id}.", id);
                return false;
            }
        }
    }
}
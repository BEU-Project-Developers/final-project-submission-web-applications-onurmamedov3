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
            _context = context;
            _logger = logger;
        }

        public async Task<Trip?> GetTripByIdAsync(int id)
        {
            try
            {
                return await _context.Trips.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching trip with ID {TripId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<Trip>> SearchTripsAsync(string? tripSearchTerm, string? startDateStr, int? travelers, decimal? maxTripPrice)
        {
            var query = _context.Trips.AsQueryable();

            if (!string.IsNullOrWhiteSpace(tripSearchTerm))
            {
                query = query.Where(t => EF.Functions.Like(t.Title, $"%{tripSearchTerm}%") ||
                                         EF.Functions.Like(t.Destination, $"%{tripSearchTerm}%") ||
                                         EF.Functions.Like(t.Description, $"%{tripSearchTerm}%"));
            }

            if (DateTime.TryParse(startDateStr, out DateTime startDateValue))
            {
                // This assumes trips have a StartDate property. Your current Trip model doesn't.
                // You'd need to add StartDate to your Trip model and DB table.
                // For now, this part won't filter if Trip model lacks StartDate.
                // query = query.Where(t => t.StartDate >= startDateValue); 
                _logger.LogInformation("Trip search by StartDate ({StartDate}) - Note: Trip model needs a StartDate property for this filter.", startDateValue);
            }

            if (travelers.HasValue && travelers > 0)
            {
                // This assumes trips have a MaxCapacity or similar. Your current Trip model doesn't.
                // query = query.Where(t => t.AvailableSlots >= travelers || t.MaxCapacity >= travelers);
                _logger.LogInformation("Trip search by Travelers ({Travelers}) - Note: Trip model needs capacity info for this filter.", travelers);
            }

            if (maxTripPrice.HasValue)
            {
                query = query.Where(t => t.Price <= maxTripPrice.Value);
            }

            return await query.AsNoTracking().ToListAsync();
        }
    }
}
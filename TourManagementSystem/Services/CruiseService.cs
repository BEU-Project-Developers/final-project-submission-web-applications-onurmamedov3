// File: TourManagementSystem/Services/CruiseService.cs
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
    public class CruiseService : ICruiseService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CruiseService> _logger;

        public CruiseService(ApplicationDbContext context, ILogger<CruiseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Cruise?> GetCruiseByIdAsync(int id)
        {
            try
            {
                return await _context.Cruises.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cruise with ID {CruiseId}", id);
                return null;
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
    }
}
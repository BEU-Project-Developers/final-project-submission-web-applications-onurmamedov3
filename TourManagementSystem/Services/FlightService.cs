// File: TourManagementSystem/Services/FlightService.cs
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
    public class FlightService : IFlightService // Ensure IFlightService is defined
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FlightService> _logger;

        public FlightService(ApplicationDbContext context, ILogger<FlightService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Flight?> GetFlightByIdAsync(int id)
        {
            try
            {
                return await _context.Flights.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching flight with ID {FlightId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<Flight>> SearchFlightsAsync(
            string origin, string destination, DateTime departureDate,
            DateTime? returnDate, int passengers, TripType tripType)
        {
            try
            {
                _logger.LogInformation("Searching flights: {Origin} to {Destination}, Departing: {DepartureDate}, TripType: {TripType}, Passengers: {Passengers}",
                    origin, destination, departureDate.ToShortDateString(), tripType, passengers);

                var query = _context.Flights.AsQueryable();

                if (!string.IsNullOrWhiteSpace(origin))
                {
                    query = query.Where(f => f.DepartureCity != null && EF.Functions.Like(f.DepartureCity, $"%{origin}%"));
                }
                if (!string.IsNullOrWhiteSpace(destination))
                {
                    query = query.Where(f => f.ArrivalCity != null && EF.Functions.Like(f.ArrivalCity, $"%{destination}%"));
                }

                query = query.Where(f => f.DepartureTime.Date == departureDate.Date);

                // Placeholder for passenger count / seat availability
                if (passengers > 0)
                {
                    // Real logic would involve checking available seats.
                    _logger.LogInformation("Passenger count {Passengers} noted. Actual seat availability check not implemented in basic search.", passengers);
                }

                var results = await query.AsNoTracking().ToListAsync();
                _logger.LogInformation("Found {Count} flights matching initial criteria.", results.Count);

                if (tripType == TripType.RoundTrip && returnDate.HasValue && results.Any())
                {
                    _logger.LogInformation("Round trip selected with return date {ReturnDate}. Complex pairing logic not implemented.", returnDate.Value.ToShortDateString());
                    // For a true round trip, you would now search for return flights based on 'returnDate'
                    // from 'destination' back to 'origin' and then combine them with the outbound results.
                    // This basic search returns one-way segments.
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching flights.");
                return new List<Flight>();
            }
        }
    }
}
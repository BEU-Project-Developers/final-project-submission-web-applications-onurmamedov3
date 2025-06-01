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
    public class FlightService : IFlightService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FlightService> _logger;

        public FlightService(ApplicationDbContext context, ILogger<FlightService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
        {
            try
            {
                return await _context.Flights
                    .AsNoTracking()
                    .OrderBy(f => f.Airline)
                    .ThenBy(f => f.DepartureTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all flights.");
                return new List<Flight>();
            }
        }

        public async Task<Flight?> GetFlightByIdAsync(int id)
        {
            try
            {
                return await _context.Flights
                    .Include(f => f.User) // Eager load user if needed
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching flight with ID {FlightId}", id);
                return null;
            }
        }

        public async Task<int> GetTotalFlightsCountAsync()
        {
            try
            {
                return await _context.Flights.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total flights count.");
                return 0;
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

                if (passengers > 0)
                {
                    _logger.LogInformation("Passenger count {Passengers} noted. Seat availability check not implemented.", passengers);
                }

                var results = await query.AsNoTracking().ToListAsync();
                _logger.LogInformation("Found {Count} flights matching initial criteria.", results.Count);

                if (tripType == TripType.RoundTrip && returnDate.HasValue && results.Any())
                {
                    _logger.LogInformation("Round trip selected with return date {ReturnDate}. Complex pairing logic not implemented.", returnDate.Value.ToShortDateString());
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching flights.");
                return new List<Flight>();
            }
        }

        public async Task<(bool Success, Flight? CreatedFlight, string? ErrorMessage)> CreateFlightAsync(AdminFlightViewModel model, int? creatingUserId)
        {
            if (model == null) return (false, null, "Flight data model cannot be null.");

            if (model.ArrivalTime <= model.DepartureTime) // Server-side validation
            {
                return (false, null, "Arrival time must be after departure time.");
            }

            var flightEntity = new Flight
            {
                Airline = model.Airline,
                FlightNumber = model.FlightNumber, // ViewModel has it as required string
                DepartureCity = model.DepartureCity,
                ArrivalCity = model.ArrivalCity,
                DepartureTime = model.DepartureTime,
                ArrivalTime = model.ArrivalTime,
                Price = model.Price,
                AirlineLogoUrl = model.AirlineLogoUrl?.Trim(),
                UserId = model.UserId ?? creatingUserId
            };

            try
            {
                _context.Flights.Add(flightEntity);
                await _context.SaveChangesAsync();
                return (true, flightEntity, "Flight created successfully.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DB error creating flight {Airline} {Number}. Inner: {InnerEx}", model.Airline, model.FlightNumber, ex.InnerException?.Message);
                return (false, null, $"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating flight {Airline} {Number}.", model.Airline, model.FlightNumber);
                return (false, null, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateFlightAsync(AdminFlightViewModel model)
        {
            if (model == null) return (false, "Flight data model cannot be null.");

            if (model.ArrivalTime <= model.DepartureTime) // Server-side validation
            {
                return (false, "Arrival time must be after departure time.");
            }

            var existingFlight = await _context.Flights.FindAsync(model.Id);
            if (existingFlight == null) return (false, "Flight not found for update.");

            existingFlight.Airline = model.Airline;
            existingFlight.FlightNumber = model.FlightNumber;
            existingFlight.DepartureCity = model.DepartureCity;
            existingFlight.ArrivalCity = model.ArrivalCity;
            existingFlight.DepartureTime = model.DepartureTime;
            existingFlight.ArrivalTime = model.ArrivalTime;
            existingFlight.Price = model.Price;
            existingFlight.AirlineLogoUrl = model.AirlineLogoUrl?.Trim();
            existingFlight.UserId = model.UserId;

            try
            {
                _context.Entry(existingFlight).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return (true, "Flight updated successfully.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency error updating flight ID {Id}", model.Id);
                return (false, "Record modified. Refresh and try again.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DB error updating flight ID {Id}. Inner: {InnerEx}", model.Id, ex.InnerException?.Message);
                return (false, $"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating flight ID {Id}", model.Id);
                return (false, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<bool> DeleteFlightAsync(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                _logger.LogWarning("Attempt to delete non-existent flight ID {Id}.", id);
                return false;
            }
            try
            {
                _context.Flights.Remove(flight);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting flight ID {Id}.", id);
                return false;
            }
        }
    }
}
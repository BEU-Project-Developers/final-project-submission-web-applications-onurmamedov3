using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public interface IFlightService
    {
        // Existing search method
        Task<IEnumerable<Flight>> SearchFlightsAsync(
            string origin,
            string destination,
            DateTime departureDate,
            DateTime? returnDate,
            int passengers,
            TripType tripType
        );

        // Methods for Admin CRUD
        Task<IEnumerable<Flight>> GetAllFlightsAsync(); // For Admin Index page
        Task<Flight?> GetFlightByIdAsync(int id); // For Admin Details/Edit GET/Delete GET

        Task<(bool Success, Flight? CreatedFlight, string? ErrorMessage)> CreateFlightAsync(AdminFlightViewModel model, int? creatingUserId);
        Task<(bool Success, string? ErrorMessage)> UpdateFlightAsync(AdminFlightViewModel model);
        Task<bool> DeleteFlightAsync(int id);
        Task<int> GetTotalFlightsCountAsync(); // For dashboard
    }
}
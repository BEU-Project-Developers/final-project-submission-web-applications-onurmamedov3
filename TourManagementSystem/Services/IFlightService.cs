// File: TourManagementSystem/Services/IFlightService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagementSystem.Models; // For Flight and TripType

namespace TourManagementSystem.Services
{
    public interface IFlightService
    {
        Task<IEnumerable<Flight>> SearchFlightsAsync(
            string origin, // Ensure this matches the type in FlightService and call from OffersController
            string destination, // Ensure this matches
            DateTime departureDate, // Ensure this matches
            DateTime? returnDate, // Ensure this matches (nullable DateTime)
            int passengers, // Ensure this matches
            TripType tripType // Ensure this matches (the enum TripType)
        );
        Task<Flight?> GetFlightByIdAsync(int id);
        // Add other methods like GetAllFlightsAsync, Create, Update, Delete if needed
    }
}
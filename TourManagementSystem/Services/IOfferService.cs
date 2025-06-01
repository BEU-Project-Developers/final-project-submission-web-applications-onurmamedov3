// File: TourManagementSystem/Services/IOfferService.cs
using TourManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TourManagementSystem.Services
{
    public interface IOfferService
    {
        // Get All Methods
        Task<List<Hotel>> GetHotelsAsync();
        Task<List<Trip>> GetTripsAsync();
        Task<List<Flight>> GetFlightsAsync();
        Task<List<CarRental>> GetCarRentalsAsync(); // For CarRentals
        Task<List<Cruise>> GetCruisesAsync();
        Task<List<Activity>> GetActivitiesAsync();

        // Get By ID Methods
        Task<Hotel?> GetHotelAsync(int id);
        Task<Trip?> GetTripAsync(int id);
        Task<Flight?> GetFlightAsync(int id);
        Task<CarRental?> GetCarRentalAsync(int id); // For CarRentals
        Task<Cruise?> GetCruiseAsync(int id);
        Task<Activity?> GetActivityAsync(int id);

        // Add Methods
        Task AddHotelAsync(Hotel hotel);
        Task AddTripAsync(Trip trip);
        Task AddFlightAsync(Flight flight);
        Task AddCarRentalAsync(CarRental carRental); // For CarRentals
        Task AddCruiseAsync(Cruise cruise);
        Task AddActivityAsync(Activity activity);

        // Update Methods
        Task UpdateHotelAsync(Hotel hotel);
        Task UpdateTripAsync(Trip trip);
        Task UpdateFlightAsync(Flight flight);
        Task UpdateCarRentalAsync(CarRental carRental); // For CarRentals
        Task UpdateCruiseAsync(Cruise cruise);
        Task UpdateActivityAsync(Activity activity);

        // Delete Methods
        Task DeleteHotelAsync(int id);
        Task DeleteTripAsync(int id);
        Task DeleteFlightAsync(int id);
        Task DeleteCarRentalAsync(int id); // For CarRentals
        Task DeleteCruiseAsync(int id);
        Task DeleteActivityAsync(int id);
    }
}
// File: TourManagementSystem/Services/OfferService.cs
using TourManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourManagementSystem.Services
{
    public class OfferService : IOfferService // Implements the corrected IOfferService
    {
        private readonly List<Hotel> _hotels = new List<Hotel>();
        private readonly List<Trip> _trips = new List<Trip>();
        private readonly List<Flight> _flights = new List<Flight>();
        private readonly List<CarRental> _carRentals = new List<CarRental>(); // For CarRentals
        private readonly List<Cruise> _cruises = new List<Cruise>();
        private readonly List<Activity> _activities = new List<Activity>();

        // Simulating ID counters
        private int GetNextId<T>(List<T> list) where T : class { dynamic? last = list.LastOrDefault(); return (last?.Id ?? 0) + 1; }

        // Get All Methods
        public Task<List<Hotel>> GetHotelsAsync() => Task.FromResult(new List<Hotel>(_hotels));
        public Task<List<Trip>> GetTripsAsync() => Task.FromResult(new List<Trip>(_trips));
        public Task<List<Flight>> GetFlightsAsync() => Task.FromResult(new List<Flight>(_flights));
        public Task<List<CarRental>> GetCarRentalsAsync() => Task.FromResult(new List<CarRental>(_carRentals)); // Corrected
        public Task<List<Cruise>> GetCruisesAsync() => Task.FromResult(new List<Cruise>(_cruises));
        public Task<List<Activity>> GetActivitiesAsync() => Task.FromResult(new List<Activity>(_activities));

        // Get By ID Methods
        public Task<Hotel?> GetHotelAsync(int id) => Task.FromResult(_hotels.Find(h => h.Id == id));
        public Task<Trip?> GetTripAsync(int id) => Task.FromResult(_trips.Find(t => t.Id == id));
        public Task<Flight?> GetFlightAsync(int id) => Task.FromResult(_flights.Find(f => f.Id == id));
        public Task<CarRental?> GetCarRentalAsync(int id) => Task.FromResult(_carRentals.Find(cr => cr.Id == id)); // Corrected
        public Task<Cruise?> GetCruiseAsync(int id) => Task.FromResult(_cruises.Find(c => c.Id == id));
        public Task<Activity?> GetActivityAsync(int id) => Task.FromResult(_activities.Find(a => a.Id == id));

        // Add Methods
        public Task AddHotelAsync(Hotel hotel) { if (hotel.Id == 0) hotel.Id = GetNextId(_hotels); _hotels.Add(hotel); return Task.CompletedTask; }
        public Task AddTripAsync(Trip trip) { if (trip.Id == 0) trip.Id = GetNextId(_trips); _trips.Add(trip); return Task.CompletedTask; }
        public Task AddFlightAsync(Flight flight) { if (flight.Id == 0) flight.Id = GetNextId(_flights); _flights.Add(flight); return Task.CompletedTask; }
        public Task AddCarRentalAsync(CarRental carRental) { if (carRental.Id == 0) carRental.Id = GetNextId(_carRentals); _carRentals.Add(carRental); return Task.CompletedTask; } // Corrected
        public Task AddCruiseAsync(Cruise cruise) { if (cruise.Id == 0) cruise.Id = GetNextId(_cruises); _cruises.Add(cruise); return Task.CompletedTask; }
        public Task AddActivityAsync(Activity activity) { if (activity.Id == 0) activity.Id = GetNextId(_activities); _activities.Add(activity); return Task.CompletedTask; }

        // Update Methods (Basic In-Memory Implementation)
        public Task UpdateHotelAsync(Hotel hotel) { var existing = _hotels.Find(h => h.Id == hotel.Id); if (existing != null) { _hotels.Remove(existing); _hotels.Add(hotel); } return Task.CompletedTask; }
        public Task UpdateTripAsync(Trip trip) { var existing = _trips.Find(t => t.Id == trip.Id); if (existing != null) { _trips.Remove(existing); _trips.Add(trip); } return Task.CompletedTask; }
        public Task UpdateFlightAsync(Flight flight) { var existing = _flights.Find(f => f.Id == flight.Id); if (existing != null) { _flights.Remove(existing); _flights.Add(flight); } return Task.CompletedTask; }
        public Task UpdateCarRentalAsync(CarRental carRental) { var existing = _carRentals.Find(cr => cr.Id == carRental.Id); if (existing != null) { _carRentals.Remove(existing); _carRentals.Add(carRental); } return Task.CompletedTask; } // Corrected
        public Task UpdateCruiseAsync(Cruise cruise) { var existing = _cruises.Find(c => c.Id == cruise.Id); if (existing != null) { _cruises.Remove(existing); _cruises.Add(cruise); } return Task.CompletedTask; }
        public Task UpdateActivityAsync(Activity activity) { var existing = _activities.Find(a => a.Id == activity.Id); if (existing != null) { _activities.Remove(existing); _activities.Add(activity); } return Task.CompletedTask; }

        // Delete Methods
        public Task DeleteHotelAsync(int id) { var item = _hotels.Find(h => h.Id == id); if (item != null) _hotels.Remove(item); return Task.CompletedTask; }
        public Task DeleteTripAsync(int id) { var item = _trips.Find(t => t.Id == id); if (item != null) _trips.Remove(item); return Task.CompletedTask; }
        public Task DeleteFlightAsync(int id) { var item = _flights.Find(f => f.Id == id); if (item != null) _flights.Remove(item); return Task.CompletedTask; }
        public Task DeleteCarRentalAsync(int id) { var item = _carRentals.Find(cr => cr.Id == id); if (item != null) _carRentals.Remove(item); return Task.CompletedTask; } // Corrected
        public Task DeleteCruiseAsync(int id) { var item = _cruises.Find(c => c.Id == id); if (item != null) _cruises.Remove(item); return Task.CompletedTask; }
        public Task DeleteActivityAsync(int id) { var item = _activities.Find(a => a.Id == id); if (item != null) _activities.Remove(item); return Task.CompletedTask; }
    }
}
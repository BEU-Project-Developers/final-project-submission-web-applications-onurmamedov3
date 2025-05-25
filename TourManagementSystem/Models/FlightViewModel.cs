// File: TourManagementSystem/Models/FlightViewModel.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models
{
    public class FlightViewModel
    {
        public int Id { get; set; }
        public string Airline { get; set; } = string.Empty;
        public string DepartureCity { get; set; } = string.Empty;
        public string ArrivalCity { get; set; } = string.Empty;

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DepartureTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ArrivalTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }
        public string? AirlineLogoUrl { get; set; }
        public string? FlightNumber { get; set; }
        public string Duration { get; set; } = string.Empty; // Calculated: ArrivalTime - DepartureTime

        public TripType SearchedTripType { get; set; }
        public DateTime? SearchedReturnDepartureTime { get; set; }
        public DateTime? SearchedReturnArrivalTime { get; set; }
        public decimal? ReturnFlightPrice { get; set; }
    }
}
// File: TourManagementSystem/Models/Flight.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagementSystem.Models
{
    public enum TripType
    {
        OneWay,
        RoundTrip
    }

    public class Flight
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Airline name is required.")]
        [StringLength(100)]
        public string Airline { get; set; } = string.Empty;

        [Required(ErrorMessage = "Departure city is required.")]
        [StringLength(100)]
        public string DepartureCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Arrival city is required.")]
        [StringLength(100)]
        public string ArrivalCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Departure date and time are required.")]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Arrival date and time are required.")]
        public DateTime ArrivalTime { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 20000.00, ErrorMessage = "Price must be realistic.")]
        public decimal Price { get; set; }

        [StringLength(255)]
        public string? AirlineLogoUrl { get; set; }

        [StringLength(20)]
        public string? FlightNumber { get; set; }

        public int? UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
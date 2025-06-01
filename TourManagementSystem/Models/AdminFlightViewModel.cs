// File: TourManagementSystem/Models/AdminFlightViewModel.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models // Or TourManagementSystem.Models
{
    public class AdminFlightViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Airline name is required.")]
        [StringLength(100)]
        public string Airline { get; set; } = string.Empty;

        [Required(ErrorMessage = "Flight number is required.")]
        [StringLength(20)]
        [Display(Name = "Flight Number")]
        public string FlightNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Departure city is required.")]
        [StringLength(100)]
        [Display(Name = "Departure City")]
        public string DepartureCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Arrival city is required.")]
        [StringLength(100)]
        [Display(Name = "Arrival City")]
        public string ArrivalCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Departure date and time are required.")]
        [Display(Name = "Departure Date & Time")]
        public DateTime DepartureTime { get; set; } = DateTime.Now.Date.AddHours(9); // Default to today at 9 AM

        [Required(ErrorMessage = "Arrival date and time are required.")]
        [Display(Name = "Arrival Date & Time")]
        public DateTime ArrivalTime { get; set; } = DateTime.Now.Date.AddHours(11); // Default to today at 11 AM

        [Required(ErrorMessage = "Price is required.")]
        [DataType(DataType.Currency)]
        [Range(0.01, 20000.00, ErrorMessage = "Price must be realistic and positive.")]
        public decimal Price { get; set; }

        [Display(Name = "Airline Logo URL (Optional)")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        [StringLength(255)]
        public string? AirlineLogoUrl { get; set; }

        [Display(Name = "User ID (Managed By - Optional)")]
        public int? UserId { get; set; }

        // You might add fields like AvailableSeats if you manage inventory
        // public int AvailableSeats { get; set; }
    }
}
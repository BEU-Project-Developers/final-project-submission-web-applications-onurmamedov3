using System;
using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models // Or TourManagementSystem.ViewModels
{
    public class AdminFlightViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Airline name is required.")]
        [StringLength(100, ErrorMessage = "Airline name cannot exceed 100 characters.")]
        public string Airline { get; set; } = string.Empty;

        // FlightNumber is nullable in Flight.cs, so make it nullable here too if it's truly optional on create/edit
        // If it's always required for admin input, keep [Required]
        [Required(ErrorMessage = "Flight number is required for admin entry.")]
        [StringLength(20, ErrorMessage = "Flight number cannot exceed 20 characters.")]
        [Display(Name = "Flight Number")]
        public string FlightNumber { get; set; } = string.Empty; // Or string? if optional

        [Required(ErrorMessage = "Departure city is required.")]
        [StringLength(100, ErrorMessage = "Departure city cannot exceed 100 characters.")]
        [Display(Name = "Departure City")]
        public string DepartureCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Arrival city is required.")]
        [StringLength(100, ErrorMessage = "Arrival city cannot exceed 100 characters.")]
        [Display(Name = "Arrival City")]
        public string ArrivalCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Departure date and time are required.")]
        [Display(Name = "Departure Date & Time")]
        [DataType(DataType.DateTime)]
        public DateTime DepartureTime { get; set; } = DateTime.Now.Date.AddHours(DateTime.Now.Hour + 1);

        [Required(ErrorMessage = "Arrival date and time are required.")]
        [Display(Name = "Arrival Date & Time")]
        [DataType(DataType.DateTime)]
        public DateTime ArrivalTime { get; set; } = DateTime.Now.Date.AddHours(DateTime.Now.Hour + 3);

        [Required(ErrorMessage = "Price is required.")]
        [DataType(DataType.Currency)]
        [Range(0.01, 20000.00, ErrorMessage = "Price must be a positive value and realistic.")]
        public decimal Price { get; set; }

        [Display(Name = "Airline Logo URL (Optional)")]
        //[Url(ErrorMessage = "Please enter a valid URL for the image.")]
        [StringLength(255, ErrorMessage = "URL cannot exceed 255 characters.")]
        public string? AirlineLogoUrl { get; set; } // Matches nullable in Flight.cs

        [Display(Name = "User ID (Managed By - Optional)")]
        public int? UserId { get; set; } // Matches nullable in Flight.cs

        // Custom validation method (can also be done as a custom validation attribute)
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ArrivalTime <= DepartureTime)
            {
                yield return new ValidationResult(
                    "Arrival time must be after departure time.",
                    new[] { nameof(ArrivalTime), nameof(DepartureTime) });
            }
        }
    }
}
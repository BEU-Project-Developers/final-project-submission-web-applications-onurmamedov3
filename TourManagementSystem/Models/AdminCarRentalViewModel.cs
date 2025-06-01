// File: TourManagementSystem/Models/AdminCarRentalViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models // Or TourManagementSystem.Models
{
    public class AdminCarRentalViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Car Model/Name is required.")]
        [StringLength(200, ErrorMessage = "Car Model/Name cannot exceed 200 characters.")]
        [Display(Name = "Car Model/Name (e.g., Toyota Camry, SUV Large)")]
        public string CarModel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters.")]
        public string Company { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price per day is required.")]
        [DataType(DataType.Currency)]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be a positive value.")]
        [Display(Name = "Price Per Day")]
        public decimal PricePerDay { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(150, ErrorMessage = "Location cannot exceed 150 characters.")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Image URL (Optional)")]
        [Url(ErrorMessage = "Please enter a valid URL for the image.")]
        [StringLength(255)]
        public string? ImageUrl { get; set; }

        [Display(Name = "User ID (Managed By - Optional)")]
        public int? UserId { get; set; }

        // Fields like Description, PassengerCapacity, IsAvailable, Make, (separate) Model, Year
        // are removed from this ViewModel as they are not in your simplified CarRental entity.
    }
}
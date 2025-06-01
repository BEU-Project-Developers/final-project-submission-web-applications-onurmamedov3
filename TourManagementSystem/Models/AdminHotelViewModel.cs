// File: TourManagementSystem/Models/AdminHotelViewModel.cs
// OR TourManagementSystem/ViewModels/AdminHotelViewModel.cs (adjust namespace accordingly)
using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models // Or TourManagementSystem.ViewModels
{
    public class AdminHotelViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hotel name is required.")]
        [StringLength(100, ErrorMessage = "Hotel name cannot exceed 100 characters.")]
        [Display(Name = "Hotel Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Destination or City is required.")]
        [StringLength(100, ErrorMessage = "Destination cannot exceed 100 characters.")]
        public string Destination { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        [Display(Name = "Full Address (Optional)")]
        public string? Address { get; set; }

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price per night is required.")]
        [DataType(DataType.Currency)]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be a positive value and realistic.")]
        [Display(Name = "Price Per Night (€)")]
        public decimal PricePerNight { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Star rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Number of available rooms is required.")]
        [Range(0, 10000, ErrorMessage = "Available rooms must be between 0 and 10,000.")]
        [Display(Name = "Available Rooms")]
        public int AvailableRooms { get; set; }

        [Display(Name = "Primary Image URL (Optional)")]
        [StringLength(255, ErrorMessage = "Image URL is too long.")]
        [Url(ErrorMessage = "Please enter a valid URL for the image.")]
        public string? PrimaryImageUrl { get; set; }

        [Display(Name = "User ID (Managed By - Optional)")]
        public int? UserId { get; set; } // <<<< ENSURE THIS PROPERTY EXISTS
    }
}
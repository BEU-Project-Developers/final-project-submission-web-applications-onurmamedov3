// File: TourManagementSystem/Models/HotelViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models
{
    public class HotelViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hotel Name is required.")]
        [StringLength(100, ErrorMessage = "Hotel Name cannot exceed 100 characters.")]
        [Display(Name = "Hotel Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Destination/City is required.")]
        [StringLength(100, ErrorMessage = "Destination cannot exceed 100 characters.")]
        [Display(Name = "Destination (e.g., City, Area)")]
        public string Destination { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        [Display(Name = "Full Address (Optional)")]
        public string? Address { get; set; }

        [Display(Name = "Description (Optional)")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price per night is required.")]
        [DataType(DataType.Currency)]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10000.00.")]
        [Display(Name = "Price Per Night")]
        public decimal PricePerNight { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Star rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Number of available rooms is required.")]
        [Range(0, 1000, ErrorMessage = "Available rooms must be between 0 and 1000.")]
        [Display(Name = "Available Rooms")]
        public int AvailableRooms { get; set; }

        [Display(Name = "Image URL")]
        public string? PrimaryImageUrl { get; set; }
    }
}
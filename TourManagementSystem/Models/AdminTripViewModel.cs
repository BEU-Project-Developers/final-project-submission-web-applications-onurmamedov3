// File: TourManagementSystem/Models/AdminTripViewModel.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagementSystem.ViewModels // Or TourManagementSystem.Models
{
    public class AdminTripViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Trip title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Destination is required.")]
        [StringLength(200, ErrorMessage = "Destination cannot exceed 200 characters.")]
        public string Destination { get; set; } = string.Empty; // Could be multiple, comma-separated

        [Required(ErrorMessage = "Duration in days is required.")]
        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days.")]
        [Display(Name = "Duration (Days)")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [DataType(DataType.Currency)]
        [Range(0.01, 100000.00, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string? Description { get; set; }

        [Display(Name = "Image URL (Optional)")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        [StringLength(255)]
        public string? ImageUrl { get; set; }

        [Display(Name = "User ID (Managed By - Optional)")]
        public int? UserId { get; set; }
    }
}
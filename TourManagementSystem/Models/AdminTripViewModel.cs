using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema; // Not needed here

namespace TourManagementSystem.Models // Or TourManagementSystem.Models
{
    public class AdminTripViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Trip title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Destination is required.")]
        [StringLength(200, ErrorMessage = "Destination cannot exceed 200 characters.")]
        public string Destination { get; set; } = string.Empty;

        [Required(ErrorMessage = "Duration in days is required.")]
        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days.")]
        [Display(Name = "Duration (Days)")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [DataType(DataType.Currency)]
        [Range(0.01, 100000.00, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Description is required.")] // Making required to align with Trip entity initialization
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Image URL is required.")] // Making required
        [Display(Name = "Image URL")]
        //[Url(ErrorMessage = "Please enter a valid URL.")]
        [StringLength(255)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "User ID is required.")] // Matches non-nullable UserId in Trip
        [Display(Name = "User ID (Managed By)")]
        public int UserId { get; set; }
    }
}
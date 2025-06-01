// File: TourManagementSystem/Models/AdminActivityViewModel.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagementSystem.Models // Or TourManagementSystem.ViewModels
{
    public class AdminActivityViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Activity name is required.")]
        [StringLength(150, ErrorMessage = "Name cannot exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(150, ErrorMessage = "Location cannot exceed 150 characters.")]
        public string Location { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters.")]
        public string? Category { get; set; } // e.g., "Sightseeing", "Adventure", "Cultural"

        [Required(ErrorMessage = "Duration in hours is required.")]
        [Range(1, 48, ErrorMessage = "Duration must be between 1 and 48 hours.")] // Max 2 days for an activity
        [Display(Name = "Duration (Hours)")]
        public int DurationHours { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [DataType(DataType.Currency)]
        [Range(0.00, 10000.00, ErrorMessage = "Price must be a non-negative value.")] // Allow free activities
        // [Column(TypeName = "decimal(18,2)")] // Not needed in ViewModel
        public decimal Price { get; set; }

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Image URL is required, even if a placeholder.")] // Entity.ImageUrl is non-nullable
        [Url(ErrorMessage = "Please enter a valid URL.")]
        [StringLength(255)]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [Display(Name = "User ID (Managed By - Optional, but recommended if entity's UserId is non-nullable)")]
        public int? UserId { get; set; } // Making this nullable in VM for flexibility, service will handle default if entity is non-nullable
    }
}
// File: TourManagementSystem/Models/AdminCruiseViewModel.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // For Column attribute if needed

namespace TourManagementSystem.Models // Or TourManagementSystem.ViewModels if you prefer
{
    public class AdminCruiseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cruise line is required.")]
        [StringLength(100, ErrorMessage = "Cruise line name cannot exceed 100 characters.")]
        [Display(Name = "Cruise Line")]
        public string CruiseLine { get; set; } = string.Empty;

        // ShipName is not in your Cruise.cs entity based on previous files.
        // If you want to add it, first add it to Cruise.cs, migrate, then add here.
        // [StringLength(100)]
        // [Display(Name = "Ship Name (Optional)")]
        // public string? ShipName { get; set; }

        [Required(ErrorMessage = "Departure port is required.")]
        [StringLength(100, ErrorMessage = "Departure port cannot exceed 100 characters.")]
        [Display(Name = "Departure Port")]
        public string DeparturePort { get; set; } = string.Empty;

        [Required(ErrorMessage = "Destination region/ports are required.")]
        [StringLength(200, ErrorMessage = "Destination cannot exceed 200 characters.")]
        public string Destination { get; set; } = string.Empty;

        [Required(ErrorMessage = "Duration in days is required.")]
        [Range(1, 100, ErrorMessage = "Duration must be between 1 and 100 days.")]
        [Display(Name = "Duration (Days)")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [DataType(DataType.Currency)]
        [Range(0.01, 100000.00, ErrorMessage = "Price must be a positive value.")]
        // [Column(TypeName = "decimal(18,2)")] // Not needed in ViewModel
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Image URL is required, even if a placeholder.")] // Entity.ImageUrl is non-nullable
        //[Url(ErrorMessage = "Please enter a valid URL for the image.")]
        [StringLength(255)]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = string.Empty; // Default to empty, form needs a value

        // ItinerarySummary is generated in your public ViewModel,
        // but admin might want to input a base summary or key points.
        [Display(Name = "Itinerary Summary / Key Features (Optional)")]
        [StringLength(500)]
        public string? ItinerarySummary { get; set; }

        [Required(ErrorMessage = "User ID is required for this cruise listing.")] // Entity.UserId is non-nullable
        [Display(Name = "User ID (Managed By)")]
        public int UserId { get; set; }
    }
}
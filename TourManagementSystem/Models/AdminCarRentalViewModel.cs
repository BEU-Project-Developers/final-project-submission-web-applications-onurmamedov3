using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models // Or TourManagementSystem.ViewModels
{
    public class AdminCarRentalViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Car Model/Name is required.")]
        [StringLength(100, ErrorMessage = "Car Model/Name cannot exceed 100 characters.")]
        [Display(Name = "Car Model/Name")]
        public string CarModel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters.")]
        [Display(Name = "Rental Company")]
        public string Company { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(150, ErrorMessage = "Location cannot exceed 150 characters.")]
        [Display(Name = "Pickup Location")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price per day is required.")]
        [DataType(DataType.Currency)]
        [Range(0.01, 5000.00, ErrorMessage = "Price must be a positive value and realistic.")]
        [Display(Name = "Price Per Day (€)")]
        public decimal PricePerDay { get; set; }

        [Required(ErrorMessage = "Image URL is required.")]
        [StringLength(255, ErrorMessage = "Image URL is too long.")]
        //[Url(ErrorMessage = "Please enter a valid image URL (e.g., http://... or /images/...).")]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "User ID is required.")]
        [Display(Name = "Managed By User ID")]
        public int UserId { get; set; } // Assuming this is always required to be set for a CarRental
    }
}
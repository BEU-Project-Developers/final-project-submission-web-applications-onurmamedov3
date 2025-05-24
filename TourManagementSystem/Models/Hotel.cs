// File: TourManagementSystem/Models/Hotel.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagementSystem.Models
{
    public class Hotel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hotel Name is required.")]
        [StringLength(100, ErrorMessage = "Hotel Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Destination/City is required.")]
        [StringLength(100, ErrorMessage = "Destination cannot exceed 100 characters.")]
        public string Destination { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string? Address { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Price per night is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10000.00.")]
        public decimal PricePerNight { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Star rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Number of available rooms is required.")]
        [Range(0, 1000, ErrorMessage = "Available rooms must be between 0 and 1000.")]
        public int AvailableRooms { get; set; }

        [StringLength(255)] // Max length for a URL/path
        public string? PrimaryImageUrl { get; set; } // Stores the path to the image in wwwroot

        // UserId to link to the admin/user who added it (optional, adjust as needed)
        public int? UserId { get; set; }
        public virtual User? User { get; set; } // Assuming you have a User model
    }
}
using System.ComponentModel.DataAnnotations; // For [Required]

namespace TourManagementSystem.Models
{
    public class CarRental
    {
        public int Id { get; set; }

        [Required] // You have this, so CarModel is definitely required
        public string CarModel { get; set; } = string.Empty;

        // For simplicity, if these are always needed, add [Required]
        // If they can be null in DB, make them string? or decimal?
        public string Company { get; set; } = string.Empty;
        public decimal PricePerDay { get; set; }
        public string Location { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty; // Assuming URL is always present

        public int UserId { get; set; } // Non-nullable, so a User is always associated
        public virtual User User { get; set; } = null!; // Navigation property, init to avoid warning if non-nullable FK
    }
}
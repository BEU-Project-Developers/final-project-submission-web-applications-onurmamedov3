using System;
using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OfferId { get; set; }

        [Required]
        [MaxLength(50)]
        public string OfferType { get; set; }

        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name must contain only letters and spaces.")]
        public string FullName { get; set; }

        [Required]
        [MaxLength(200)]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string CustomerEmail { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public DateTime BookingDateTime { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string Status { get; set; } = "Confirmed";
    }
}
// File: TourManagementSystem/Models/BookingSubmissionViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models
{
    public class BookingSubmissionViewModel
    {
        [Required]
        public int OfferId { get; set; }

        [Required]
        public string OfferType { get; set; } // "Hotel", "CarRental", "Flight", etc.

        [Required(ErrorMessage = "Your name is required.")]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Your email is required.")]
        [EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;

        // NumberOfAdults and NumberOfChildren are removed for simplicity
    }
}
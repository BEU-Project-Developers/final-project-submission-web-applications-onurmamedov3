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
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Your email is required.")]
        [EmailAddress]
        public string CustomerEmail { get; set; }

        // Add other fields as needed:
        // For hotels: NumberOfRooms, SpecialRequests
        // For flights: PassengerDetails (List<PassengerInfo>)
        // For generic: NumberOfTravelers (if applicable across types)
        public int NumberOfAdults { get; set; } = 1;
        public int NumberOfChildren { get; set; } = 0;

        // You might have specific properties for each offer type if the booking form varies greatly
        // Or common properties here and fetch specific offer details in the controller
    }
}
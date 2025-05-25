// File: TourManagementSystem/Models/CarRentalViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models
{
    public class CarRentalViewModel
    {
        public int Id { get; set; }
        public string CarModel { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal PricePerDay { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } // Make nullable if image can be absent

        // You might add other properties like:
        // public string? Description { get; set; }
        // public int PassengerCapacity { get; set; }
        // public string? TransmissionType { get; set; } // Automatic, Manual
        // public bool HasAirConditioning { get; set; }
    }
}
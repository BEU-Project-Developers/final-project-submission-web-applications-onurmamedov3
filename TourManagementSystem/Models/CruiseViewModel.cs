// File: TourManagementSystem/Models/CruiseViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models
{
    public class CruiseViewModel
    {
        public int Id { get; set; }
        public string CruiseLine { get; set; } = string.Empty;
        public string DeparturePort { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty; // Can be a region like "Caribbean" or specific ports
        public int DurationDays { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; } // Make nullable if image can be absent
        public string? ItinerarySummary { get; set; } // e.g., "7-Day Eastern Caribbean Cruise"
    }
}
// File: TourManagementSystem/Models/ActivityViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models
{
    public class ActivityViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Category { get; set; }
        public int DurationHours { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; } // For display purposes, can be a stock image or category image
    }
}
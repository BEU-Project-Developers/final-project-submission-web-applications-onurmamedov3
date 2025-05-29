// File: TourManagementSystem/Models/TripViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models
{
    public class TripViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Destination { get; set; }
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        // Add any other properties you want to display from the Trip entity
    }
}
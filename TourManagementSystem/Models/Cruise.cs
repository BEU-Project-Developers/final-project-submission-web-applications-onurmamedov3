using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models
{
    public class Cruise
    {
        public int Id { get; set; }
        [Required]
        public string CruiseLine { get; set; } // Searchable
        public string DeparturePort { get; set; } // Searchable
        public string Destination { get; set; }   // Searchable
        public int DurationDays { get; set; }
        public decimal Price { get; set; }         // Searchable by price range
        public string ImageUrl { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
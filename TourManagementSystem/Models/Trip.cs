using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models
{
    public class Trip
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }       // Searchable by name/title
        public string Destination { get; set; } // Searchable
        public int DurationDays { get; set; }
        public decimal Price { get; set; }         // Searchable by price range
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
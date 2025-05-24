using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models
{
    public class Flight
    {
        public int Id { get; set; }
        [Required]
        public string Airline { get; set; }
        public string DepartureCity { get; set; } // Searchable
        public string ArrivalCity { get; set; }   // Searchable
        public DateTime DepartureTime { get; set; } // Searchable by date
        public DateTime ArrivalTime { get; set; }
        public decimal Price { get; set; }         // Searchable by price range
        public int UserId { get; set; }            // Likely for admin/linking, not user search
        public User User { get; set; }
    }
}
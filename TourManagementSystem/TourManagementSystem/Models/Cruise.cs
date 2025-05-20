using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models
{
    public class Cruise
    {
        public int Id { get; set; }

        [Required]
        public string CruiseLine { get; set; }

        public string DeparturePort { get; set; }

        public string Destination { get; set; }

        public int DurationDays { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }

}
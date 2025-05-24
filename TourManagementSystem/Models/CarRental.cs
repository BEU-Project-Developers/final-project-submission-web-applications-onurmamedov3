using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models
{
    public class CarRental
    {
        public int Id { get; set; }
        [Required]
        public string CarModel { get; set; } // Searchable
        public string Company { get; set; }  // Searchable
        public decimal PricePerDay { get; set; } // Searchable by price range
        public string Location { get; set; }    // Searchable (city/pickup point)
        public string ImageUrl { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
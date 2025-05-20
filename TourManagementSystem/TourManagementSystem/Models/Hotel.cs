using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models
{
    public class Hotel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Location { get; set; }

        public decimal PricePerNight { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        // Foreign key to User
        public int UserId { get; set; }
        public User User { get; set; }
    }

}
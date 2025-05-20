using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models
{
   
    public class CarRental
    {
        public int Id { get; set; }

        [Required]
        public string CarModel { get; set; }

        public string Company { get; set; }

        public decimal PricePerDay { get; set; }

        public string Location { get; set; }

        public string ImageUrl { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }

}
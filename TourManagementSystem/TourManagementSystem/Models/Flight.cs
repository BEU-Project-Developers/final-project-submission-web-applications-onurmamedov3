using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models
{
   
    public class Flight
    {
        public int Id { get; set; }

        [Required]
        public string Airline { get; set; }

        public string DepartureCity { get; set; }

        public string ArrivalCity { get; set; }

        public DateTime DepartureTime { get; set; }

        public DateTime ArrivalTime { get; set; }

        public decimal Price { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }

}
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
// using Microsoft.AspNetCore.Identity; // REMOVE THIS LINE

namespace TourManagementSystem.Models
{
    public class User // Does NOT inherit from IdentityUser
    {
        [Key]
        public int Id { get; set; } // Keep your int Id

        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty; // Keep your Email

        [Required]
        public string PasswordHash { get; set; } = string.Empty; // Keep your PasswordHash

        [StringLength(50)]
        public string Role { get; set; } = "User"; // Keep your Role, default to "User"

        public DateTime DateRegistered { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
        public virtual ICollection<CarRental> CarRentals { get; set; } = new List<CarRental>();
        public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public virtual ICollection<Cruise> Cruises { get; set; } = new List<Cruise>();
        public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}
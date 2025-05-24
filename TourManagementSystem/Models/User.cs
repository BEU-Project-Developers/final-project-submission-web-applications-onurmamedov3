// In TourManagementSystem.Models.User.cs
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using TourActivity = TourManagementSystem.Models.Activity;


namespace TourManagementSystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)] // Consider adding a max length
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Changed from Password to PasswordHash

        [StringLength(50)] // Max length for role name
        public string Role { get; set; } = "User"; // Default role

        public DateTime DateRegistered { get; set; } = DateTime.UtcNow; // Good to have

        // Navigation properties (EF Core will try to create relationships)
        // Initialize collections to prevent null reference exceptions
        public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
        public virtual ICollection<CarRental> CarRentals { get; set; } = new List<CarRental>();
        public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public virtual ICollection<Cruise> Cruises { get; set; } = new List<Cruise>();
         public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}
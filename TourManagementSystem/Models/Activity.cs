// File: TourManagementSystem/Models/Activity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagementSystem.Models
{
    public class Activity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } // e.g., "City Tour", "Museum Visit", "Hiking Trip" (Searchable)

        [Required]
        [StringLength(150)]
        public string Location { get; set; } // e.g., "Paris", "Grand Canyon" (Searchable)

        [StringLength(100)]
        public string? Category { get; set; } // e.g., "Sightseeing", "Adventure", "Cultural" (Searchable)

        public int DurationHours { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } // Searchable

        
        public string? Description { get; set; }

        //public string? ImageUrl { get; set; }

        // Optional: Link to a User if activities are added by specific users/admins
        public int? UserId { get; set; } // Nullable if not always tied to a user
        public virtual User? User { get; set; }
    }
}
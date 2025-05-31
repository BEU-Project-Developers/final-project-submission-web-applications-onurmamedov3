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
        public string Name { get; set; }

        [Required]
        [StringLength(150)]
        public string Location { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        public int DurationHours { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string ImageUrl { get; set; } 

        public int? UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
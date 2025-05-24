// File: TourManagementSystem/Models/HotelImage.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagementSystem.Models
{
    public class HotelImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; }

        [StringLength(100)]
        public string? Caption { get; set; }

        // Foreign Key to Hotel
        public int HotelId { get; set; }
        [ForeignKey("HotelId")]
        public virtual Hotel Hotel { get; set; }
    }
}
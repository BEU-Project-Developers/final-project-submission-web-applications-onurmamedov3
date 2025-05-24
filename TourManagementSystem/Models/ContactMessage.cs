// File: TourManagementSystem/Models/ContactMessage.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace TourManagementSystem.Models
{
    public class ContactMessage
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters.")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please, write us a message.")]
        public string Message { get; set; } = string.Empty; // Will map to longtext by default

        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false; // To track if an admin has reviewed it
    }
}
// File: TourManagementSystem/Services/ContactService.cs
using System;
using System.Threading.Tasks;
using TourManagementSystem.Data;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public class ContactService : IContactService
    {
        private readonly ApplicationDbContext _context;

        public ContactService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string ErrorMessage)> SaveContactMessageAsync(ContactMessageViewModel model)
        {
            try
            {
                var contactMessage = new ContactMessage
                {
                    Name = model.Name,
                    Email = model.Email,
                    Subject = model.Subject,
                    Message = model.Message,
                    SubmittedDate = DateTime.UtcNow,
                    IsRead = false
                };

                _context.ContactMessages.Add(contactMessage);
                await _context.SaveChangesAsync();
                return (true, "Your message has been sent successfully!");
            }
            catch (Exception ex)
            {
                // Log the exception (ex.ToString())
                System.Diagnostics.Debug.WriteLine($"Error saving contact message: {ex.ToString()}");
                return (false, "An error occurred while sending your message. Please try again later.");
            }
        }
    }
}
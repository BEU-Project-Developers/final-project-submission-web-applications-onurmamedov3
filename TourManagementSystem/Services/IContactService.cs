// File: TourManagementSystem/Services/IContactService.cs
using System.Threading.Tasks;
using TourManagementSystem.Models; // For ContactMessage and a ViewModel if you create one

namespace TourManagementSystem.Services
{
    public interface IContactService
    {
        // Takes a ViewModel representing the form data
        Task<(bool Success, string ErrorMessage)> SaveContactMessageAsync(ContactMessageViewModel model);
        // Task<IEnumerable<ContactMessage>> GetAllMessagesAsync(); // For admin to view later
    }
}
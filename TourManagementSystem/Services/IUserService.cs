// File: TourManagementSystem/Services/IUserService.cs
using System.Threading.Tasks;
using TourManagementSystem.Models; // For User model and RegisterViewModel

namespace TourManagementSystem.Services
{
    public interface IUserService
    {
        // Registers a new user.
        // Returns: A tuple indicating success, an error message (if any), and the created User object.
        Task<(bool Success, string ErrorMessage, User? CreatedUser)> RegisterUserAsync(RegisterViewModel model);

        // Validates user credentials for login.
        // Returns: The User object if valid, otherwise null.
        Task<User?> ValidateUserAsync(string email, string password);

        // Example for a delete operation (we can implement this later if you want)
        // Task<bool> DeleteUserAsync(int userId); 
    }
}
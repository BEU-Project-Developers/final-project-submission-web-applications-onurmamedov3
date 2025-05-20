// File: TourManagementSystem/Services/UserService.cs
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TourManagementSystem.Data;
using TourManagementSystem.Models;
using BCrypt.Net; // Ensure you have the BCrypt.Net NuGet package installed

namespace TourManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string ErrorMessage, User? CreatedUser)> RegisterUserAsync(RegisterViewModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                return (false, "An account with this email address already exists.", null);
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                // Now BCrypt.HashPassword should be found
                PasswordHash = global::BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "User", 
                DateRegistered = DateTime.UtcNow
            };

            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
                return (true, string.Empty, user);
            }
            catch (DbUpdateException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString()); 
                return (false, "An error occurred while creating your account. Please try again later.", null);
            }
        }

        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                // Now BCrypt.Verify should be found
                if (global::BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) 
                {
                    return user; 
                }
            }
            return null; 
        }
    }
}
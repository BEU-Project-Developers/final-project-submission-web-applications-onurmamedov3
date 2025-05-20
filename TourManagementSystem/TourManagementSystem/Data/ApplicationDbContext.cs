// File: TourManagementSystem/Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models; // Assuming User.cs is in the Models folder

namespace TourManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        // Add other DbSets here for your other entities like Hotel, CarRental, Flight, etc.
        // public DbSet<Hotel> Hotels { get; set; } 
        // public DbSet<CarRental> CarRentals { get; set; }
        // ...

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity (example: ensure Email is unique)
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                // Add other configurations for User or other entities if needed
                // For example, if your User model had a Username distinct from Email:
                // entity.HasIndex(e => e.Username).IsUnique();
            });
        }
    }
}
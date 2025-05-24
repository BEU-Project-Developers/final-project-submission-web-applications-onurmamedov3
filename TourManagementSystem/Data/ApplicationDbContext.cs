// File: TourManagementSystem/Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models; // Make sure this matches your Models namespace
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // If using ASP.NET Core Identity

namespace TourManagementSystem.Data
{
    // If using ASP.NET Core Identity, inherit from IdentityDbContext<User> or your custom User class
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } // Assuming User model exists
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Flight> Flights { get; set; } // Assuming Flight model exists
        public DbSet<CarRental> CarRentals { get; set; } // Assuming CarRental model exists
        public DbSet<Cruise> Cruises { get; set; } // Assuming Cruise model exists
        public DbSet<Trip> Trips { get; set; } // Assuming Trip model exists
        public DbSet<Activity> Activities { get; set; } // Assuming Activity model exists

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Important if using Identity

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                // Add other User configurations if needed
            });

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.HasIndex(e => e.Name);
                entity.Property(p => p.PricePerNight).HasColumnType("decimal(18,2)");

                // Example for UserId foreign key if Hotel.UserId is not nullable
                // if (entity.Metadata.FindNavigation(nameof(Hotel.User)) != null)
                // {
                //     entity.HasOne(h => h.User)
                //           .WithMany() // Or .WithMany(u => u.Hotels) if User has ICollection<Hotel>
                //           .HasForeignKey(h => h.UserId)
                //           .IsRequired() // If UserId is not nullable
                //           .OnDelete(DeleteBehavior.Restrict); // Choose appropriate delete behavior
                // }
            });

            modelBuilder.Entity<Flight>(entity =>
            {
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<CarRental>(entity =>
            {
                entity.Property(p => p.PricePerDay).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Cruise>(entity =>
            {
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Activity>(entity =>
            {
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });

            // Example Seed Data (Optional, requires a new migration if added/changed)
            // modelBuilder.Entity<Hotel>().HasData(
            //     new Hotel {
            //         Id = 1, Name = "Seed Hotel Paris", Destination = "Paris", Address = "1 Rue de Seed",
            //         Description = "A lovely seeded hotel in Paris.", PricePerNight = 150.00m, Rating = 4, AvailableRooms = 20,
            //         PrimaryImageUrl = "~/images/offer_1.jpg" // Ensure image exists
            //     },
            //     new Hotel {
            //         Id = 2, Name = "Seed Hotel London", Destination = "London", Address = "2 Seedling Street",
            //         Description = "Comfortable seeded accommodation in London.", PricePerNight = 120.50m, Rating = 3, AvailableRooms = 15,
            //         PrimaryImageUrl = "~/images/offer_4.jpg" // Ensure image exists
            //     }
            // );
        }
    }
}
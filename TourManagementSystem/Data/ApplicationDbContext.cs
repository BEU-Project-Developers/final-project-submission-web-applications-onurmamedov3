using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<CarRental> CarRentals { get; set; } // Ensure this DbSet is present
        public DbSet<Cruise> Cruises { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        // Add other DbSets here

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- START: CarRental Specific Fluent API Configuration ---
            // In ApplicationDbContext.OnModelCreating
            modelBuilder.Entity<CarRental>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CarModel).IsRequired().HasMaxLength(100); // If using annotations on entity, this is redundant but harmless
                entity.Property(e => e.Company).IsRequired().HasMaxLength(100); // Add IsRequired if not on entity
                entity.Property(e => e.Location).IsRequired().HasMaxLength(150); // Add IsRequired if not on entity
                entity.Property(e => e.PricePerDay).IsRequired().HasColumnType("decimal(18,2)"); // Add IsRequired if not on entity
                entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(255); // Add IsRequired if not on entity

                entity.HasOne(cr => cr.User)
                      .WithMany(u => u.CarRentals) // Assumes User has ICollection<CarRental> CarRentals
                      .HasForeignKey(cr => cr.UserId) // CarRental.UserId is non-nullable int
                      .IsRequired() // Foreign key is required
                      .OnDelete(DeleteBehavior.Cascade); // Or Restrict, if a User is deleted, associated CarRentals are deleted. Choose carefully.
            });
            // --- END: CarRental Specific Fluent API Configuration ---


            // --- YOUR EXISTING OnModelCreating CONFIGURATIONS FOR OTHER ENTITIES ---
            // --- (User, Trip, Hotel, Flight, etc.) should remain here if they exist. ---
            // Example:
            modelBuilder.Entity<User>(entity =>
            {
                // Your existing configurations for User
                // e.g., entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                // Define the required relationship with User
                entity.HasOne(t => t.User)
                      .WithMany(u => u.Trips) // Assumes User.cs has ICollection<Trip> Trips
                      .HasForeignKey(t => t.UserId)
                      .IsRequired() // Because Trip.UserId is non-nullable
                      .OnDelete(DeleteBehavior.Cascade); // Or Restrict, consider implications
            });

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.PricePerNight).HasColumnType("decimal(18,2)");
                entity.HasOne(h => h.User)
                   .WithMany(u => u.Hotels)
                   .HasForeignKey(h => h.UserId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Flight>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                // Relationship with User
                entity.HasOne(f => f.User)
                      .WithMany(u => u.Flights) // Assumes User.cs has ICollection<Flight> Flights
                      .HasForeignKey(f => f.UserId)
                      .IsRequired(false) // Since Flight.UserId is nullable
                      .OnDelete(DeleteBehavior.SetNull); // Or your preferred delete behavior
            });

            modelBuilder.Entity<Cruise>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.HasOne(c => c.User)
                    .WithMany(u => u.Cruises)
                    .HasForeignKey(c => c.UserId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Activity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.HasOne(a => a.User)
                    .WithMany(u => u.Activities)
                    .HasForeignKey(a => a.UserId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<ContactMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Subject).HasMaxLength(200);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(4000);
                entity.Property(e => e.SubmittedDate).IsRequired();
                entity.HasIndex(e => e.SubmittedDate);
            });
            // ... and so on for all your other entities ...
        }
    }
}
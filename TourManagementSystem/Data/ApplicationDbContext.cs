using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Data
{
    public class ApplicationDbContext : DbContext // Does NOT inherit from IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } // Your custom User DbSet
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<CarRental> CarRentals { get; set; }
        public DbSet<Cruise> Cruises { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                // Your User.Role property is just a string here.
                // If you have a separate Roles table, you'd define that relationship.
            });

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.HasIndex(e => e.Name);
                entity.Property(p => p.PricePerNight).HasColumnType("decimal(18,2)");
                // Assuming Hotel.UserId is int? and refers to User.Id (int)
                if (entity.Metadata.FindNavigation(nameof(Hotel.User)) != null)
                {
                    entity.HasOne(h => h.User)
                          .WithMany(u => u.Hotels) // Assumes User has ICollection<Hotel> Hotels
                          .HasForeignKey(h => h.UserId) // Ensure Hotel.UserId is int or int?
                          .OnDelete(DeleteBehavior.SetNull); // Or Restrict, Cascade as per your needs
                }
            });

            modelBuilder.Entity<CarRental>(entity =>
            {
                entity.Property(p => p.PricePerDay).HasColumnType("decimal(18,2)");
                if (entity.Metadata.FindNavigation(nameof(CarRental.User)) != null)
                {
                    entity.HasOne(cr => cr.User)
                          .WithMany(u => u.CarRentals)
                          .HasForeignKey(cr => cr.UserId)
                          .OnDelete(DeleteBehavior.SetNull);
                }
            });

            modelBuilder.Entity<Flight>(entity =>
            {
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                if (entity.Metadata.FindNavigation(nameof(Flight.User)) != null)
                {
                    entity.HasOne(fl => fl.User)
                          .WithMany(u => u.Flights)
                          .HasForeignKey(fl => fl.UserId)
                          .OnDelete(DeleteBehavior.SetNull);
                }
            });

            modelBuilder.Entity<Cruise>(entity =>
            {
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                if (entity.Metadata.FindNavigation(nameof(Cruise.User)) != null)
                {
                    entity.HasOne(c => c.User)
                          .WithMany(u => u.Cruises)
                          .HasForeignKey(c => c.UserId)
                          .OnDelete(DeleteBehavior.SetNull);
                }
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                if (entity.Metadata.FindNavigation(nameof(Trip.User)) != null)
                {
                    entity.HasOne(t => t.User)
                          .WithMany(u => u.Trips)
                          .HasForeignKey(t => t.UserId)
                          .OnDelete(DeleteBehavior.SetNull);
                }
            });

            modelBuilder.Entity<Activity>(entity =>
            {
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                if (entity.Metadata.FindNavigation(nameof(Activity.User)) != null)
                {
                    entity.HasOne(a => a.User)
                          .WithMany(u => u.Activities)
                          .HasForeignKey(a => a.UserId)
                          .OnDelete(DeleteBehavior.SetNull);
                }
            });


            modelBuilder.Entity<ContactMessage>(entity =>
            {
                entity.HasIndex(e => e.SubmittedDate);
            });
        }
    }
}
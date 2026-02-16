using CarServiceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace CarServiceTracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; } = null!;
        public DbSet<ServiceRecord> ServiceRecords { get; set; } = null!;
        public DbSet<ServiceType> ServiceTypes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fix: decimal precision for Price (removes EF warning)
            modelBuilder.Entity<ServiceRecord>()
                .Property(sr => sr.Price)
                .HasPrecision(18, 2);

            // Seed ServiceTypes
            modelBuilder.Entity<ServiceType>().HasData(
                new ServiceType { Id = 1, Name = "Oil Change" },
                new ServiceType { Id = 2, Name = "Oil Filter" },
                new ServiceType { Id = 3, Name = "Air Filter" },
                new ServiceType { Id = 4, Name = "Cabin Filter" },
                new ServiceType { Id = 5, Name = "Brake Pads" },
                new ServiceType { Id = 6, Name = "Brake Discs" },
                new ServiceType { Id = 7, Name = "Battery" },
                new ServiceType { Id = 8, Name = "Diagnostics" },
                new ServiceType { Id = 9, Name = "Tires" },
                new ServiceType { Id = 10, Name = "Other" }
            );
        }
    }
}

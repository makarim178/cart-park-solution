using CarPark.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Infrastructure
{
    public class CarParkDbContext : DbContext
    {
        public CarParkDbContext(DbContextOptions<CarParkDbContext> options) : base(options) { }

        public DbSet<ParkingSpace> ParkingSpaces { get; set; }
        public DbSet<ParkingRecord> ParkingRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed 20 parking spaces (you can adjust the number)
            var seed = Enumerable.Range(1, 20)
                .Select(i => new ParkingSpace { Id = i, SpaceNumber = i, IsOccupied = false })
                .ToArray();
            modelBuilder.Entity<ParkingSpace>().HasData(seed);

            base.OnModelCreating(modelBuilder);
        }
    }
}

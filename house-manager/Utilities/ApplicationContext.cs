using house_manager.Models;
using Microsoft.EntityFrameworkCore;

namespace house_manager.Utilities
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
        }

        public virtual DbSet<House> Houses { get; set; } = null!;
        public virtual DbSet<Apartment> Apartments { get; set; } = null!;
        public virtual DbSet<Lodger> Lodgers { get; set; } = null!;
        public virtual DbSet<ParkingSpace> ParkingSpaces { get; set; } = null!;
        public virtual DbSet<Car> Cars { get; set; } = null!;
        public virtual DbSet<OwnedApartment> OwnedApartments { get; set; } = null!;
        public virtual DbSet<OwnedCar> OwnedCars { get; set; } = null!;
        public virtual DbSet<OwnedParkingSpace> OwnedParkingSpaces { get; set; } = null!;
    }
}

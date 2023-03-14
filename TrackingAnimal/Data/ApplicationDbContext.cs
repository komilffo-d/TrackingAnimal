using Microsoft.EntityFrameworkCore;
using TrackingAnimal.Models;

namespace TrackingAnimal.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
            Database.EnsureDeleted();
            Database.EnsureCreated();
            
        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<LocationPoint> Locations { get; set; }
        public DbSet<AnimalType> AnimalTypes { get; set; }
        public DbSet<LocationVisitedAnimal> locationVisitedAnimals { get; set; }
        public DbSet<Animal> Animals { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasData(
                new Account()
                {
                    Id = 1,
                    firstName = "Daniil",
                    lastName = "Korepanov",
                    email = "123@gmail.com",
                    password = "12345678"
                },
                new Account()
                {
                    Id = 2,
                    firstName = "Oleg",
                    lastName = "Nechaev",
                    email = "Hello@gmail.com",
                    password = "abcdefghi"
                });
        }
    }
}

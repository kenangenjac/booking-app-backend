using blandus_backend.Models;
using blandus_backend.Models.Accommodation;
using blandus_backend.Models.DatesOccupied;
using blandus_backend.Models.Image;
using blandus_backend.Models.Reservation;
using blandus_backend.Models.Review;
using blandus_backend.Models.Service;
using blandus_backend.Models.User;
using Microsoft.EntityFrameworkCore;

namespace blandus_backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Accommodation> Accommodations { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<DatesOccupied> DatesOccupied { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accommodation>()
                .HasOne(a => a.User)
                .WithMany(a => a.Accommodations)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

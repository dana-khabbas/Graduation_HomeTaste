using graduation_proj.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace graduation_proj.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<HostProfile> HostProfiles { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // CRITICAL for Identity

            // 1. Fix Review -> ApplicationUser
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Guest)
                .WithMany()
                .HasForeignKey(r => r.GuestId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2. Fix Review -> Dish 
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Dish)
                .WithMany() // Change to .WithMany(d => d.Reviews) if your Dish model has a public List<Review> Reviews property
                .HasForeignKey(r => r.DishId)
                .OnDelete(DeleteBehavior.NoAction); // Prevents multiple cascade paths
        }
    }
}
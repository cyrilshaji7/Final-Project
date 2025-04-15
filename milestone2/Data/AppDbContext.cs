using milestone2.Models;
using Microsoft.EntityFrameworkCore;

namespace milestone2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>()
                .Property(n => n.SentAt)
                .HasColumnType("datetime2"); 

            base.OnModelCreating(modelBuilder);
        }
    }
}

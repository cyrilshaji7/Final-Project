using milestone2.Models;
using Microsoft.EntityFrameworkCore;

namespace milestone2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<WaitlistEntry> WaitlistEntries => Set<WaitlistEntry>();
        public DbSet<Reservation> Reservations => Set<Reservation>();

        public DbSet<Loan> Loans => Set<Loan>();
        public DbSet<User> Users => Set<User>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>()
                .Property(n => n.SentAt)
                .HasColumnType("datetime2"); 

            base.OnModelCreating(modelBuilder);
        }
    }



}

using ConcertTicketApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcertTicketApi.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<TicketType> TicketTypes { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ← Add this to specify precision/scale for your Price column
            builder.Entity<TicketType>()
                   .Property(tt => tt.Price)
                   .HasPrecision(18, 2);
        }
    }
}

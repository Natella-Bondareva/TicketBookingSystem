using Microsoft.EntityFrameworkCore;
using TicketBookingSystem.Models.Entities;
using RouteEntity = TicketBookingSystem.Models.Entities.Route;

namespace TicketBookingSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<RouteEntity> Routes { get; set; }
        public DbSet<RouteStop> RouteStops { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ScheduleStop> ScheduleStops { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RouteStop>()
                .HasKey(rs => new { rs.RouteId, rs.StationId });

            modelBuilder.Entity<ScheduleStop>()
                .HasKey(ss => new { ss.ScheduleId, ss.StationId });

            // Додаткові зв'язки
            modelBuilder.Entity<RouteEntity>()
                .HasOne(r => r.StartStation)
                .WithMany()
                .HasForeignKey(r => r.StartStationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Booking)
                .WithOne(b => b.Ticket)
                .HasForeignKey<Ticket>(t => t.BookingId);

            modelBuilder.Entity<RouteEntity>()
                .HasOne(r => r.EndStation)
                .WithMany()
                .HasForeignKey(r => r.EndStationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking.Status check constraint
            modelBuilder.Entity<Booking>()
                .Property(b => b.Status)
                .HasMaxLength(10);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
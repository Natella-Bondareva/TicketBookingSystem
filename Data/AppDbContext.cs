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
        public DbSet<BookingStatus> BookingStatuses { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Seat> Seats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RouteStop>()
                .HasKey(rs => new { rs.RouteId, rs.StationId });

            modelBuilder.Entity<ScheduleStop>()
                .HasKey(ss => new { ss.ScheduleId, ss.StationId });

            // Додаткові зв'язки
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Route)
                .WithMany(r => r.Schedules)
                .HasForeignKey(s => s.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RouteStop>()
                .HasOne(rs => rs.Route)
                .WithMany(r => r.RouteStops)
                .HasForeignKey(rs => rs.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ScheduleStop>()
                .HasOne(ss => ss.Schedule)
                .WithMany(s => s.ScheduleStops)
                .HasForeignKey(ss => ss.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RouteEntity>()
                .HasOne(r => r.StartStation)
                .WithMany()
                .HasForeignKey(r => r.StartStationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Booking)
                .WithOne(b => b.Ticket)
                .HasForeignKey<Ticket>(t => t.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RouteEntity>()
                .HasOne(r => r.EndStation)
                .WithMany()
                .HasForeignKey(r => r.EndStationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RouteEntity>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Schedule>()
                .Property(s => s.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Station>()
                .Property(s => s.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Seat>()
                .HasIndex(s => new { s.ScheduleId, s.SeatCode })
                .IsUnique(); 

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Seat)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Booking>()
                .HasOne(b => b.BookingStatus)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.BookingStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.PaymentMethod)
                .WithMany(m => m.Payments)
                .HasForeignKey(p => p.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "client" },
                new Role { Id = 2, Name = "cashier" },
                new Role { Id = 3, Name = "admin" }
                );

            modelBuilder.Entity<PaymentMethod>().HasData(
                new PaymentMethod { Id = 1, Name = "cash" },
                new PaymentMethod { Id = 2, Name = "card" }
                );

            modelBuilder.Entity<BookingStatus>().HasData(
                new BookingStatus { Id = 1, Name = "cancelled" },
                new BookingStatus { Id = 2, Name = "active" },
                new BookingStatus { Id = 3, Name = "completed" }
                );
            modelBuilder.Entity<Schedule>()
                .Property(s => s.Date)
                .HasColumnType("date");

        }
    }
}
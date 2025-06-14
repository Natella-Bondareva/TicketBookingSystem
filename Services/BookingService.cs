using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Data;
using TicketBookingSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;
using TicketBookingSystem.Services;

namespace TicketBookingSystem.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;
        private readonly IAutoSeatService _autoSeatService;

        public BookingService(AppDbContext context, IAutoSeatService autoSeatService)
        {
            _context = context;
            _autoSeatService = autoSeatService;
        }

        public async Task<BookingResponseDto> CreateBookingAsync(AutoBookingRequestDto dto)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Route).ThenInclude(r => r.RouteStops)
                .FirstOrDefaultAsync(s => s.Id == dto.ScheduleId);

            if (schedule == null)
                throw new InvalidOperationException("Розклад не знайдено.");

            var fromStop = schedule.Route.RouteStops.FirstOrDefault(s => s.StationId == dto.FromStationId);
            var toStop = schedule.Route.RouteStops.FirstOrDefault(s => s.StationId == dto.ToStationId);

            if (fromStop == null || toStop == null || fromStop.StopOrder >= toStop.StopOrder)
                throw new InvalidOperationException("Некоректні зупинки.");

            var seat = await _autoSeatService.FindAvailableSeatAsync(
                dto.ScheduleId, dto.FromStationId, dto.ToStationId);

            if (seat == null)
                throw new InvalidOperationException("Немає доступних місць.");

            var price = PriceService.CalculatePrice(fromStop.StopOrder, toStop.StopOrder);

            var booking = new Booking
            {
                ScheduleId = dto.ScheduleId,
                FromStationId = dto.FromStationId,
                ToStationId = dto.ToStationId,
                UserId = dto.UserId,
                Status = "booked",
                ExpirationTime = DateTime.UtcNow.AddMinutes(20),
                SeatNumber = seat.Value
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return new BookingResponseDto
            {
                BookingId = booking.Id,
                Price = price
            };
        }

        public async Task<BookingDetailsDto?> GetBookingDetailsByIdAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Schedule)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null) return null;

            var fromStation = await _context.Stations
                .Where(s => s.Id == booking.FromStationId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();

            var toStation = await _context.Stations
                .Where(s => s.Id == booking.ToStationId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();

            return new BookingDetailsDto
            {
                Id = booking.Id,
                ClientName = booking.User.Login,
                FromStation = fromStation,
                ToStation = toStation,
                ScheduleDate = booking.Schedule?.Date,
                Status = booking.Status
            };
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    ScheduleId = b.ScheduleId,
                    Status = b.Status,
                    BookingTime = b.ExpirationTime.AddMinutes(-20)
                }).ToListAsync();
        }

        public async Task CancelBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null || booking.Status != "booked")
                return;

            booking.Status = "cancelled";
            await _context.SaveChangesAsync();
        }
    }
}

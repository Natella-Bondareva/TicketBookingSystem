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
                .Include(s => s.ScheduleStops)
                .Include(s => s.Route)
                .ThenInclude(r => r.RouteStops)
                .FirstOrDefaultAsync(s => s.Id == dto.ScheduleId);

            if (schedule == null)
                throw new InvalidOperationException("Розклад не знайдено.");

            // Перевірка наявності зупинок у маршруті
            var fromStop = schedule.ScheduleStops.FirstOrDefault(s => s.StationId == dto.FromStationId);
            var toStop = schedule.ScheduleStops.FirstOrDefault(s => s.StationId == dto.ToStationId);

            if (fromStop == null || toStop == null || fromStop.ArrivalTime >= toStop.DepartureTime)
                throw new InvalidOperationException("Некоректний вибір станцій.");

            // Автоматичний вибір місця
            var availableSeats = await _autoSeatService.FindAvailableSeatAsync(
                dto.ScheduleId, dto.FromStationId, dto.ToStationId);

            if (availableSeats == null)
                throw new InvalidOperationException("Немає доступних місць.");

            // Обчислення ціни за StopOrder (за потреби — через тарифи)
            var routeFrom = schedule.Route.RouteStops.FirstOrDefault(r => r.StationId == dto.FromStationId);
            var routeTo = schedule.Route.RouteStops.FirstOrDefault(r => r.StationId == dto.ToStationId);

            if (routeFrom == null || routeTo == null || routeFrom.StopOrder >= routeTo.StopOrder)
                throw new InvalidOperationException("Станції не відповідають маршруту.");

            var price = PriceService.CalculatePrice(routeFrom.StopOrder, routeTo.StopOrder);

            // Створення бронювання
            var booking = new Booking
            {
                UserId = dto.UserId, // (тимчасово; краще отримувати з Claims)
                ScheduleId = dto.ScheduleId,
                FromStationId = dto.FromStationId,
                ToStationId = dto.ToStationId,
                SeatId = availableSeats.First(),
                BookingStatusId = 2, // "active"
                Price = price,
                CreatedAt = DateTime.UtcNow,
                ExpirationTime = DateTime.UtcNow.AddMinutes(20)
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
                .Include(b => b.Seat)
                .Include(b => b.Schedule)
                .Include(b => b.BookingStatus)
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
                SeatCode = booking.Seat.SeatCode,
                Status = booking.BookingStatus.Name,
                Price = booking.Price,
                CreatedAt = booking.CreatedAt
            };
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.BookingStatus)
                .Include(b => b.Seat)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.ScheduleStops)
                        .ThenInclude(ss => ss.Station)
                .Where(b => b.UserId == userId)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    ScheduleId = b.ScheduleId,
                    Status = b.BookingStatus.Name,
                    Price = b.Price,
                    SeatCode = b.Seat.SeatCode,
                    BookingTime = b.CreatedAt,

                    FromStationName = b.Schedule.ScheduleStops
                        .FirstOrDefault(ss => ss.StationId == b.FromStationId)!.Station.Name,

                    ToStationName = b.Schedule.ScheduleStops
                        .FirstOrDefault(ss => ss.StationId == b.ToStationId)!.Station.Name,

                    DepartureDateTime = b.Schedule.Date.Date +
                        b.Schedule.ScheduleStops.FirstOrDefault(ss => ss.StationId == b.FromStationId)!.DepartureTime,

                    ArrivalDateTime = b.Schedule.Date.Date +
                        b.Schedule.ScheduleStops.FirstOrDefault(ss => ss.StationId == b.ToStationId)!.ArrivalTime
                })
                .ToListAsync();
        }

        public async Task CancelBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingStatus)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null || booking.BookingStatus.Name.ToLower() != "active")
                return;

            // Знаходимо ID статусу "cancelled" (lookup)
            var cancelledStatusId = await _context.BookingStatuses
                .Where(s => s.Name.ToLower() == "cancelled")
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            if (cancelledStatusId == 0)
                throw new Exception("BookingStatus 'cancelled' not found.");

            booking.BookingStatusId = cancelledStatusId;
            await _context.SaveChangesAsync();
        }
    }
}

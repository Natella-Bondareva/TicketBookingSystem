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

        public async Task<int> CreateBookingAsync(CreateBookingDto dto)
        {
            // Перевірка чи місце зайняте
            bool seatTaken = await _context.Bookings
                .AnyAsync(b =>
                    b.ScheduleId == dto.ScheduleId &&
                    b.SeatNumber == int.Parse(dto.SeatNumber) &&
                    b.Status == "booked");

            if (seatTaken)
                throw new InvalidOperationException("Місце вже заброньоване.");

            var booking = new Booking
            {
                ScheduleId = dto.ScheduleId,
                FromStationId = dto.FromStationId,
                ToStationId = dto.ToStationId,
                SeatNumber = int.Parse(dto.SeatNumber),
                Status = "booked",
                ExpirationTime = DateTime.UtcNow.AddMinutes(20), // тимчасова бронь
                UserId = dto.UserId
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return booking.Id;
        }

        public async Task<BookingDetailsDto?> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.FromStationId)
                .Include(b => b.ToStationId)
                .Include(b => b.Schedule)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null) return null;

            return new BookingDetailsDto
            {
                Id = booking.Id,
                ClientName = booking.User.Login,
                FromStationId = booking.FromStationId,
                ToStationId = booking.ToStationId,
                ScheduleDate = booking.Schedule?.Date,
                SeatNumber = booking.SeatNumber,
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
                    SeatNumber = b.SeatNumber.ToString(),
                    Status = b.Status,
                    BookingTime = b.ExpirationTime.AddMinutes(-20)
                }).ToListAsync();
        }


        public async Task<int> CreateBookingAsync(AutoBookingRequestDto dto)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Route)
                .FirstOrDefaultAsync(s => s.Id == dto.ScheduleId);

            if (schedule == null)
                throw new InvalidOperationException("Розклад не знайдено.");

            var seat = await _autoSeatService.FindAvailableSeatAsync(
                dto.ScheduleId, dto.FromStationId, dto.ToStationId);

            if (seat == null)
                throw new InvalidOperationException("Немає доступних місць.");

            var booking = new Booking
            {
                ScheduleId = dto.ScheduleId,
                FromStationId = dto.FromStationId,
                ToStationId = dto.ToStationId,
                UserId = dto.UserId,
                SeatNumber = seat.Value,
                Status = "booked",
                ExpirationTime = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return booking.Id;
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

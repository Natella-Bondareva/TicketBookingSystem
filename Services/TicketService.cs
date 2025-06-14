using Microsoft.EntityFrameworkCore;
using TicketBookingSystem.Data;
using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Models.Entities;

namespace TicketBookingSystem.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _context;
        private readonly IAutoSeatService _autoSeatService;

        public TicketService(AppDbContext context, IAutoSeatService autoSeatService)
        {
            _context = context;
            _autoSeatService = autoSeatService;
        }

        public async Task<int> PurchaseTicketAsync(PurchaseTicketDto dto)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == dto.BookingId && b.Status == "booked");

            if (booking == null)
                throw new InvalidOperationException("Бронювання неактивне або не існує.");

            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == booking.Id);

            if (payment == null)
                throw new InvalidOperationException("Оплата не знайдена. Неможливо завершити покупку.");

            var ticket = new Ticket
            {
                ScheduleId = booking.ScheduleId,
                UserId = booking.UserId,
                FromStationId = booking.FromStationId,
                ToStationId = booking.ToStationId,
                SeatNumber = booking.SeatNumber,
                Price = payment.Amount
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            payment.TicketId = ticket.Id;
            booking.Status = "cancelled";

            await _context.SaveChangesAsync();

            return ticket.Id;
        }

        public async Task<int> CreateTicketFromBookingAsync(PaymentCreateDto dto)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == dto.BookingId && b.Status == "booked");

            if (booking == null)
                throw new InvalidOperationException("Бронювання не знайдено або вже використано.");

            var payment = new Payment
            {
                BookingId = booking.Id,
                Amount = dto.Amount,
                PaymentTime = DateTime.UtcNow
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var ticket = new Ticket
            {
                ScheduleId = booking.ScheduleId,
                UserId = booking.UserId,
                FromStationId = booking.FromStationId,
                ToStationId = booking.ToStationId,
                SeatNumber = booking.SeatNumber,
                Price = dto.Amount
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            payment.TicketId = ticket.Id;
            booking.Status = "cancelled"; // або "used"

            await _context.SaveChangesAsync();

            return ticket.Id;
        }


        public async Task<TicketDto?> GetTicketByIdAsync(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return null;

            return new TicketDto
            {
                Id = ticket.Id,
                BookingId = ticket.Id,
                Price = ticket.Price,
                PurchaseDate = DateTime.UtcNow,
                ScheduleId = ticket.ScheduleId
            };
        }

        public async Task<IEnumerable<TicketDto>> GetTicketsByUserIdAsync(int userId)
        {
            return await _context.Tickets
                .Where(t => t.UserId == userId)
                .Select(t => new TicketDto
                {
                    Id = t.Id,
                    BookingId = t.Id,
                    Price = t.Price,
                    PurchaseDate = DateTime.UtcNow,
                    ScheduleId = t.ScheduleId
                }).ToListAsync();
        }
    }
}

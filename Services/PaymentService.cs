using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Data;
using TicketBookingSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace TicketBookingSystem.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> ProcessPaymentAsync(PaymentRequestDto dto)
        {
            var booking = await _context.Bookings
                .Include(b => b.Schedule)
                .FirstOrDefaultAsync(b => b.Id == dto.BookingId && b.Status == "booked");

            if (booking == null)
                throw new InvalidOperationException("Бронювання не знайдено або вже неактивне.");

            var existingPayment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == booking.Id);

            if (existingPayment != null)
                throw new InvalidOperationException("Оплата для цього бронювання вже здійснена.");

            var price = booking.Price;

            var payment = new Payment
            {
                BookingId = booking.Id,
                PaymentMethod = dto.PaymentMethod,
                Amount = price,
                PaymentTime = DateTime.UtcNow
            };

            _context.Payments.Add(payment);

            var ticket = new Ticket
            {
                ScheduleId = booking.ScheduleId,
                FromStationId = booking.FromStationId,
                ToStationId = booking.ToStationId,
                UserId = booking.UserId,
                Price = price,
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            payment.TicketId = ticket.Id;
            booking.Status = "cancelled"; // бронювання переходить в квиток

            await _context.SaveChangesAsync();

            return payment.Id;
        }
    }
}

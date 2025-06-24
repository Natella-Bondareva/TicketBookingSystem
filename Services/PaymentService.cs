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

        public async Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto dto)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingStatus)
                .FirstOrDefaultAsync(b => b.Id == dto.BookingId);

            if (booking == null)
                throw new InvalidOperationException("Бронювання не знайдено.");

            if (booking.BookingStatus.Name == "completed")
                throw new InvalidOperationException("Це бронювання вже оплачено.");

            var existingPayment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == booking.Id);

            if (existingPayment != null)
                throw new InvalidOperationException("Оплата вже зареєстрована.");

            // Перевірка суми
            if (dto.Amount < booking.Price)
                throw new InvalidOperationException("Недостатня сума оплати.");

            // Створення оплати
            var payment = new Payment
            {
                BookingId = booking.Id,
                UserId = dto.CashierId, // отриманий з UI або токена
                PaymentMethodId = dto.PaymentMethodId,
                Amount = dto.Amount,
                PaymentTime = DateTime.UtcNow
            };
            _context.Payments.Add(payment);

            // Оновлення статусу бронювання
            booking.BookingStatusId = 3; // completed

            // Створення квитка
            var ticket = new Ticket
            {
                BookingId = booking.Id
            };
            _context.Tickets.Add(ticket);

            await _context.SaveChangesAsync();

            return new PaymentResponseDto
            {
                PaymentId = payment.Id,
                TicketId = ticket.Id,
                Amount = payment.Amount,
                PaidAt = payment.PaymentTime
            };
        }
    }
}

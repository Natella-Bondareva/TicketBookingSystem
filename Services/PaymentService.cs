using Microsoft.EntityFrameworkCore;
using TicketBookingSystem.Data;
using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Models.Entities;

namespace TicketBookingSystem.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly ITicketService _ticketService;

        public PaymentService(AppDbContext context, ITicketService ticketService)
        {
            _context = context;
            _ticketService = ticketService;
        }

        public async Task<(int paymentId, int ticketId)> PayAsync(PaymentDto dto)
        {
            // Перевірка наявності броні
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == dto.BookingId && b.Status == "booked");

            if (booking == null)
                throw new InvalidOperationException("Неможливо здійснити оплату. Бронювання не знайдено або неактивне.");

            // Створення запису оплати
            var payment = new Payment
            {
                UserId = dto.UserId,
                BookingId = dto.BookingId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                PaymentTime = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Створення квитка після оплати
            var ticketId = await _ticketService.PurchaseTicketAsync(new PurchaseTicketDto
            {
                BookingId = dto.BookingId.Value,
                Price = dto.Amount
            });

            // Прив’язка квитка до оплати
            payment.TicketId = ticketId;
            await _context.SaveChangesAsync();

            return (payment.Id, ticketId);
        }

        public async Task<PaymentDto?> GetByIdAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return null;

            return new PaymentDto
            {
                UserId = payment.UserId,
                BookingId = payment.BookingId,
                TicketId = payment.TicketId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod
            };
        }
    }
}

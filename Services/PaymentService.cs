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
                throw new InvalidOperationException("���������� �� ��������.");

            if (booking.BookingStatus.Name == "completed")
                throw new InvalidOperationException("�� ���������� ��� ��������.");

            var existingPayment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == booking.Id);

            if (existingPayment != null)
                throw new InvalidOperationException("������ ��� ������������.");

            // �������� ����
            if (dto.Amount < booking.Price)
                throw new InvalidOperationException("���������� ���� ������.");

            // ��������� ������
            var payment = new Payment
            {
                BookingId = booking.Id,
                UserId = dto.CashierId, // ��������� � UI ��� ������
                PaymentMethodId = dto.PaymentMethodId,
                Amount = dto.Amount,
                PaymentTime = DateTime.UtcNow
            };
            _context.Payments.Add(payment);

            // ��������� ������� ����������
            booking.BookingStatusId = 3; // completed

            // ��������� ������
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

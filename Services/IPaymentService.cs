using TicketBookingSystem.Models.DTOs;


namespace TicketBookingSystem.Services
{
    public interface IPaymentService
    {
        Task<(int paymentId, int ticketId)> PayAsync(PaymentDto dto);
        Task<PaymentDto?> GetByIdAsync(int id);
    }
}
using TicketBookingSystem.Models.DTOs;


namespace TicketBookingSystem.Services
{
    public interface IPaymentService
    {
        Task<int> ProcessPaymentAsync(PaymentRequestDto dto);
        //Task<PaymentDto?> GetByIdAsync(int id);
    }
}
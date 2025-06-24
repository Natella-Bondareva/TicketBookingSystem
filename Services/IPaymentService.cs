using TicketBookingSystem.Models.DTOs;


namespace TicketBookingSystem.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto dto);
        //Task<PaymentDto?> GetByIdAsync(int id);
    }
}
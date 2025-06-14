using TicketBookingSystem.Models.DTOs;


namespace TicketBookingSystem.Services
{
    public interface ITicketService
    {
        Task<int> PurchaseTicketAsync(PurchaseTicketDto dto);
        Task<TicketDto?> GetTicketByIdAsync(int ticketId);
        Task<IEnumerable<TicketDto>> GetTicketsByUserIdAsync(int userId);
        Task<int> CreateTicketFromBookingAsync(PaymentCreateDto dto);
    }
}

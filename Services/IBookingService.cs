using TicketBookingSystem.Models.DTOs;


namespace TicketBookingSystem.Services
{
    public interface IBookingService
    {
        Task<int> CreateBookingAsync(CreateBookingDto dto);
        Task<BookingDetailsDto?> GetBookingByIdAsync(int bookingId);
        Task<IEnumerable<BookingDto>> GetBookingsByUserIdAsync(int userId);
        Task CancelBookingAsync(int bookingId);
        Task<int> CreateBookingAsync(AutoBookingRequestDto dto);
    }
}


using TicketBookingSystem.Models.DTOs;

namespace TicketBookingSystem.Services
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateBookingAsync(AutoBookingRequestDto dto);
        Task<BookingDetailsDto?> GetBookingDetailsByIdAsync(int bookingId);
        Task<IEnumerable<BookingDto>> GetBookingsByUserIdAsync(int userId);
        Task CancelBookingAsync(int bookingId);
    }
}

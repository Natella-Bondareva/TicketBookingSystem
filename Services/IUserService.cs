using TicketBookingSystem.Models.DTOs;


namespace TicketBookingSystem.Services
{
    public interface IUserService
    {
        Task<UserProfileDto?> GetUserByIdAsync(int id);
        Task<bool> UpdateUserAsync(int id, UpdateUserDto dto);
    }
}

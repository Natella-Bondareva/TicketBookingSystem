using TicketBookingSystem.Models.DTOs;


namespace TicketBookingSystem.Services
{
    public interface IRouteService
    {
        Task<IEnumerable<AdminRouteDto>> GetAllAsync();
        Task<AdminRouteDto?> GetByIdAsync(int id);
        Task<bool> CreateAsync(CreateRouteDto dto);
        Task<bool> UpdateAsync(int id, UpdateRouteDto dto);
        Task<bool> DeleteAsync(int id);
    }
}

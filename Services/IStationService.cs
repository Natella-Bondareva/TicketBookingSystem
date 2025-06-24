using TicketBookingSystem.Models.DTOs;


namespace TicketBookingSystem.Services
{
    public interface IStationService
    {
        Task<IEnumerable<StationDto>> GetAllAsync();
        Task<StationDto?> GetByIdAsync(int id);
        Task<IEnumerable<StationDto>> SearchByNameAsync(string name);
        Task<int> CreateAsync(CreateUpdateStationDto dto);
        Task<bool> UpdateAsync(int id, CreateUpdateStationDto dto);
        Task<bool> DeleteAsync(int id);
    }
}

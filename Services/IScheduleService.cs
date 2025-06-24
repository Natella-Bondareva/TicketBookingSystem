using TicketBookingSystem.Models.DTOs;


namespace TicketBookingSystem.Services
{
    public interface IScheduleService
    {
        Task<IEnumerable<AdminScheduleDto>> GetAllAsync();
        Task<AdminScheduleDto?> GetByIdAsync(int id);
        Task<IEnumerable<AdminScheduleDto>> GetByRouteIdAsync(int routeId);
        Task<IEnumerable<AdminScheduleStopDto>> GetStopsByScheduleIdAsync(int scheduleId);
        Task<bool> UpdateScheduleStopAsync(UpdateScheduleStopDto dto);


        Task<int> CreateAsync(CreateScheduleDto dto);
        Task<bool> UpdateAsync(int id, UpdateScheduleDto dto);
        Task<bool> DeleteAsync(int id);
    }

}

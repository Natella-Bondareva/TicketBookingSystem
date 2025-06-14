using TicketBookingSystem.Models.DTOs;


namespace TicketBookingSystem.Services
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleStopDto>> GetStopsByScheduleIdAsync(int scheduleId);
    }
}

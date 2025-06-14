using TicketBookingSystem.Models.DTOs;


namespace TicketBookingSystem.Services
{
    public interface IAutoSeatService
    {
        Task<int?> FindAvailableSeatAsync(int scheduleId, int fromStationId, int toStationId);
    }
}
using TicketBookingSystem.Models.DTOs;


namespace TicketBookingSystem.Services
{
    public interface IAutoSeatService
    {
        Task<List<int>> FindAvailableSeatAsync(int scheduleId, int fromStationId, int toStationId);
    }
}
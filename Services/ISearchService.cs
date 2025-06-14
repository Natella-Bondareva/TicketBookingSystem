using TicketBookingSystem.Models.DTOs;


namespace TicketBookingSystem.Services
{
    public interface ISearchService
    {
        Task<IEnumerable<RouteSearchWithSeatsDto>> SearchRoutesWithSeatAvailabilityAsync(SearchRouteDto dto);
        Task<IEnumerable<TransferRouteDto>> SearchRoutesWithTransfersAsync(SearchRouteDto dto);
    }
}


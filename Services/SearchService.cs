using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Data;
using TicketBookingSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace TicketBookingSystem.Services
{
    public class SearchService : ISearchService
    {
        private readonly AppDbContext _context;
        private readonly IAutoSeatService _autoSeatService;

        public SearchService(AppDbContext context, IAutoSeatService autoSeatService)
        {
            _context = context;
            _autoSeatService = autoSeatService;
        }

        public async Task<IEnumerable<RouteSearchWithSeatsDto>> SearchRoutesWithSeatAvailabilityAsync(SearchRouteDto dto)
        {
            var schedules = await _context.Schedules
                .Include(s => s.Route).ThenInclude(r => r.StartStation)
                .Include(s => s.Route).ThenInclude(r => r.EndStation)
                .Include(s => s.ScheduleStops).ThenInclude(ss => ss.Station)
                .ToListAsync();

            var results = new List<RouteSearchWithSeatsDto>();

            foreach (var schedule in schedules)
            {
                var fromStop = schedule.ScheduleStops.FirstOrDefault(s => s.Station.Name.ToLower() == dto.FromStation.ToLower());
                var toStop = schedule.ScheduleStops.FirstOrDefault(s => s.Station.Name.ToLower() == dto.ToStation.ToLower());

                if (fromStop == null || toStop == null || fromStop.StopOrder >= toStop.StopOrder)
                    continue;

                if (dto.Date.HasValue && schedule.Date.Date != dto.Date.Value.Date)
                    continue;

                // ѕерев≥р€Їмо на€вн≥сть в≥льного м≥сц€
                var seat = await _autoSeatService.FindAvailableSeatAsync(schedule.Id, fromStop.StationId, toStop.StationId);

                results.Add(new RouteSearchWithSeatsDto
                {
                    ScheduleId = schedule.Id,
                    RouteName = $"{schedule.Route.StartStation.Name} ? {schedule.Route.EndStation.Name}",
                    FromStation = dto.FromStation,
                    ToStation = dto.ToStation,
                    DepartureTime = schedule.Date.Date + fromStop.DepartureTime,
                    ArrivalTime = schedule.Date.Date + toStop.ArrivalTime,
                    SeatsAvailable = seat.HasValue
                });
            }

            return results;
        }

        public async Task<IEnumerable<TransferRouteDto>> SearchRoutesWithTransfersAsync(SearchRouteDto dto)
        {
            var schedules = await _context.Schedules
                .Include(s => s.ScheduleStops).ThenInclude(ss => ss.Station)
                .Include(s => s.Route).ThenInclude(r => r.StartStation)
                .Include(s => s.Route).ThenInclude(r => r.EndStation)
                .ToListAsync();

            var results = new List<TransferRouteDto>();

            const int MIN_TRANSFER_MINUTES = 10;
            const int MAX_TRANSFER_MINUTES = 120;

            foreach (var s1 in schedules)
            {
                var fromStop1 = s1.ScheduleStops.FirstOrDefault(ss => ss.Station.Name.ToLower() == dto.FromStation.ToLower());
                if (fromStop1 == null) continue;

                foreach (var midStop1 in s1.ScheduleStops)
                {
                    if (midStop1.StopOrder <= fromStop1.StopOrder)
                        continue;

                    string midStationName = midStop1.Station.Name;

                    var seat1 = await _autoSeatService.FindAvailableSeatAsync(s1.Id, fromStop1.StationId, midStop1.StationId);
                    if (!seat1.HasValue) continue;

                    DateTime arrivalAtMid = s1.Date.Date + midStop1.ArrivalTime;

                    foreach (var s2 in schedules)
                    {
                        if (s1.Id == s2.Id) continue;

                        var midStop2 = s2.ScheduleStops.FirstOrDefault(ss => ss.Station.Name.ToLower() == midStationName.ToLower());
                        var toStop2 = s2.ScheduleStops.FirstOrDefault(ss => ss.Station.Name.ToLower() == dto.ToStation.ToLower());

                        if (midStop2 == null || toStop2 == null || midStop2.StopOrder >= toStop2.StopOrder)
                            continue;

                        if (dto.Date.HasValue && s2.Date.Date != dto.Date.Value.Date)
                            continue;

                        DateTime departureFromMid = s2.Date.Date + midStop2.DepartureTime;

                        var waitMinutes = (departureFromMid - arrivalAtMid).TotalMinutes;

                        if (waitMinutes < MIN_TRANSFER_MINUTES || waitMinutes > MAX_TRANSFER_MINUTES)
                            continue;

                        var seat2 = await _autoSeatService.FindAvailableSeatAsync(s2.Id, midStop2.StationId, toStop2.StationId);
                        if (!seat2.HasValue) continue;

                        // ќбчислюЇмо варт≥сть €к: базова + 10 грн за сегмент
                        var price1 = 100 + 10 * (midStop1.StopOrder - fromStop1.StopOrder);
                        var price2 = 100 + 10 * (toStop2.StopOrder - midStop2.StopOrder);

                        results.Add(new TransferRouteDto
                        {
                            FirstScheduleId = s1.Id,
                            FirstFromStation = fromStop1.Station.Name,
                            FirstToStation = midStop1.Station.Name,
                            FirstDeparture = s1.Date.Date + fromStop1.DepartureTime,
                            FirstArrival = arrivalAtMid,

                            SecondScheduleId = s2.Id,
                            SecondFromStation = midStop2.Station.Name,
                            SecondToStation = toStop2.Station.Name,
                            SecondDeparture = departureFromMid,
                            SecondArrival = s2.Date.Date + toStop2.ArrivalTime,

                            TotalPrice = price1 + price2
                        });
                    }
                }
            }

            return results;
        }
    }


}

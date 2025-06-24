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
            string fromName = (dto.FromStation ?? "").Trim().ToLowerInvariant();
            string toName = (dto.ToStation ?? "").Trim().ToLowerInvariant();
            DateTime? searchDate = dto.Date?.Date;

            var fromStationId = await _context.Stations
                .Where(s => s.Name.ToLower() == dto.FromStation.ToLower())
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            var toStationId = await _context.Stations
                .Where(s => s.Name.ToLower() == dto.ToStation.ToLower())
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            if (fromStationId == 0 || toStationId == 0)
                return Enumerable.Empty<RouteSearchWithSeatsDto>();

            var schedules = await _context.Schedules
                .Include(s => s.ScheduleStops)
                    .ThenInclude(ss => ss.Station)
                .Include(s => s.Route)
                    .ThenInclude(r => r.StartStation)
                .Include(s => s.Route)
                    .ThenInclude(r => r.EndStation)
                .Where(s => s.ScheduleStops.Any(ss => ss.StationId == fromStationId) &&
                            s.ScheduleStops.Any(ss => ss.StationId == toStationId))
                .Where(s => !dto.Date.HasValue || s.Date == dto.Date.Value)
                .ToListAsync();

            var results = new List<RouteSearchWithSeatsDto>();

            foreach (var schedule in schedules)
            {
                var stops = schedule.ScheduleStops.ToList();
                var stopOrders = await _context.RouteStops
                    .Where(rs => rs.RouteId == schedule.RouteId &&
                                 (rs.StationId == fromStationId || rs.StationId == toStationId))
                    .ToDictionaryAsync(rs => rs.StationId, rs => rs.StopOrder);

                var fromStop = stops.FirstOrDefault(s => s.StationId == fromStationId);

                var toStop = stops.FirstOrDefault(s => s.StationId == toStationId);

                if (fromStop == null || toStop == null || stopOrders[fromStationId] >= stopOrders[toStationId])
                    continue;

                // знайти вс≥ доступн≥ м≥сц€ (маЇ повертати список, не т≥льки перше)
                var freeSeats = await _autoSeatService.FindAvailableSeatAsync(schedule.Id, fromStop.StationId, toStop.StationId);

                if (freeSeats.Count > 0)
                {
                    if (schedule.Route == null) Console.WriteLine("Route is null");
                    if (schedule.Route?.StartStation == null) Console.WriteLine("StartStation is null");

                    var price = PriceService.CalculatePrice(stopOrders[fromStationId], stopOrders[toStationId]);

                    results.Add(new RouteSearchWithSeatsDto
                    {
                        ScheduleId = schedule.Id,
                        RouteName = $"{schedule.Route.StartStation.Name} Ц {schedule.Route.EndStation.Name}",
                        FromStation = fromStop.Station.Name,
                        ToStation = toStop.Station.Name,
                        FromStationId = fromStop.StationId,
                        ToStationId = toStop.StationId,
                        DepartureTime = schedule.Date.Date + fromStop.DepartureTime,
                        ArrivalTime = schedule.Date.Date + toStop.ArrivalTime,
                        SeatsAvailable = true,
                        FreeSeats = freeSeats,
                        Price = price
                    });
                }
            }

            return results;
        }

        public async Task<IEnumerable<TransferRouteDto>> SearchRoutesWithTransfersAsync(SearchRouteDto dto)
        {
            var fromName = dto.FromStation.ToLower();
            var toName = dto.ToStation.ToLower();

            var schedules = await _context.Schedules
                .Include(s => s.ScheduleStops).ThenInclude(ss => ss.Station)
                .Include(s => s.Route).ThenInclude(r => r.RouteStops)
                .Where(s => !dto.Date.HasValue || s.Date.Date == dto.Date.Value.Date)
                .ToListAsync();

            var results = new List<TransferRouteDto>();
            const int MIN_TRANSFER_MINUTES = 10;
            const int MAX_TRANSFER_MINUTES = 360;

            foreach (var s1 in schedules)
            {
                var routeStops1 = s1.Route.RouteStops;

                var fromStop1 = s1.ScheduleStops
                    .FirstOrDefault(ss => ss.Station.Name.ToLower() == fromName);
                if (fromStop1 == null) continue;

                var fromStopOrder1 = routeStops1
                    .FirstOrDefault(rs => rs.StationId == fromStop1.StationId)?.StopOrder;
                if (fromStopOrder1 == null) continue;

                foreach (var midStop1 in s1.ScheduleStops)
                {
                    var midStopOrder1 = routeStops1
                        .FirstOrDefault(rs => rs.StationId == midStop1.StationId)?.StopOrder;

                    if (midStopOrder1 == null || midStopOrder1 <= fromStopOrder1) continue;

                    var seat1 = await _autoSeatService.FindAvailableSeatAsync(
                        s1.Id, fromStop1.StationId, midStop1.StationId);
                    if (seat1 == null) continue;

                    var arrivalAtMid = s1.Date.Date + midStop1.ArrivalTime;

                    foreach (var s2 in schedules)
                    {
                        if (s1.Id == s2.Id) continue;

                        var routeStops2 = s2.Route.RouteStops;

                        var midStop2 = s2.ScheduleStops
                            .FirstOrDefault(ss => ss.StationId == midStop1.StationId);
                        if (midStop2 == null) continue;

                        var midStopOrder2 = routeStops2
                            .FirstOrDefault(rs => rs.StationId == midStop2.StationId)?.StopOrder;

                        if (midStopOrder2 == null) continue;

                        var toStop2 = s2.ScheduleStops
                            .FirstOrDefault(ss => ss.Station.Name.ToLower() == toName);
                        if (toStop2 == null) continue;

                        var toStopOrder2 = routeStops2
                            .FirstOrDefault(rs => rs.StationId == toStop2.StationId)?.StopOrder;

                        if (toStopOrder2 == null || midStopOrder2 >= toStopOrder2) continue;

                        if (s2.Date.Date != s1.Date.Date) continue;

                        var departureFromMid = s2.Date.Date + midStop2.DepartureTime;
                        var waitMinutes = (departureFromMid - arrivalAtMid).TotalMinutes;

                        if (waitMinutes < MIN_TRANSFER_MINUTES || waitMinutes > MAX_TRANSFER_MINUTES)
                            continue;

                        var seat2 = await _autoSeatService.FindAvailableSeatAsync(
                            s2.Id, midStop2.StationId, toStop2.StationId);
                        if (seat2 == null) continue;

                        var price1 = PriceService.CalculatePrice(fromStopOrder1.Value, midStopOrder1.Value);
                        var price2 = PriceService.CalculatePrice(midStopOrder2.Value, toStopOrder2.Value);

                        results.Add(new TransferRouteDto
                        {
                            FirstScheduleId = s1.Id,
                            FirstFromStation = fromStop1.Station.Name,
                            FirstToStation = midStop1.Station.Name,
                            FirstFromStationId = fromStop1.StationId,
                            FirstToStationId = midStop1.StationId,

                            FirstDeparture = s1.Date.Date + fromStop1.DepartureTime,
                            FirstArrival = arrivalAtMid,

                            SecondScheduleId = s2.Id,
                            SecondFromStation = midStop2.Station.Name,
                            SecondToStation = toStop2.Station.Name,
                            SecondFromStationId = midStop2.StationId,
                            SecondToStationId = toStop2.StationId,
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

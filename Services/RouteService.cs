using Microsoft.EntityFrameworkCore;
using TicketBookingSystem.Data;
using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Models.Entities;

namespace TicketBookingSystem.Services
{
    public class RouteService : IRouteService
    {
        private readonly AppDbContext _context;

        public RouteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdminRouteDto>> GetAllAsync()
        {
            var routes = await _context.Routes
                .Include(r => r.StartStation)
                .Include(r => r.EndStation)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.Station)
                .ToListAsync();

            var result = routes.Select(r => new AdminRouteDto
            {
                Id = r.Id,
                StartStation = r.StartStation.Name,
                EndStation = r.EndStation.Name,
                Stops = r.RouteStops
                    .OrderBy(rs => rs.StopOrder)
                    .Select(rs => new RouteStopDto
                    {
                        StationId = rs.StationId,
                        StationName = rs.Station.Name,
                        StopOrder = rs.StopOrder
                    })
                    .ToList()
            }).ToList();

            return result;
        }


        public async Task<AdminRouteDto?> GetByIdAsync(int id)
        {
            var route = await _context.Routes
                .Include(r => r.StartStation)
                .Include(r => r.EndStation)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.Station)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null) return null;

            return new AdminRouteDto
            {
                Id = route.Id,
                StartStation = route.StartStation.Name,
                EndStation = route.EndStation.Name,
                Stops = route.RouteStops
                    .OrderBy(rs => rs.StopOrder)
                    .Select(rs => new RouteStopDto
                    {
                        StationId = rs.StationId,
                        StationName = rs.Station.Name,
                        StopOrder = rs.StopOrder
                    })
                    .ToList()
            };
        }


        public async Task<bool> CreateAsync(CreateRouteDto dto)
        {
            var route = new TicketBookingSystem.Models.Entities.Route
            {
                StartStationId = dto.StartStationId,
                EndStationId = dto.EndStationId,
                RouteStops = new List<RouteStop>()
            };

            int order = 0;
            var allStationIds = new List<int> { dto.StartStationId };
            allStationIds.AddRange(dto.IntermediateStationIds);
            allStationIds.Add(dto.EndStationId);

            foreach (var stationId in allStationIds)
            {
                route.RouteStops.Add(new RouteStop
                {
                    StationId = stationId,
                    StopOrder = order++
                });
            }

            _context.Routes.Add(route);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int id, UpdateRouteDto dto)
        {
            var route = await _context.Routes
                .Include(r => r.RouteStops)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (route == null) return false;

            route.StartStationId = dto.StartStationId;
            route.EndStationId = dto.EndStationId;

            _context.RouteStops.RemoveRange(route.RouteStops);

            int order = 0;
            var allStationIds = new List<int> { dto.StartStationId };
            allStationIds.AddRange(dto.IntermediateStationIds);
            allStationIds.Add(dto.EndStationId);

            route.RouteStops = allStationIds.Select(sid => new RouteStop
            {
                StationId = sid,
                StopOrder = order++
            }).ToList();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var route = await _context.Routes
                .Include(r => r.RouteStops)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (route == null) return false;

            _context.RouteStops.RemoveRange(route.RouteStops);
            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
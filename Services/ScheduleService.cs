using Microsoft.EntityFrameworkCore;
using TicketBookingSystem.Data;
using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Models.Entities;

namespace TicketBookingSystem.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly AppDbContext _context;

        public ScheduleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ScheduleStopDto>> GetStopsByScheduleIdAsync(int scheduleId)
        {
            var stops = await _context.ScheduleStops
                .Where(s => s.ScheduleId == scheduleId)
                .OrderBy(s => s.StopOrder)
                .Include(s => s.Station)
                .ToListAsync();

            return stops.Select(s => new ScheduleStopDto
            {
                StationName = s.Station.Name,
                ArrivalTime = s.ArrivalTime,
                DepartureTime = s.DepartureTime,
                StopOrder = s.StopOrder
            });
        }
    }
}

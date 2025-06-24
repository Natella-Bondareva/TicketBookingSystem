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

        public async Task<IEnumerable<AdminScheduleDto>> GetAllAsync()
        {
            return await _context.Schedules
                .Select(s => new AdminScheduleDto
                {
                    Id = s.Id,
                    RouteId = s.RouteId,
                    Date = s.Date
                }).ToListAsync();
        }

        public async Task<AdminScheduleDto?> GetByIdAsync(int id)
        {
            return await _context.Schedules
                .Where(s => s.Id == id)
                .Select(s => new AdminScheduleDto
                {
                    Id = s.Id,
                    RouteId = s.RouteId,
                    Date = s.Date
                }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AdminScheduleDto>> GetByRouteIdAsync(int routeId)
        {
            return await _context.Schedules
                .Where(s => s.RouteId == routeId)
                .Select(s => new AdminScheduleDto
                {
                    Id = s.Id,
                    RouteId = s.RouteId,
                    Date = s.Date
                }).ToListAsync();
        }

        public async Task<IEnumerable<AdminScheduleStopDto>> GetStopsByScheduleIdAsync(int scheduleId)
        {
            return await _context.ScheduleStops
                .Where(ss => ss.ScheduleId == scheduleId)
                .OrderBy(ss => ss.StationId) // або StopOrder, якщо він є
                .Select(ss => new AdminScheduleStopDto
                {
                    StationId = ss.StationId,
                    StationName = ss.Station.Name,
                    ArrivalTime = ss.ArrivalTime,
                    DepartureTime = ss.DepartureTime
                }).ToListAsync();
        }

        public async Task<int> CreateAsync(CreateScheduleDto dto)
        {
            var routeStops = await _context.RouteStops
                .Where(rs => rs.RouteId == dto.RouteId)
                .OrderBy(rs => rs.StopOrder)
                .ToListAsync();

            if (dto.Stops.Count != routeStops.Count ||
                !dto.Stops.Select(s => s.StationId).SequenceEqual(routeStops.Select(r => r.StationId)))
            {
                throw new InvalidOperationException("Список зупинок не відповідає маршруту.");
            }

            var schedule = new Schedule
            {
                RouteId = dto.RouteId,
                Date = dto.Date
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync(); // генерує Id

            var stops = dto.Stops.Select(s => new ScheduleStop
            {
                ScheduleId = schedule.Id,
                StationId = s.StationId,
                ArrivalTime = s.ArrivalTime,
                DepartureTime = s.DepartureTime
            });

            _context.ScheduleStops.AddRange(stops);
            await _context.SaveChangesAsync();

            return schedule.Id;
        }



        public async Task<bool> UpdateAsync(int id, UpdateScheduleDto dto)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return false;

            schedule.Date = dto.Date;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return false;

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateScheduleStopAsync(UpdateScheduleStopDto dto)
        {
            var stop = await _context.ScheduleStops
                .FirstOrDefaultAsync(s => s.ScheduleId == dto.ScheduleId && s.StationId == dto.StationId);

            if (stop == null) return false;

            stop.ArrivalTime = dto.ArrivalTime;
            stop.DepartureTime = dto.DepartureTime;

            await _context.SaveChangesAsync();
            return true;
        }

    }
}

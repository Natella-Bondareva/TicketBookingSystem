using Microsoft.EntityFrameworkCore;
using TicketBookingSystem.Data;
using TicketBookingSystem.Models.Entities;

namespace TicketBookingSystem.Services
{
    public class AutoSeatService : IAutoSeatService
    {
        private readonly AppDbContext _context;

        public AutoSeatService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int?> FindAvailableSeatAsync(int scheduleId, int fromStationId, int toStationId)
        {
            var stops = await _context.ScheduleStops
                .Where(s => s.ScheduleId == scheduleId)
                .OrderBy(s => s.StopOrder)
                .ToListAsync();

            var fromStop = stops.FirstOrDefault(s => s.StationId == fromStationId);
            var toStop = stops.FirstOrDefault(s => s.StationId == toStationId);

            if (fromStop == null || toStop == null || fromStop.StopOrder >= toStop.StopOrder)
                return null;

            int fromOrder = fromStop.StopOrder;
            int toOrder = toStop.StopOrder;

            const int MAX_SEATS = 5;

            var seatMap = new Dictionary<int, List<(int from, int to)>>();

            var tickets = await _context.Tickets
                .Where(t => t.ScheduleId == scheduleId)
                .Select(t => new { t.SeatNumber, t.FromStationId, t.ToStationId })
                .ToListAsync();

            var bookings = await _context.Bookings
                .Where(b => b.ScheduleId == scheduleId && b.Status == "booked")
                .Select(b => new { b.SeatNumber, b.FromStationId, b.ToStationId })
                .ToListAsync();

            foreach (var t in tickets)
            {
                var tFrom = stops.FirstOrDefault(s => s.StationId == t.FromStationId)?.StopOrder ?? -1;
                var tTo = stops.FirstOrDefault(s => s.StationId == t.ToStationId)?.StopOrder ?? -1;
                if (tFrom != -1 && tTo != -1)
                {
                    if (!seatMap.ContainsKey(t.SeatNumber))
                        seatMap[t.SeatNumber] = new List<(int, int)>();
                    seatMap[t.SeatNumber].Add((tFrom, tTo));
                }
            }

            foreach (var b in bookings)
            {
                var bFrom = stops.FirstOrDefault(s => s.StationId == b.FromStationId)?.StopOrder ?? -1;
                var bTo = stops.FirstOrDefault(s => s.StationId == b.ToStationId)?.StopOrder ?? -1;
                if (bFrom != -1 && bTo != -1)
                {
                    if (!seatMap.ContainsKey(b.SeatNumber))
                        seatMap[b.SeatNumber] = new List<(int, int)>();
                    seatMap[b.SeatNumber].Add((bFrom, bTo));
                }
            }

            for (int seat = 1; seat <= MAX_SEATS; seat++)
            {
                var occupied = seatMap.ContainsKey(seat) ? seatMap[seat] : new List<(int, int)>();

                bool isFree = true;
                foreach (var segment in occupied)
                {
                    if (!(toOrder <= segment.Item1 || fromOrder >= segment.Item2))
                    {
                        isFree = false;
                        break;
                    }
                }

                if (isFree)
                    return seat;
            }

            return null;
        }
    }
}

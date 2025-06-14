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
                .Include(s => s.Station)
                .ToListAsync();

            var fromStop = stops.FirstOrDefault(s => s.StationId == fromStationId);
            var toStop = stops.FirstOrDefault(s => s.StationId == toStationId);

            if (fromStop == null || toStop == null || fromStop.StopOrder >= toStop.StopOrder)
                return null;

            int fromOrder = fromStop.StopOrder;
            int toOrder = toStop.StopOrder;

            const int MAX_SEATS = 5;

            // Створюємо мапу зайнятості
            var seatMap = new Dictionary<int, List<(int from, int to)>>();

            var tickets = await _context.Tickets.Where(t => t.ScheduleId == scheduleId).ToListAsync();
            var bookings = await _context.Bookings.Where(b => b.ScheduleId == scheduleId && b.Status == "booked").ToListAsync();

            void AddOccupied(int seat, int sFrom, int sTo)
            {
                if (!seatMap.ContainsKey(seat))
                    seatMap[seat] = new List<(int from, int to)>();
                seatMap[seat].Add((sFrom, sTo));
            }

            int GetOrder(int stationId) =>
                stops.FirstOrDefault(s => s.StationId == stationId)?.StopOrder ?? -1;

            foreach (var t in tickets)
                AddOccupied(t.SeatNumber, GetOrder(t.FromStationId), GetOrder(t.ToStationId));

            foreach (var b in bookings)
                AddOccupied(b.SeatNumber, GetOrder(b.FromStationId), GetOrder(b.ToStationId));

            int? bestSeat = null;
            int bestFragmentScore = int.MaxValue;

            for (int seat = 1; seat <= MAX_SEATS; seat++)
            {
                var occupied = seatMap.ContainsKey(seat)
                    ? seatMap[seat].OrderBy(s => s.from).ToList()
                    : new List<(int from, int to)>();

                // Перевіряємо, чи є конфлікт
                bool conflict = occupied.Any(seg =>
                    !(toOrder <= seg.from || fromOrder >= seg.to));

                if (conflict)
                    continue;

                // Вставляємо тимчасово сегмент і рахуємо фрагментацію
                occupied.Add((fromOrder, toOrder));
                occupied = occupied.OrderBy(s => s.from).ToList();

                int fragments = 0;
                int lastEnd = 0;

                foreach (var seg in occupied)
                {
                    if (lastEnd < seg.from)
                        fragments++;
                    lastEnd = Math.Max(lastEnd, seg.to);
                }

                if (lastEnd < stops.Max(s => s.StopOrder))
                    fragments++;

                if (fragments < bestFragmentScore)
                {
                    bestFragmentScore = fragments;
                    bestSeat = seat;
                }
            }

            return bestSeat;
        }
    }
}
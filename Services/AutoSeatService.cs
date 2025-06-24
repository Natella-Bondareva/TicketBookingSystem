using Microsoft.EntityFrameworkCore;
using TicketBookingSystem.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TicketBookingSystem.Services
{
    public class AutoSeatService : IAutoSeatService
    {
        private readonly AppDbContext _context;

        public AutoSeatService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<int>> FindAvailableSeatAsync(int scheduleId, int fromStationId, int toStationId)
        {
            // 1. Отримуємо всі місця на цьому рейсі
            var seats = await _context.Seats
                .Where(s => s.ScheduleId == scheduleId)
                .ToListAsync();

            // 1. Отримуємо Schedule, включаючи його маршрут
            var schedule = await _context.Schedules
                .Include(s => s.Route)
                .FirstOrDefaultAsync(s => s.Id == scheduleId);

            if (schedule == null)
                return new List<int>();

            var routeId = schedule.RouteId;

            // 2. Отримуємо RouteStops, щоб дізнатися StopOrder
            var routeStops = await _context.RouteStops
                .Where(rs => rs.RouteId == routeId)
                .ToDictionaryAsync(rs => rs.StationId, rs => rs.StopOrder);

            // 3. Побудова мапи StationId ? StopOrder
            // (routeStops вже є цією мапою)

            // 4. Перевірка чи станції є у маршруті
            if (!routeStops.TryGetValue(fromStationId, out var fromOrder) ||
                !routeStops.TryGetValue(toStationId, out var toOrder) ||
                fromOrder >= toOrder)
            {
                return new List<int>(); // Невалідний сегмент
            }

            // 5. Отримуємо всі активні бронювання
            var bookings = await _context.Bookings
                .Where(b => b.ScheduleId == scheduleId &&
                            (b.BookingStatus.Name == "active" || b.BookingStatus.Name == "completed"))
                .Select(b => new { b.SeatId, b.FromStationId, b.ToStationId })
                .ToListAsync();

            // 6. Побудова карти зайнятості кожного SeatId
            var seatMap = new Dictionary<int, List<(int from, int to)>>();

            void RegisterOccupied(Dictionary<int, List<(int, int)>> map, IEnumerable<dynamic> records)
            {
                foreach (var r in records)
                {
                    if (!routeStops.TryGetValue(r.FromStationId, out int sFrom))
                        continue;

                    if (!routeStops.TryGetValue(r.ToStationId, out int sTo))
                        continue;

                    if (!map.ContainsKey(r.SeatId))
                        map[r.SeatId] = new List<(int, int)>();

                    map[r.SeatId].Add((sFrom, sTo));
                }
            }

            RegisterOccupied(seatMap, bookings);

            // 7. Перевірка доступності місць
            var freeSeats = new List<int>();

            foreach (var seat in seats)
            {
                var occupiedSegments = seatMap.ContainsKey(seat.Id) ? seatMap[seat.Id] : new List<(int, int)>();

                bool isFree = true;

                foreach (var segment in occupiedSegments)
                {
                    // Якщо є перетин з сегментом користувача ? місце зайняте
                    if (!(toOrder <= segment.Item1 || fromOrder >= segment.Item2))
                    {
                        isFree = false;
                        break;
                    }
                }

                if (isFree)
                    freeSeats.Add(seat.Id); // Додаємо вільне місце
            }

            return freeSeats;
        }
    }
}
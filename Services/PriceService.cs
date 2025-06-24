using Microsoft.EntityFrameworkCore;
using TicketBookingSystem.Data;
using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Models.Entities;

namespace TicketBookingSystem.Services
{
    public class PriceService
    {

            /// <summary>
            /// Обчислює вартість квитка на основі порядку зупинок у маршруті.
            /// </summary>
            /// <param name="fromOrder">Порядковий номер станції посадки (StopOrder).</param>
            /// <param name="toOrder">Порядковий номер станції висадки (StopOrder).</param>
            /// <returns>Вартість поїздки у гривнях.</returns>
            public static decimal CalculatePrice(int fromOrder, int toOrder)
            {
                if (fromOrder < 0 || toOrder < 0)
                    throw new ArgumentException("Номери зупинок не можуть бути від'ємними.");

                int segmentCount = toOrder - fromOrder;

                if (segmentCount <= 0)
                    throw new ArgumentException("Кінцева зупинка має бути пізніше початкової.");

                const decimal basePrice = 100m;
                const decimal pricePerSegment = 25m;

                return basePrice + pricePerSegment * segmentCount;
            }
        
    }
}

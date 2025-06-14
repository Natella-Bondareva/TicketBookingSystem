using Microsoft.EntityFrameworkCore;
using TicketBookingSystem.Data;
using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Models.Entities;

namespace TicketBookingSystem.Services
{
    public class PriceService
    {
        public static decimal CalculatePrice(int fromOrder, int toOrder)
        {
            int segmentLength = toOrder - fromOrder;
            if (segmentLength <= 0)
                throw new ArgumentException("Некоректні зупинки.");

            return 100 + 25 * segmentLength; // прикладна логіка
        }
    }
}

using Microsoft.EntityFrameworkCore;
using TicketBookingSystem.Data;
using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Models.Entities;

namespace TicketBookingSystem.Services
{
    public class PriceService
    {

            /// <summary>
            /// �������� ������� ������ �� ����� ������� ������� � �������.
            /// </summary>
            /// <param name="fromOrder">���������� ����� ������� ������� (StopOrder).</param>
            /// <param name="toOrder">���������� ����� ������� ������� (StopOrder).</param>
            /// <returns>������� ������ � �������.</returns>
            public static decimal CalculatePrice(int fromOrder, int toOrder)
            {
                if (fromOrder < 0 || toOrder < 0)
                    throw new ArgumentException("������ ������� �� ������ ���� ��'������.");

                int segmentCount = toOrder - fromOrder;

                if (segmentCount <= 0)
                    throw new ArgumentException("ʳ����� ������� �� ���� ����� ���������.");

                const decimal basePrice = 100m;
                const decimal pricePerSegment = 25m;

                return basePrice + pricePerSegment * segmentCount;
            }
        
    }
}

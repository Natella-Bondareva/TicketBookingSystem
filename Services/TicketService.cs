//using TicketBookingSystem.Models.DTOs;
//using TicketBookingSystem.Data;
//using TicketBookingSystem.Models.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;

//namespace TicketBookingSystem.Services
//{
//    public class TicketService : ITicketService
//    {
//        private readonly AppDbContext _context;

//        public TicketService(AppDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<TicketDto?> GetTicketByIdAsync(int ticketId)
//        {
//            var ticket = await _context.Tickets
//                .Include(t => t.Schedule)
//                .Include(t => t.Payment)
//                .FirstOrDefaultAsync(t => t.Id == ticketId);

//            if (ticket == null) return null;

//            return new TicketDto
//            {
//                Id = ticket.Id,
//                BookingId = ticket.BookingId,
//                ScheduleId = ticket.ScheduleId,
//                FromStationId = ticket.FromStationId,
//                ToStationId = ticket.ToStationId,
//                Price = ticket.Price,
//                PurchaseDate = ticket.Payment?.PaymentTime ?? DateTime.MinValue
//            };
//        }

//        public async Task<IEnumerable<TicketDto>> GetTicketsByUserIdAsync(int userId)
//        {
//            return await _context.Tickets
//                .Include(t => t.Payment)
//                .Where(t => t.UserId == userId)
//                .Select(t => new TicketDto
//                {
//                    Id = t.Id,
//                    BookingId = t.BookingId,
//                    ScheduleId = t.ScheduleId,
//                    FromStationId = t.FromStationId,
//                    ToStationId = t.ToStationId,
//                    Price = t.Price,
//                    PurchaseDate = t.Payment!.PaymentTime
//                }).ToListAsync();
//        }
//    }
//}

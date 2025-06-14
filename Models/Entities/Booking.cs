using System;
using System.Collections.Generic;
using TicketBookingSystem.Models.Entities;


namespace TicketBookingSystem.Models.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ScheduleId { get; set; }
        public int FromStationId { get; set; }
        public int ToStationId { get; set; }
        public int SeatNumber { get; set; }
        public string Status { get; set; } // active, cancelled
        public DateTime ExpirationTime { get; set; }
        public decimal Price { get; set; }
        public User User { get; set; }
        public Schedule Schedule { get; set; }
        public Ticket Ticket { get; set; }
    }
}
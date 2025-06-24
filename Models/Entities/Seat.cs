using System;
using System.Collections.Generic;
using TicketBookingSystem.Models.Entities;

namespace TicketBookingSystem.Models.Entities
{
    public class Seat
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public string SeatCode { get; set; }
        public string Type { get; set; } // economy, premium, etc.

        public Schedule Schedule { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}

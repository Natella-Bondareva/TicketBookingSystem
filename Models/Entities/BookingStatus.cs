using System;
using System.Collections.Generic;
using TicketBookingSystem.Models.Entities;

namespace TicketBookingSystem.Models.Entities
{
    public class BookingStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; // e.g. "active", "cancelled"
        public ICollection<Booking> Bookings { get; set; }
    }
}
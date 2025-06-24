using System;
using System.Collections.Generic;
using TicketBookingSystem.Models.Entities;

namespace TicketBookingSystem.Models.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public int BookingId { get; set; }

        public Booking Booking { get; set; }
    }
}
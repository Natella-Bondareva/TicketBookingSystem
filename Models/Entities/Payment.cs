using System;
using System.Collections.Generic;
using TicketBookingSystem.Models.Entities;

namespace TicketBookingSystem.Models.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? BookingId { get; set; }
        public int? TicketId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentTime { get; set; }
        public string PaymentMethod { get; set; } // cash, card, online, other
        public User User { get; set; }
        public Ticket Ticket { get; set; }

    }
}
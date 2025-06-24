using System;
using System.Collections.Generic;
using TicketBookingSystem.Models.Entities;

namespace TicketBookingSystem.Models.Entities
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; // e.g. "card", "cash", "online"
        public ICollection<Payment> Payments { get; set; }
    }
}
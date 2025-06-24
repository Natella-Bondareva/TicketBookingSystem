using System;
using System.Collections.Generic;
using TicketBookingSystem.Models.Entities;

namespace TicketBookingSystem.Models.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SeatId { get; set; }
        public int FromStationId { get; set; }
        public int ToStationId { get; set; }

        public int BookingStatusId { get; set; } 
        public BookingStatus BookingStatus { get; set; }

        public DateTime ExpirationTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Price { get; set; }

        public int ScheduleId { get; set; } // для зручності фільтрації
        public Schedule Schedule { get; set; }

        public User User { get; set; }
        public Seat Seat { get; set; }
        public Ticket Ticket { get; set; }
        public Payment Payment { get; set; }
    }
}
using System;
using System.Collections.Generic;
using TicketBookingSystem.Models.Entities;


namespace TicketBookingSystem.Models.Entities
{
    public class Station
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Coordinates { get; set; } = null!;
        public ICollection<RouteStop> RouteStops { get; set; }
        public ICollection<ScheduleStop> ScheduleStops { get; set; }
    }
}
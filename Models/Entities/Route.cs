using System;
using System.Collections.Generic;
using TicketBookingSystem.Models.Entities;

namespace TicketBookingSystem.Models.Entities
{
    public class Route
    {
        public int Id { get; set; }
        public int StartStationId { get; set; }
        public int EndStationId { get; set; }
        public Station StartStation { get; set; }
        public Station EndStation { get; set; }
        public ICollection<RouteStop> RouteStops { get; set; }
        public ICollection<Schedule> Schedules { get; set; }
    }
}
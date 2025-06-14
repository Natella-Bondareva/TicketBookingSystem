using System;
using System.Collections.Generic;
using TicketBookingSystem.Models.Entities;

namespace TicketBookingSystem.Models.Entities
{
    public class RouteStop
    {
        public int RouteId { get; set; }
        public int StationId { get; set; }
        public int StopOrder { get; set; }
        public Route Route { get; set; }
        public Station Station { get; set; }
    }
}
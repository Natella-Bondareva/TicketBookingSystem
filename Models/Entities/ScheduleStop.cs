using System;
using System.Collections.Generic;
using TicketBookingSystem.Models.Entities;


namespace TicketBookingSystem.Models.Entities
{
  public class ScheduleStop
    {
        public int ScheduleId { get; set; }
        public int StationId { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan DepartureTime { get; set; }

        public Schedule Schedule { get; set; }
        public Station Station { get; set; }
    }
}
using System;
using System.Collections.Generic;
using TicketBookingSystem.Models.Entities;


namespace TicketBookingSystem.Models.Entities
{
	public class Schedule
	{
		public int Id { get; set; }
		public int RouteId { get; set; }
		public DateTime Date { get; set; }
		public Route Route { get; set; }
		public ICollection<ScheduleStop> ScheduleStops { get; set; }
		public ICollection<Booking> Bookings { get; set; }
	}
}
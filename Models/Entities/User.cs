using System;
using System.Collections.Generic;
using TicketBookingSystem.Models.Entities;


namespace TicketBookingSystem.Models.Entities
{
	public class User
	{
		public int Id { get; set; }
		public string Login { get; set; } = null!;
		public string PasswordHash { get; set; } = null!;

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;

		public ICollection<Ticket> Tickets { get; set; }
		public ICollection<Booking> Bookings { get; set; }
		public ICollection<Payment> Payments { get; set; }
	}
}
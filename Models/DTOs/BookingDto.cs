namespace TicketBookingSystem.Models.DTOs
{
	/// <summary>
	/// DTO для створення броні
	/// </summary>
	public class CreateBookingDto
	{
		public int UserId { get; set; }
		public int ScheduleId { get; set; }
		public int FromStationId { get; set; }
		public int ToStationId { get; set; }
		public string SeatNumber { get; set; }
	}


	/// <summary>
	/// DTO для перегляду броні
	/// </summary>
	public class BookingDto
	{
		public int Id { get; set; }
		public int ScheduleId { get; set; }
		public string SeatNumber { get; set; }
		public string Status { get; set; } // booked, cancelled
		public DateTime BookingTime { get; set; }
	}

    public class AutoBookingRequestDto
    {
        public int ScheduleId { get; set; }
        public int FromStationId { get; set; }
        public int ToStationId { get; set; }
        public int UserId { get; set; }
    }

    public class BookingDetailsDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int FromStationId { get; set; }
        public int ToStationId { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public int SeatNumber { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
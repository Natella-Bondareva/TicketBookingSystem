namespace TicketBookingSystem.Models.DTOs
{
    /// <summary>
    /// DTO для автоматичного створення бронювання
    /// </summary>
    public class AutoBookingRequestDto
    {
        public int ScheduleId { get; set; }
        public int FromStationId { get; set; }
        public int ToStationId { get; set; }
        public int UserId { get; set; }
    }

    /// <summary>
    /// DTO, що повертається після створення бронювання
    /// </summary>
    public class BookingResponseDto
    {
        public int BookingId { get; set; }
        public decimal Price { get; set; }
        public string SeatCode { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO для розширеного перегляду бронювання (у списку)
    /// </summary>
    public class BookingDto
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime BookingTime { get; set; }
        public string SeatCode { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public string FromStationName { get; set; } = string.Empty;
        public string ToStationName { get; set; } = string.Empty;
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
    }


    /// <summary>
    /// DTO для детального перегляду бронювання (касирами або адміністратором)
    /// </summary>
    public class BookingDetailsDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string FromStation { get; set; } = string.Empty;
        public string ToStation { get; set; } = string.Empty;
        public DateTime? ScheduleDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string SeatCode { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}

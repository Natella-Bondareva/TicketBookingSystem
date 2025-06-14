namespace TicketBookingSystem.Models.DTOs
{
    /// <summary>
    /// Розклад для певного маршруту
    /// </summary>
    public class ScheduleDto
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public string RouteName { get; set; }
        public IEnumerable<ScheduleStopDto> Stops { get; set; }
        public decimal BasePrice { get; set; }
        public IEnumerable<string> AvailableSeats { get; set; }
    }

    public class ScheduleStopDto
    {
        public string StationName { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public int StopOrder { get; set; }
    }

}
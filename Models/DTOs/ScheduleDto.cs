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

    public class AdminScheduleDto
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public DateTime Date { get; set; }
    }

    public class CreateScheduleDto
    {
        public int RouteId { get; set; }
        public DateTime Date { get; set; }
        public List<ScheduleStopDto> Stops { get; set; } = new();
    } 

    public class UpdateScheduleDto
    {
        public DateTime Date { get; set; }
    }



    public class ScheduleStopDto
    {
        public string StationName { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public int StopOrder { get; set; }
    }

    public class AdminScheduleStopDto
    {
        public int StationId { get; set; }
        public string StationName { get; set; } = string.Empty;
        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan DepartureTime { get; set; }
    }

    public class UpdateScheduleStopDto
    {
        public int ScheduleId { get; set; }
        public int StationId { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan DepartureTime { get; set; }
    }

}
namespace TicketBookingSystem.Models.DTOs
{
    /// <summary>
    /// Відображення маршруту для адміністратора
    /// </summary>
    public class AdminRouteDto
    {
        public int Id { get; set; }
        public string StartStation { get; set; } = string.Empty;
        public string EndStation { get; set; } = string.Empty;
        public List<RouteStopDto> Stops { get; set; } = new();
    }


    /// <summary>
    /// Відображення маршруту для користувача
    /// </summary>
    public class RouteDto
    {
        public int Id { get; set; }
        public string FromStation { get; set; }
        public string ToStation { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// DTO для запиту пошуку маршрутів
    /// </summary>
    public class SearchRouteDto
    {
        public string FromStation { get; set; }
        public string ToStation { get; set; }
        public DateTime? Date { get; set; }
        public bool AllowTransfers { get; set; } = false;
    }

    public class RouteStopDto
    {
        public int StationId { get; set; }
        public string StationName { get; set; } = string.Empty;
        public int StopOrder { get; set; }
    } 

    public class RouteSearchResultDto
    {
        public int ScheduleId { get; set; }
        public string RouteName { get; set; }
        public string FromStation { get; set; }
        public string ToStation { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal EstimatedPrice { get; set; }
    }

    public class RouteSearchWithSeatsDto
    {
        public int ScheduleId { get; set; }
        public string RouteName { get; set; }
        public string FromStation { get; set; }
        public string ToStation { get; set; }
        public int FromStationId { get; set; }
        public int ToStationId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public bool SeatsAvailable { get; set; }
        public List<int> FreeSeats { get; set; }
        public decimal Price { get; set; }
    }

    public class CreateRouteDto
    {
        public int StartStationId { get; set; }
        public int EndStationId { get; set; }
        public List<int> IntermediateStationIds { get; set; }
    }

    public class UpdateRouteDto
    {
        public int StartStationId { get; set; }
        public int EndStationId { get; set; }
        public List<int> IntermediateStationIds { get; set; } = new();
    }

    public class TransferRouteDto
    {
        public int FirstScheduleId { get; set; }
        public string FirstFromStation { get; set; }
        public string FirstToStation { get; set; }
        public int FirstFromStationId { get; set; }
        public int FirstToStationId { get; set; }
        public DateTime FirstDeparture { get; set; }
        public DateTime FirstArrival { get; set; }

        public int SecondScheduleId { get; set; }
        public string SecondFromStation { get; set; }
        public string SecondToStation { get; set; }
        public int SecondFromStationId { get; set; }
        public int SecondToStationId { get; set; }
        public DateTime SecondDeparture { get; set; }
        public DateTime SecondArrival { get; set; }

        public decimal TotalPrice { get; set; }
    }

}
namespace TicketBookingSystem.Models.DTOs
{
    /// <summary>
    /// DTO для покупки квитка
    /// </summary>
    public class PurchaseTicketDto
    {
        public int BookingId { get; set; }
        public decimal Price { get; set; }
    }

    /// <summary>
    /// DTO для відображення квитка
    /// </summary>
    public class TicketDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int ScheduleId { get; set; }
    }


}
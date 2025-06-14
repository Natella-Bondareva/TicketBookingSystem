namespace TicketBookingSystem.Models.DTOs
{
    public class TicketDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int ScheduleId { get; set; }
        public int FromStationId { get; set; }
        public int ToStationId { get; set; }

    }

}
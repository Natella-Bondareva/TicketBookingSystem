namespace TicketBookingSystem.Models.DTOs
{
    public class PaymentDto
    {
        public int UserId { get; set; }
        public int? BookingId { get; set; }
        public int? TicketId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class PaymentCreateDto
    {
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
    }
}
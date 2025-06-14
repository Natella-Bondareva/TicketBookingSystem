namespace TicketBookingSystem.Models.DTOs
{
    /// <summary>
    /// DTO ��� ��������� ���������� ��� ������
    /// </summary>
    public class PaymentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? BookingId { get; set; }
        public int? TicketId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime PaidAt { get; set; }
    }

    /// <summary>
    /// DTO ��� ��������� ������ �������
    /// </summary>
    public class PaymentCreateDto
    {
        public int BookingId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO ������ �� ��������� ������ (������� ������������ � �����)
    /// </summary>
    public class PaymentRequestDto
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO ������ ���� ��������� ������
    /// </summary>
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }
        public int TicketId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
    }
}

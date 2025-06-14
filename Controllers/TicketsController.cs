using Microsoft.AspNetCore.Mvc;
using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;



namespace TicketBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // �������� ������ �� ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }

        //������ ������� �����������
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var tickets = await _ticketService.GetTicketsByUserIdAsync(userId);
            return Ok(tickets);
        }

        [HttpPost("cashier/create-from-booking")]
        [Authorize(Roles = "cashier")]
        public async Task<IActionResult> CreateFromBooking([FromBody] PaymentCreateDto dto)
        {
            var ticketId = await _ticketService.CreateTicketFromBookingAsync(dto);
            return Ok(new { ticketId });
        }



        // ������� ������� ������� ����� ��� ��������� � �� �������,
        // �� ����� ������� ���������� ����� PaymentsController ����������� ���� ������.
    }
}
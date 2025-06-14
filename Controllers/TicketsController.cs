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

        /// <summary>
        /// �������� ������ �� ���� ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "client,cashier,admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
                return NotFound(new { message = "������ �� ��������" });
            return Ok(ticket);
        }

        /// <summary>
        /// �������� �� ������ �����������
        /// </summary>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "client,cashier,admin")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var tickets = await _ticketService.GetTicketsByUserIdAsync(userId);
            return Ok(tickets);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using TicketBookingSystem.Models.DTOs;

namespace TicketBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StationsController : ControllerBase
    {
        // ������ ��� �������
        [HttpGet]
        public IActionResult GetAll()
        {
            // �������� �� ������� ����� ���
            return Ok(new[] { new { id = 1, name = "Central Station" } });
        }

        // ���������� ��� ���� �������
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            // ����� ������� ����� ���
            return Ok(new { id, name = "Central Station" });
        }
    }
}
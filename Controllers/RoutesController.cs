using Microsoft.AspNetCore.Mvc;
using TicketBookingSystem.Models.DTOs;
using Microsoft.AspNetCore.Authorization;


namespace TicketBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController : ControllerBase
    {
        // ������ ��� ��������
        [HttpGet]
        public IActionResult GetAll()
        {
            // �������� �� �������� ����� ���
            return Ok(new[] { new { id = 1, from = "A", to = "B" } });
        }

        // ���������� ��� ���� �������
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            // ����� �������� ����� ���
            return Ok(new { id, from = "A", to = "B" });
        }

        // ������ ������� (����)
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult Create([FromBody] CreateRouteDto dto)
        {
            // ��������� �������� ����� ���
            return Ok(new { message = "Route created" });
        }

        // ������� ������� (����)
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateRouteDto dto)
        {
            // ��������� �������� ����� ���
            return Ok(new { message = "Route updated" });
        }
    }
}
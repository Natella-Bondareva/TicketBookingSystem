using Microsoft.AspNetCore.Mvc;
using TicketBookingSystem.Models.DTOs;

namespace TicketBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StationsController : ControllerBase
    {
        // Список всіх станцій
        [HttpGet]
        public IActionResult GetAll()
        {
            // Отримати всі станції логіка тут
            return Ok(new[] { new { id = 1, name = "Central Station" } });
        }

        // Інформація про одну станцію
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            // Пошук станції логіка тут
            return Ok(new { id, name = "Central Station" });
        }
    }
}
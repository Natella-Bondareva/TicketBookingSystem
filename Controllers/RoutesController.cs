using Microsoft.AspNetCore.Mvc;
using TicketBookingSystem.Models.DTOs;
using Microsoft.AspNetCore.Authorization;


namespace TicketBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController : ControllerBase
    {
        // Список всіх маршрутів
        [HttpGet]
        public IActionResult GetAll()
        {
            // Отримати всі маршрути логіка тут
            return Ok(new[] { new { id = 1, from = "A", to = "B" } });
        }

        // Інформація про один маршрут
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            // Пошук маршруту логіка тут
            return Ok(new { id, from = "A", to = "B" });
        }

        // Додати маршрут (адмін)
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult Create([FromBody] CreateRouteDto dto)
        {
            // Додавання маршруту логіка тут
            return Ok(new { message = "Route created" });
        }

        // Оновити маршрут (адмін)
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateRouteDto dto)
        {
            // Оновлення маршруту логіка тут
            return Ok(new { message = "Route updated" });
        }
    }
}
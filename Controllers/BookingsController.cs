using Microsoft.AspNetCore.Mvc;
using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;

namespace TicketBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        //Створення бронювання автоматично (без вибору місця)
        [HttpPost("auto")]
        [Authorize(Roles = "cashier,client")]
        public async Task<IActionResult> AutoBook([FromBody] AutoBookingRequestDto dto)
        {
            try
            {
                var result = await _bookingService.CreateBookingAsync(dto);
                return Ok(result); // Повертає BookingId + Price
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Перегляд бронювання по ID (для касира або адміністратора)
        [HttpGet("{id}")]
        [Authorize(Roles = "cashier,admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _bookingService.GetBookingDetailsByIdAsync(id);
            if (booking == null)
                return NotFound(new { message = "Бронювання не знайдено" });

            return Ok(booking);
        }

        // Бронювання конкретного користувача
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "client,cashier,admin")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
            return Ok(bookings);
        }

        // Скасування бронювання
        [HttpPost("{id}/cancel")]
        [Authorize(Roles = "client,cashier,admin")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _bookingService.CancelBookingAsync(id);
            return Ok(new { message = "Бронювання скасовано" });
        }
    }
}

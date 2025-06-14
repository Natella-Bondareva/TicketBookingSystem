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

        //Створення броні
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
        {
            try
            {
                var bookingId = await _bookingService.CreateBookingAsync(dto);
                return Ok(new { bookingId, status = "booked" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Отримання броні по ID
        [HttpGet("{id}")]
        [Authorize(Roles = "cashier,admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound(new { message = "Бронювання не знайдено" });

            return Ok(booking);
        }


        //Броні конкретного користувача
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
            return Ok(bookings);
        }

        //Скасування броні
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _bookingService.CancelBookingAsync(id);
            return Ok(new { message = "Booking cancelled" });
        }

        [HttpPost("auto")]
        [Authorize(Roles = "cashier","client")]
        public async Task<IActionResult> AutoBook([FromBody] AutoBookingRequestDto dto)
        {
            var id = await _bookingService.CreateBookingAsync(dto);
            return Ok(new { bookingId = id });
        }

    }
}

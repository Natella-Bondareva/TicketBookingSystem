using Microsoft.AspNetCore.Mvc;
using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Services;


namespace TicketBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        //Оплата + створення квитка
        [HttpPost]
        public async Task<IActionResult> Pay([FromBody] PaymentDto dto)
        {
            try
            {
                var result = await _paymentService.PayAsync(dto);
                return Ok(new
                {
                    paymentId = result.paymentId,
                    ticketId = result.ticketId,
                    status = "paid"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Перегляд оплати
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null) return NotFound();
            return Ok(payment);
        }
    }
}
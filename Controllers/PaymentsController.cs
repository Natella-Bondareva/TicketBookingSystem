using Microsoft.AspNetCore.Mvc;
using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;



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

        /// <summary>
        /// ������ �� ���������� �� ������ ������
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "cashier")]
        public async Task<IActionResult> Pay([FromBody] PaymentRequestDto dto)
        {
            try
            {
                var paymentId = await _paymentService.ProcessPaymentAsync(dto);
                return Ok(new { message = "������ ��������� �� ������ ������", paymentId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        ////�������� ������
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var payment = await _paymentService.GetByIdAsync(id);
        //    if (payment == null) return NotFound();
        //    return Ok(payment);
        //}
    }
}
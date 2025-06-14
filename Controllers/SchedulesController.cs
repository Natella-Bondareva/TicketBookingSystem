using Microsoft.AspNetCore.Mvc;
using TicketBookingSystem.Services;
using TicketBookingSystem.Models.DTOs;

namespace TicketBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly ISearchService _searchService;


        public SchedulesController(IScheduleService scheduleService, ISearchService searchService)
        {
            _scheduleService = scheduleService;
            _searchService = searchService;

        }

        //Отримати всі розклади
        [HttpGet]
        public IActionResult GetAll()
        {
            // Можна реалізувати при потребі
            return Ok(new[] { new { id = 1, routeId = 1, date = "2025-06-13" } });
        }

        //Отримати один розклад за ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            // Можна розширити деталями при потребі
            return Ok(new { id, routeId = 1, date = "2025-06-13" });
        }

        //Отримати розклади для конкретного маршруту
        [HttpGet("route/{routeId}")]
        public IActionResult GetByRoute(int routeId)
        {
            // Можна реалізувати при потребі
            return Ok(new[] { new { id = 1, routeId, date = "2025-06-13" } });
        }

        // Отримати список зупинок для конкретного розкладу
        [HttpGet("{scheduleId}/stops")]
        public async Task<IActionResult> GetStops(int scheduleId)
        {
            var stops = await _scheduleService.GetStopsByScheduleIdAsync(scheduleId);
            return Ok(stops);
        }

        [HttpPost("with-transfers")]
        public async Task<IActionResult> SearchWithTransfers([FromBody] SearchRouteDto dto)
        {
            var results = await _searchService.SearchRoutesWithTransfersAsync(dto);
            return Ok(results);
        }
    }
}

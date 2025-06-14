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

        //�������� �� ��������
        [HttpGet]
        public IActionResult GetAll()
        {
            // ����� ���������� ��� ������
            return Ok(new[] { new { id = 1, routeId = 1, date = "2025-06-13" } });
        }

        //�������� ���� ������� �� ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            // ����� ��������� �������� ��� ������
            return Ok(new { id, routeId = 1, date = "2025-06-13" });
        }

        //�������� �������� ��� ����������� ��������
        [HttpGet("route/{routeId}")]
        public IActionResult GetByRoute(int routeId)
        {
            // ����� ���������� ��� ������
            return Ok(new[] { new { id = 1, routeId, date = "2025-06-13" } });
        }

        // �������� ������ ������� ��� ����������� ��������
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

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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var schedules = await _scheduleService.GetAllAsync();
            return Ok(schedules);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var schedule = await _scheduleService.GetByIdAsync(id);
            if (schedule == null) return NotFound();
            return Ok(schedule);
        }

        [HttpGet("route/{routeId}")]
        public async Task<IActionResult> GetByRoute(int routeId)
        {
            var schedules = await _scheduleService.GetByRouteIdAsync(routeId);
            return Ok(schedules);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateScheduleDto dto)
        {
            var id = await _scheduleService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateScheduleDto dto)
        {
            var success = await _scheduleService.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return Ok(new { message = "Updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _scheduleService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Deleted successfully" });
        }

        [HttpPut("{scheduleId}/stops/{stationId}")]
        public async Task<IActionResult> UpdateStop(int scheduleId, int stationId, [FromBody] UpdateScheduleStopDto dto)
        {
            if (dto.ScheduleId != scheduleId || dto.StationId != stationId)
                return BadRequest("IDs in URL and body must match");

            var success = await _scheduleService.UpdateScheduleStopAsync(dto);
            if (!success) return NotFound();

            return Ok(new { message = "Schedule stop updated" });
        }

        [HttpPost("with-transfers")]
        public async Task<IActionResult> SearchWithTransfers([FromBody] SearchRouteDto dto)
        {
            var results = await _searchService.SearchRoutesWithTransfersAsync(dto);
            return Ok(results);
        }
    }
}

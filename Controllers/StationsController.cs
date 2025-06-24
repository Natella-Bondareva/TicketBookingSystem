using Microsoft.AspNetCore.Mvc;
using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;

namespace TicketBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StationsController : ControllerBase
    {
        private readonly IStationService _stationService;

        public StationsController(IStationService stationService)
        {
            _stationService = stationService;
        }

        // GET: api/stations
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stations = await _stationService.GetAllAsync();
            return Ok(stations);
        }

        // GET: api/stations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var station = await _stationService.GetByIdAsync(id);
            if (station == null)
                return NotFound(new { message = "Station not found" });

            return Ok(station);
        }

        // GET: api/stations/search?name=Central
        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string name)
        {
            var results = await _stationService.SearchByNameAsync(name);
            if (!results.Any())
                return NotFound(new { message = "No stations found" });

            return Ok(results);
        }

        // POST: api/stations
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUpdateStationDto dto)
        {
            var id = await _stationService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        // PUT: api/stations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateStationDto dto)
        {
            var result = await _stationService.UpdateAsync(id, dto);
            if (!result)
                return NotFound(new { message = "Station not found or not updated" });

            return Ok(new { message = "Station updated successfully" });
        }

        // DELETE: api/stations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _stationService.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = "Station not found" });

            return Ok(new { message = "Station deleted successfully" });
        }
    }
}
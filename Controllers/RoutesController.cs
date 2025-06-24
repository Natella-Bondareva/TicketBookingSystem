using Microsoft.AspNetCore.Mvc;
using TicketBookingSystem.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using TicketBookingSystem.Services;


namespace TicketBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController : ControllerBase
    {
        private readonly IRouteService _routeService;

        public RoutesController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var routes = await _routeService.GetAllAsync();
            return Ok(routes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var route = await _routeService.GetByIdAsync(id);
            if (route == null)
                return NotFound();
            return Ok(route);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRouteDto dto)
        {
            var success = await _routeService.CreateAsync(dto);
            return success ? Ok(new { message = "Route created" }) : BadRequest();
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRouteDto dto)
        {
            var success = await _routeService.UpdateAsync(id, dto);
            return success ? Ok(new { message = "Route updated" }) : NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _routeService.DeleteAsync(id);
            return success ? Ok(new { message = "Route deleted" }) : NotFound();
        }
    }

}
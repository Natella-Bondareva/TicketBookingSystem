using Microsoft.AspNetCore.Mvc;
using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Services;

namespace TicketBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpPost("with-availability")]
        public async Task<IActionResult> SearchWithAvailability([FromBody] SearchRouteDto dto)
        {
            var routes = await _searchService.SearchRoutesWithSeatAvailabilityAsync(dto);
            return Ok(routes);
        }
    }

}

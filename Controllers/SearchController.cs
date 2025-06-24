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
            if (dto.AllowTransfers)
            {
                var withTransfers = await _searchService.SearchRoutesWithTransfersAsync(dto);
                return Ok(withTransfers);
            }

            var directRoutes = await _searchService.SearchRoutesWithSeatAvailabilityAsync(dto);
            return Ok(directRoutes);
        }
    }
}

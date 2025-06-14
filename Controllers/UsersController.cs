using Microsoft.AspNetCore.Mvc;
using TicketBookingSystem.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using TicketBookingSystem.Services;
using TicketBookingSystem.Data;
using Microsoft.EntityFrameworkCore;



namespace TicketBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // Реєстрація користувача
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUserDto dto)
        {
            // Реєстрація логіка тут
            return Ok(new { message = "User registered" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Login == dto.Login && u.PasswordHash == dto.Password);

                if (user == null)
                    return Unauthorized(new { message = "Невірний логін або пароль" });

                var token = _tokenService.GenerateToken(user);

                return Ok(new
                {
                    token,
                    userId = user.Id,
                    role = user.Role
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stack = ex.StackTrace });
            }
        }

        // Перегляд профілю
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            // Пошук користувача логіка тут
            return Ok(new { id, name = "User Name" });
        }

        // Оновлення профілю
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            // Оновлення профілю логіка тут
            return Ok(new { message = "User updated" });
        }
    }
}
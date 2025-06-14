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

        // ��������� �����������
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUserDto dto)
        {
            // ��������� ����� ���
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
                    return Unauthorized(new { message = "������� ���� ��� ������" });

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

        // �������� �������
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            // ����� ����������� ����� ���
            return Ok(new { id, name = "User Name" });
        }

        // ��������� �������
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            // ��������� ������� ����� ���
            return Ok(new { message = "User updated" });
        }
    }
}
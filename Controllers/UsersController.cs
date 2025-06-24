using Microsoft.AspNetCore.Mvc;
using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Models.Entities;

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
        private readonly IUserService _userService;


        public UsersController(AppDbContext context, TokenService tokenService, IUserService userService)
        {
            _context = context;
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Login == dto.Login || u.Email == dto.Email))
            {
                return Conflict(new { message = "Користувач з таким логіном або email вже існує." });
            }

            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "admin");
            if (defaultRole == null)
            {
                return StatusCode(500, new { message = "Роль 'client' не знайдена в системі." });
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Login = dto.Login,
                Email = dto.Email,
                Name = dto.Name,
                Surname = dto.Surname,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = hashedPassword,
                RoleId = defaultRole.Id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Користувача успішно зареєстровано." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Login == dto.Login);

                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                {
                    return Unauthorized(new { message = "Невірний логін або пароль" });
                }


                var token = _tokenService.GenerateToken(user);

                var response = new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    Role = user.Role.Name
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stack = ex.StackTrace });
            }
        }

        // Перегляд профілю
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        // Оновлення профілю
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var success = await _userService.UpdateUserAsync(id, dto);
            if (!success)
                return NotFound(new { message = "User not found or not updated" });

            return Ok(new { message = "User updated successfully" });
        }
    }
}
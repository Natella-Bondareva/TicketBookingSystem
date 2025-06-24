namespace TicketBookingSystem.Models.DTOs
{
    public class RegisterUserDto
    {
        public string Login { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }

    public class LoginUserDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
    }

    public class UpdateUserDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }

    public class UserProfileDto
    {
        public string Login { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

}

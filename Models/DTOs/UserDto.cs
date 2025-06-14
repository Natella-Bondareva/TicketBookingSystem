namespace TicketBookingSystem.Models.DTOs
{
    public class RegisterUserDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // client, cashier, admin
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class LoginUserDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
    public class UpdateUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
    }

}

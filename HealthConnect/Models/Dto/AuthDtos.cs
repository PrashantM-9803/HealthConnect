namespace HealthConnect.Models.Dto
{
    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class SignupRequestDto
    {
        public string Email { get; set; } // Required
        public string Password { get; set; } // Required
        public string Name { get; set; } // Required
        public string Role { get; set; } // Required
    }

    public class LoginResponseDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Token { get; set; } // Placeholder for JWT
        public string? RefreshToken { get; set; } // Nullable for consistency
        public DateTime ExpiresAt { get; set; }
    }
}
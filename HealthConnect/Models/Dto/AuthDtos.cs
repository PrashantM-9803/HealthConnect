namespace HealthConnect.Models.Dto
{
    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // New: Role input for login
    }

    public class SignupRequestDto
    {
        public string Email { get; set; } // Required
        public string Password { get; set; } // Required
        public string Name { get; set; } // Required
        public string Role { get; set; } // Required
        public string PhoneNumber { get; set; } // Required
        public DateTime? Dob { get; set; } // Optional
    }

    public class LoginResponseDto
    {
        public Guid Id { get; set; } // UserId
        public Guid? PatientId { get; set; }
        public Guid? DoctorId { get; set; }     
        public string Email { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Token { get; set; } // Placeholder for JWT
        public string? RefreshToken { get; set; } // Nullable for consistency
        public DateTime ExpiresAt { get; set; }
    }
}
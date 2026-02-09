using Microsoft.AspNetCore.Identity;
using System;

namespace HealthConnect.Models
{
    public class User : IdentityUser<Guid>
    {
        // Custom properties
        public string Name { get; set; }                     
        public string PhoneNumber { get; set; }                    
        public DateTime? Dob { get; set; }                     // Optional

        // Optional (for JWT)
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Navigation Properties
        public Patient? Patient { get; set; }                 // One-to-One, optional
        public Doctor? Doctor { get; set; }                   // One-to-One, optional
    }
}
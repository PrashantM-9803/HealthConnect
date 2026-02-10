using System;

namespace HealthConnect.Models.Dto
{
    public class PatientUpdateProfileDto
    {
        public string FullName { get; set; }
        public string Email { get; set; } // Read-only, will not be updated
        public string Phone { get; set; }
        public DateTime? Dob { get; set; }
        public string? Address { get; set; }
    }
}

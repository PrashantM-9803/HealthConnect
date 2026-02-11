using System;

namespace HealthConnect.Models
{
    public class DoctorSlot
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsBooked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Doctor Doctor { get; set; }
        public Appointment? Appointment { get; set; }
    }
}

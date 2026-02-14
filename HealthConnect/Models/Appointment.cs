using System;

namespace HealthConnect.Models
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public Guid SlotId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public string Reason { get; set; }

        // Navigation properties
        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }
        public DoctorSlot Slot { get; set; }
        public Invoice Invoice { get; set; }
        public List<Medications> Medications { get; set; }
        public Vitals Vitals { get; set; }
        public Diagnosis Diagnosis { get; set; }
    }

    public enum AppointmentStatus
    {
        Pending = 0,
        Completed = 1,
        Cancelled = 2
    }
}
using System;

namespace HealthConnect.Models.Dto
{
    public class DoctorWorkloadDto
    {
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public double AverageAppointmentsPerDay { get; set; }
        public DateTime? LastAppointmentDate { get; set; }
    }

    public class DoctorWorkloadResponseDto
    {
        public List<DoctorWorkloadDto> Doctors { get; set; } = new();
        public int TotalCount { get; set; }
    }
}

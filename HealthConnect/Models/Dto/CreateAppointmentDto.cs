using System;

namespace HealthConnect.Models.Dto
{
    public class CreateAppointmentDto
    {
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; }
    }
}

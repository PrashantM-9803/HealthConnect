using System;

namespace HealthConnect.Models.Dto
{
    public class CreateAppointmentDto
    {
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public Guid SlotId { get; set; }
        public string Reason { get; set; }
    }
}


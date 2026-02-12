using System;

namespace HealthConnect.Models.Dto
{
    public class AddDiagnosisDto
    {
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public string DiagnosisDetails { get; set; }
    }
}

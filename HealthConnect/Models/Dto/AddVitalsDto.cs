using System;

namespace HealthConnect.Models.Dto
{
    public class AddVitalsDto
    {
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public string BloodPressure { get; set; }
        public int HeartRate { get; set; }
        public float Temperature { get; set; }
        public int SpO2 { get; set; }
    }
}

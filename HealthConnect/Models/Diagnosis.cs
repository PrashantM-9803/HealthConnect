namespace HealthConnect.Models
{
    public class Diagnosis
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public string DiagnosisDetails { get; set; }

        // navigation property
        public Appointment Appointment { get; set; }
        public Patient Patient { get; set; }
    }
}

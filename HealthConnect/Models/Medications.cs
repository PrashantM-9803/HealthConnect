namespace HealthConnect.Models
{
    public class Medications
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }

        

        public string Drug { get; set; }
        public string Dose { get; set; }
        public string Route { get; set; }
        public string Frequency { get; set; }

        public Activity Activity { get; set; }
        public Appointment Appointment { get; set; }
        public Patient Patient { get; set; }

    }

    public enum Activity
    {
        Active,
        OnHold,
        Completed,
        Pending
    }
}

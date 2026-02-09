namespace HealthConnect.Models
{
    public class Vitals
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }

        public string BloodPressure { get; set; }
        public int HeartRate { get; set; }
        public float Temperature { get; set; }
        public int SpO2 { get; set; }


       

        // Navigation Property
        public Appointment Appointment { get; set; }
        public Patient Patient { get; set; }
    

}
}

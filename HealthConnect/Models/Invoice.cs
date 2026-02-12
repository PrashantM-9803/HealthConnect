namespace HealthConnect.Models
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime IssuedDate { get; set; }
        public string Status { get; set; }

        public string ConsultationType { get; set; }
        public int ConsulationFee { get; set; }

        public int LabFee { get; set; }

        public int MedicineFee { get; set; } // Added property

        public int Total { get; set; }
        public int? Outstanding { get; set; }
    

       

        // Navigation Property
        public Appointment Appointment { get; set; }
        public Patient Patient { get; set; }
    }
}

using System;

namespace HealthConnect.Models.Dto
{
    public class AddInvoiceDto
    {
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public string ConsultationType { get; set; }
        public int ConsulationFee { get; set; }
        public int LabFee { get; set; }
        public int MedicineFee { get; set; }
        public int Total { get; set; } // Calculated on frontend, sent to backend\

       
    }
}

using System;

namespace HealthConnect.Models.Dto
{
    public enum Activity
    {
        Active,
        OnHold,
        Completed,
        Pending
    }

    public class AddMedicationsDto
    {
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public string Drug { get; set; }
        public string Dose { get; set; }
        public string Route { get; set; }
        public string Frequency { get; set; }
        public Activity Activity { get; set; }
    }
}

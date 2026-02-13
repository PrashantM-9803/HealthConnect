using System;
using System.Collections.Generic;

namespace HealthConnect.Models.Dto
{
    public class PatientDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? DoctorId { get; set; }
        public string BloodGroup { get; set; }

        public string? Address { get; set; }

        public string ProfileImage { get; set; }


        public UserDto User { get; set; }
        public DoctorDto Doctor { get; set; }
        public List<AppointmentDto> Appointments { get; set; }
       
    }   

    public class VitalsDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public string? BloodPressure { get; set; }
        public int HeartRate { get; set; }
        public float Temperature { get; set; }
        public int SpO2 { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? Dob { get; set; }
    }

    public class DoctorDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Specialization { get; set; }
        public int YearsOfExperience { get; set; }
        public int? MemberSince { get; set; }
        public string Bio { get; set; }
        public UserDto User { get; set; }
        public List<PatientDto> Patients { get; set; }
        public List<AppointmentDto> Appointments { get; set; }
    }

    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public Guid SlotId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Status { get; set; }
        public string Reason { get; set; }

        // Navigation properties
        public VitalsDto Vitals { get; set; }
        public List<MedicationsDto> Medications { get; set; }
        public InvoiceDto Invoice { get; set; }
        public DiagnosisDto Diagnosis { get; set; }
    }



    public class MedicationsDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public string Drug { get; set; }
        public string Dose { get; set; }
        public string Route { get; set; }
        public string Frequency { get; set; }
        public int Activity { get; set; }
    }

    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime IssuedDate { get; set; }
        public string Status { get; set; }
        public string ConsultationType { get; set; }
        public int ConsulationFee { get; set; }
        public int LabFee { get; set; }
        public int Total { get; set; }
        public int? Outstanding { get; set; }
    }

    public class DiagnosisDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public string DiagnosisDetails { get; set; }
    }
}

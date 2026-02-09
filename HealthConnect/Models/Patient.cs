using System;
using System.Collections.Generic;

namespace HealthConnect.Models
{
    public class Patient
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DoctorId { get; set; } // Foreign key to Doctor

        public string MedicalHistory { get; set; }
        public string BloodGroup { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Doctor Doctor { get; set; }
        public List<Appointment> Appointments { get; set; } = new();
        public List<Vitals> Vitals { get; set; } = new();
        public List<Medications> Medications { get; set; } = new();
        public List<Invoice> Invoices { get; set; } = new();
        public List<Diagnosis> Diagnoses { get; set; } = new();
    }
}
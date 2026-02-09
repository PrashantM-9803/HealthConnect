using System;
using System.Collections.Generic;

namespace HealthConnect.Models
{
    public class Doctor
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Specialization { get; set; }
        public int YearsOfExperience { get; set; }
        public int? MemberSince { get; set; }
        public string Bio { get; set; }

        // Navigation properties
        public User User { get; set; }
        public List<Patient> Patients { get; set; } = new();
        public List<Appointment> Appointments { get; set; } = new();
    }
}
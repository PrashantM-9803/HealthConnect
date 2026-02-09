using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HealthConnect.Models;

namespace HealthConnect.Data
{
    public class HealthConnectDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public HealthConnectDbContext(DbContextOptions<HealthConnectDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Prevent cascade delete from Doctor to Patient
            builder.Entity<Patient>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.Patients)
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent cascade delete from Patient to Appointment
            builder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent cascade delete from Doctor to Appointment
            builder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent cascade delete from Appointment to Diagnosis
            builder.Entity<Diagnosis>()
                .HasOne(d => d.Appointment)
                .WithOne(a => a.Diagnosis)
                .HasForeignKey<Diagnosis>(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent cascade delete from Appointment to Invoice
            builder.Entity<Invoice>()
                .HasOne(i => i.Appointment)
                .WithOne(a => a.Invoice)
                .HasForeignKey<Invoice>(i => i.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent cascade delete from Appointment to Medications
            builder.Entity<Medications>()
                .HasOne(m => m.Appointment)
                .WithOne(a => a.Medications)
                .HasForeignKey<Medications>(m => m.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent cascade delete from Appointment to Vitals
            builder.Entity<Vitals>()
                .HasOne(v => v.Appointment)
                .WithOne(a => a.Vitals)
                .HasForeignKey<Vitals>(v => v.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
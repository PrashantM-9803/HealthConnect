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
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorSlot> DoctorSlots { get; set; }
        public DbSet<Vitals> Vitals { get; set; }
        public DbSet<Medications> Medications { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }

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

            // Configure DoctorSlot relationships
            builder.Entity<DoctorSlot>()
                .HasOne(ds => ds.Doctor)
                .WithMany(d => d.DoctorSlots)
                .HasForeignKey(ds => ds.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Appointment-Slot relationship
            builder.Entity<Appointment>()
                .HasOne(a => a.Slot)
                .WithOne(s => s.Appointment)
                .HasForeignKey<Appointment>(a => a.SlotId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent cascade delete from Appointment to Diagnosis
            builder.Entity<Diagnosis>()
                .HasOne(d => d.Appointment)
                .WithOne(a => a.Diagnosis)
                .HasForeignKey<Diagnosis>(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
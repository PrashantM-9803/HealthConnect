using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthConnect.Data;
using HealthConnect.Models;
using HealthConnect.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace HealthConnect.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly HealthConnectDbContext _context;

        public AppointmentRepository(HealthConnectDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto dto)
        {
            // Verify the slot exists and is available
            var slot = await _context.DoctorSlots.FindAsync(dto.SlotId);
            if (slot == null)
                throw new Exception("Slot not found.");

            if (slot.IsBooked)
                throw new Exception("Slot is already booked.");

            if (slot.DoctorId != dto.DoctorId)
                throw new Exception("Slot does not belong to the specified doctor.");

            // Create the appointment
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                SlotId = dto.SlotId,
                AppointmentDate = slot.Date,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                Status = AppointmentStatus.Pending,
                Reason = dto.Reason
            };

            // Mark the slot as booked
            slot.IsBooked = true;
            slot.UpdatedAt = DateTime.UtcNow;

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            
            return appointment;
        }

        public async Task<Appointment> GetAppointmentByIdAsync(Guid id)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Slot)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Slot)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Slot)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<bool> CancelAppointmentAsync(Guid appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Slot)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                return false;

            // Update appointment status
            appointment.Status = AppointmentStatus.Cancelled;

            // Free up the slot
            if (appointment.Slot != null)
            {
                appointment.Slot.IsBooked = false;
                appointment.Slot.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAppointmentStatusAsync(Guid appointmentId, AppointmentStatus status)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return false;

            appointment.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}


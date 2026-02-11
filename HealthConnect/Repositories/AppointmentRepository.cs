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
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                AppointmentDate = dto.AppointmentDate,
                Status = AppointmentStatus.Pending,
                Reason = dto.Reason
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<Appointment> GetAppointmentByIdAsync(Guid id)
        {
            return await _context.Appointments.FindAsync(id);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId)
        {
            return await _context.Appointments.Where(a => a.PatientId == patientId).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Appointments.Where(a => a.DoctorId == doctorId).ToListAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthConnect.Data;
using HealthConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthConnect.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly HealthConnectDbContext _context;

        public AdminRepository(UserManager<User> userManager, HealthConnectDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> UpdateUserPasswordAsync(Guid userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            // Generate password reset token and reset password (admin override)
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            return result.Succeeded;
        }

        public async Task<bool> DeletePatientAsync(Guid userId)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
            
            if (patient == null)
                return false;

            var user = patient.User;

            // Remove patient record
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            // Remove associated user account
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                    return false;
            }

            return true;
        }

        public async Task<bool> DeleteDoctorAsync(Guid userId)
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);
            
            if (doctor == null)
                return false;

            var user = doctor.User;

            // Remove doctor record
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            // Remove associated user account
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                    return false;
            }

            return true;
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Slot)
                .Include(a => a.Invoice)
                .Include(a => a.Medications)
                .Include(a => a.Vitals)
                .Include(a => a.Diagnosis)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetPendingAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Slot)
                .Include(a => a.Invoice)
                .Include(a => a.Medications)
                .Include(a => a.Vitals)
                .Include(a => a.Diagnosis)
                .Where(a => a.Status == AppointmentStatus.Pending)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<int> GetTotalPatientsAsync()
        {
            return await _context.Patients.CountAsync();
        }

        public async Task<int> GetTotalDoctorsAsync()
        {
            return await _context.Doctors.CountAsync();
        }

        public async Task<int> GetTotalAppointmentsAsync()
        {
            return await _context.Appointments.CountAsync();
        }
    }
}

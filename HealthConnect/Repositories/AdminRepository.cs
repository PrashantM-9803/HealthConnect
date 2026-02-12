using System;
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

        public async Task<bool> DeletePatientAsync(Guid patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null)
                return false;

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDoctorAsync(Guid userId)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == userId);
            
            if (doctor == null)
                return false;

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

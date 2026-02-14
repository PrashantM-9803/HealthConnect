using System;
using System.Threading.Tasks;
using HealthConnect.Data;
using HealthConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthConnect.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly HealthConnectDbContext _context;
        private readonly UserManager<User> _userManager;
        public PatientRepository(HealthConnectDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            return await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.User)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Slot)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Vitals)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Medications)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Invoice)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Diagnosis)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Doctor)
                        .ThenInclude(d => d.User)
                .ToListAsync();
        }

        public async Task<Patient?> GetPatientByIdAsync(Guid id)
        {
            return await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.User)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Slot)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Vitals)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Medications)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Invoice)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Diagnosis)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Doctor)
                        .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Patient?> GetPatientByUserIdAsync(Guid userId)
        {
            // Validate user exists using UserManager
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return null;
            return await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.User)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Slot)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Vitals)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Medications)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Invoice)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Diagnosis)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Doctor)
                        .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<bool> UpdatePatientProfileAsync(Guid userId, Models.Dto.PatientUpdateProfileDto updateDto)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return false;

            // Update User fields (do NOT update Email)
            patient.User.Name = updateDto.FullName;
            patient.User.PhoneNumber = updateDto.Phone;
            patient.User.Dob = updateDto.Dob;

            // Update Patient fields
            patient.Address = updateDto.Address;
            patient.BloodGroup = updateDto.BloodGroup;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePatientProfileImageAsync(Guid userId, string profileImagePath)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return false;

            patient.ProfileImage = profileImagePath;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
